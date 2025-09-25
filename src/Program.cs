namespace Miksado;

using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using BizHawk.Emulation.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

[ExternalTool("miksado")]
public sealed class MiksadoToolForm : ToolFormBase, IExternalToolForm
{
    // enums
    private enum ShuffleAlgorithm
    {
        TrueRandom,
        ForceNew,
        Bag,
    }

    private enum RandomNumberGeneratorModel
    {
        System,
        SM64,
        FinalFantasy1,
        Doom,
        PokemonGen1
    }

    private enum ShuffleTrigger
    {
        Timer,
        File,
        Twitch
    }

    private enum LogLevel
    {
        Info,
        Debug,
    }

    // UI elements
    private Label RNGLabel = null!;
    private Label AlgorithmLabel = null!;
    private Label MinTimeLabel = null!;
    private Label MaxTimeLabel = null!;
    private Label SeedTextBoxLabel = null!;

    private TextBox ConsoleMultiline = null!;
    private TextBox SeedTextBox = null!;

    private Button StartButton = null!;
    private Button ResumeButton = null!;
    private Button ForceShuffleButton = null!;

    private ComboBox RngComboBox = null!;
    private ComboBox AlgorithmComboBox = null!;
    private CheckBox DebugLogCheckBox = null!;

    private NumericUpDown MinTimeUpDown = null!;
    private NumericUpDown MaxTimeUpDown = null!;

    private CheckedListBox ShuffleTriggerListBox = null!;

    protected override string WindowTitleStatic
    => "miksado";

    // BizHawk API setup
    public ApiContainer? MaybeAPIContainer { get; set; }

    private ApiContainer APIs
        => MaybeAPIContainer!;

    // constants
    private static readonly RandomNumberGeneratorModel DefaultRngModel = RandomNumberGeneratorModel.System;
    private static readonly ShuffleAlgorithm DefaultShuffleAlgorithm = ShuffleAlgorithm.ForceNew;
    private static readonly ShuffleTrigger[] DefaultShuffleTriggers = [ShuffleTrigger.Timer];

    private static readonly string MiksadoPath = Path.Combine("miksado");
    private static readonly string MiksadoGamePath = Path.Combine(MiksadoPath, "game");
    private static readonly string MiksadoStatePath = Path.Combine(MiksadoPath, "state");
    private static readonly string MiksadoDataPath = Path.Combine(MiksadoPath, "data");

    private static readonly string[] InvalidGameExtensions = [".bin"];
    private static readonly string SaveStatePrefix = "MSAV_";
    private static readonly int ShuffleRetryLimit = 10;

    // variables
    private IGameInfo? CurrentGameInfo = null;
    private string CurrentGamePath = "";

    private RandomNumberGeneratorModel CurrentRngModel;
    private ShuffleAlgorithm CurrentShuffleAlgorithm;
    private ShuffleTrigger[] ActiveShuffleTriggers; // try to ensure at most one of each trigger?

    private Random SystemRng = null!;
    // add SM64 rng state
    // add FF1 rng state
    // etc

    private bool ShouldShuffle = false;
    private bool IsShuffling = false;
    private bool IsActiveSession = false;

    private readonly Dictionary<string, string> GameStateMap = [];
    private readonly string[] AllGames = [];
    private readonly string[] AllStates = [];
    private System.Timers.Timer MiksadoTimer = null!;
    private int ShuffleRetryCount = 0;
    private LogLevel CurrentLogLevel;

    // constructor
    public MiksadoToolForm()
    {
        InitializeComponent();

        ConsoleMultiline.Text = "";
        LogConsoleDebug("miksado tool loaded");

        // ensure directories exist
        if (!Directory.Exists(MiksadoPath))
        {
            Directory.CreateDirectory(MiksadoPath);
        }
        if (!Directory.Exists(MiksadoGamePath))
        {
            Directory.CreateDirectory(MiksadoGamePath);
        }
        if (!Directory.Exists(MiksadoStatePath))
        {
            Directory.CreateDirectory(MiksadoStatePath);
        }
        if (!Directory.Exists(MiksadoDataPath))
        {
            Directory.CreateDirectory(MiksadoDataPath);
        }

        // setup logging
        CurrentLogLevel = LogLevel.Debug;
        DebugLogCheckBox.Checked = CurrentLogLevel == LogLevel.Debug;
        DebugLogCheckBox.CheckedChanged += (s, e) =>
        {
            CurrentLogLevel = DebugLogCheckBox.Checked ? LogLevel.Debug : LogLevel.Info;
            LogConsoleDebug($"log level set to {CurrentLogLevel}");
        };

        // setup rng model
        RngComboBox.Items.AddRange(System.Enum.GetNames(typeof(RandomNumberGeneratorModel)));
        RngComboBox.SelectedIndex = (int)DefaultRngModel;
        RngComboBox.SelectedIndexChanged += (s, e) =>
        {
            CurrentRngModel = (RandomNumberGeneratorModel)RngComboBox.SelectedIndex;
            LogConsoleDebug($"RNG set to {CurrentRngModel}");
        };
        CurrentRngModel = (RandomNumberGeneratorModel)RngComboBox.SelectedIndex;

        // setup algorithm
        AlgorithmComboBox.Items.AddRange(System.Enum.GetNames(typeof(ShuffleAlgorithm)));
        AlgorithmComboBox.SelectedIndex = (int)DefaultShuffleAlgorithm;
        AlgorithmComboBox.SelectedIndexChanged += (s, e) =>
        {
            CurrentShuffleAlgorithm = (ShuffleAlgorithm)AlgorithmComboBox.SelectedIndex;
            LogConsoleDebug($"shuffle algorithm set to {CurrentShuffleAlgorithm}");
        };
        CurrentShuffleAlgorithm = (ShuffleAlgorithm)AlgorithmComboBox.SelectedIndex;

        // setup shuffle trigger settings
        ShuffleTriggerListBox.Items.Clear();
        ActiveShuffleTriggers = DefaultShuffleTriggers;

        foreach (string triggerName in System.Enum.GetNames(typeof(ShuffleTrigger)))
        {
            ShuffleTriggerListBox.Items.Add(triggerName, false);
        }

        foreach (ShuffleTrigger trigger in ActiveShuffleTriggers)
        {
            ShuffleTriggerListBox.SetItemChecked((int)trigger, true);
        }

        ShuffleTriggerListBox.ItemCheck += (s, e) =>
        {
            int index = e.Index;
            bool isChecked = e.NewValue == CheckState.Checked;
            ShuffleTrigger thisTrigger = (ShuffleTrigger)index;
            if (isChecked)
            {
                if (!ActiveShuffleTriggers.Contains(thisTrigger))
                {
                    ActiveShuffleTriggers = [.. ActiveShuffleTriggers, thisTrigger];
                    LogConsoleDebug($"added active trigger: {thisTrigger}");
                }
            }
            else
            {
                if (ActiveShuffleTriggers.Contains(thisTrigger))
                {
                    ActiveShuffleTriggers = [.. ActiveShuffleTriggers.Where(t => t != thisTrigger)];
                    LogConsoleDebug($"removed active trigger: {thisTrigger}");
                }
            }
        };

        // load games and states
        string[] rawGameFiles = Directory.GetFiles(MiksadoGamePath, "*", SearchOption.AllDirectories);
        AllGames = [.. rawGameFiles.Where(filePath => !InvalidGameExtensions.Contains(Path.GetExtension(filePath).ToLower()))];

        LogConsoleDebug($"found {AllGames.Length} valid game files");

        foreach (string gamePath in AllGames)
        {
            string gameFileName = Path.GetFileNameWithoutExtension(gamePath);
            LogConsoleDebug($"game file: {gameFileName}");
        }

        string[] rawStateFiles = Directory.GetFiles(MiksadoStatePath, "*", SearchOption.AllDirectories);
        AllStates = [.. rawStateFiles.Where(filePath => Path.GetFileName(filePath).StartsWith(SaveStatePrefix))];

        LogConsoleDebug($"found {AllStates.Length} valid state files");

        foreach (string gamePath in AllGames)
        {
            foreach (string statePath in AllStates)
            {
                string gameFileName = Path.GetFileNameWithoutExtension(gamePath);
                string stateFileName = Path.GetFileNameWithoutExtension(statePath);

                if (stateFileName.StartsWith(SaveStatePrefix) && stateFileName.EndsWith(gameFileName))
                {
                    GameStateMap[gamePath] = statePath;
                    break;
                }
            }
        }

        LogConsoleDebug($"mapped {GameStateMap.Count} game files to state files");

        // basic validate game files
        if (AllGames.Length == 0)
        {
            LogConsoleInfo("no valid game files found, tool will not function");
            StartButton.Enabled = false;
            ResumeButton.Enabled = false;
            ForceShuffleButton.Enabled = false;
            return;
        }

        if (AllGames.Length == 1)
        {
            LogConsoleInfo("only one valid game file found, shuffling will have no effect");
        }

        StartButton.Enabled = true;
        if (GameStateMap.Count > 0)
        {
            ResumeButton.Enabled = true;
        }
    }

    /// <summary>
    /// called when the tool is first opened, and when a new ROM is loaded
    /// </summary>
    public override void Restart()
    {
        LogConsoleDebug("miksado restarted");
    }

    /// <summary>
    /// called after every frame when a ROM is actually running
    /// </summary>
    protected override void UpdateAfter()
    {
        IGameInfo? info = APIs.Emulation.GetGameInfo();
        string gameTitleString;
        if (info == null)
        {
            gameTitleString = "no game yet...";
        }
        else
        {
            gameTitleString = $"{info.Name} ({info.System})";
        }
        APIs.Gui.Text(0, 0, gameTitleString, System.Drawing.Color.LimeGreen, "topright");

        if (ShouldShuffle && !IsShuffling)
        {
            LogConsoleDebug("ShouldShuffle is true, forcing shuffle");
            ShouldShuffle = false;
            if (InvokeRequired)
            {
                Invoke(new System.Action(ShuffleGame));
            }
            else
            {
                ShuffleGame();
            }
        }
    }

    private void ShuffleGame()
    {
        if (IsShuffling)
        {
            LogConsoleDebug("already shuffling");
            return;
        }

        try
        {
            IsShuffling = true;

            if (CurrentGameInfo != null)
            {
                LogConsoleDebug("saving current state");
                string newStateStatePath = Path.Combine(MiksadoStatePath, $"{SaveStatePrefix}{Path.GetFileNameWithoutExtension(CurrentGamePath)}.state");
                LogConsoleDebug($"saving state to: {newStateStatePath}");
                APIs.SaveState.Save(newStateStatePath);
                GameStateMap[CurrentGamePath] = newStateStatePath;
            }

            LogConsoleInfo("shuffling game");

            string randomGamePath = PickRandomGamePath();
            APIs.EmuClient.OpenRom(randomGamePath);
            CurrentGamePath = randomGamePath;
            LogConsoleDebug($"loaded random game: {Path.GetFileNameWithoutExtension(CurrentGamePath)}");

            if (GameStateMap.ContainsKey(CurrentGamePath))
            {
                string statePath = GameStateMap[CurrentGamePath]!;
                LogConsoleDebug($"loading state: {Path.GetFileName(statePath)}");
                APIs.SaveState.Load(statePath);
            }
            else
            {
                LogConsoleDebug("no save state found for this game, starting fresh");
            }
        }
        catch (System.Exception ex)
        {
            LogConsoleInfo($"error shuffling game: {ex.Message}");
        }
        finally
        {
            CurrentGameInfo = APIs.Emulation.GetGameInfo();

            // setup next shuffle triggers
            if (ActiveShuffleTriggers.Contains(ShuffleTrigger.Timer))
            {
                int minSeconds = (int)MinTimeUpDown.Value;
                int maxSeconds = (int)MaxTimeUpDown.Value;
                int nextInterval = new System.Random().Next(minSeconds * 1000, maxSeconds * 1000);
                MiksadoTimer.Interval = nextInterval;
                LogConsoleDebug($"next shuffle in {nextInterval / 1000} seconds");
                MiksadoTimer.Start();
            }

            IsShuffling = false;
        }
    }

    private void WriteConsoleMultiline(string text)
    {
        if (InvokeRequired)
        {
            Invoke(new System.Action(() => WriteConsoleMultiline(text)));
            return;
        }

        if (ConsoleMultiline.Text.Length > 0)
        {
            ConsoleMultiline.AppendText("\r\n");
        }

        ConsoleMultiline.AppendText(text);
    }

    private void LogConsoleInfo(string text)
    {
        if (CurrentLogLevel == LogLevel.Info || CurrentLogLevel == LogLevel.Debug)
        {
            WriteConsoleMultiline($"[INFO] {text}");
        }
    }

    private void LogConsoleDebug(string text)
    {
        if (CurrentLogLevel == LogLevel.Debug)
        {
            WriteConsoleMultiline($"[DEBUG] {text}");
        }
    }

    private string PickRandomGamePath()
    {
        switch (CurrentShuffleAlgorithm)
        {
            case ShuffleAlgorithm.TrueRandom:
                int trueRandNum = GetNextRandomNumber();
                int trueRandIdx = trueRandNum % AllGames.Length;
                LogConsoleDebug($"true random index: {trueRandIdx}");
                string trueRandomGame = AllGames[trueRandIdx];
                LogConsoleDebug($"true random selected game: {Path.GetFileNameWithoutExtension(trueRandomGame)}");
                return trueRandomGame;

            case ShuffleAlgorithm.ForceNew:
                ShuffleRetryCount = 0;
                string forceNextGamePath = CurrentGamePath;
                while (forceNextGamePath == CurrentGamePath && AllGames.Length > 1 && ShuffleRetryCount <= ShuffleRetryLimit)
                {
                    ShuffleRetryCount++;
                    int forceNextRandNum = GetNextRandomNumber();
                    int allGamesIndex = forceNextRandNum % AllGames.Length;
                    LogConsoleDebug($"force new try {ShuffleRetryCount}: index {allGamesIndex}");
                    forceNextGamePath = AllGames[allGamesIndex];
                }
                LogConsoleDebug($"force new selected game: {Path.GetFileNameWithoutExtension(forceNextGamePath)} after {ShuffleRetryCount} try");
                return forceNextGamePath;

            case ShuffleAlgorithm.Bag:
            // Implement bag algorithm here

            default:
                LogConsoleInfo("rando error, default first game");
                return AllGames[0];
        }
    }

    private int GetNextRandomNumber()
    {
        int r;

        switch (CurrentRngModel)
        {
            case RandomNumberGeneratorModel.System:
                r = SystemRng.Next();
                break;
            case RandomNumberGeneratorModel.SM64:
            // Implement SM64 RNG here
            case RandomNumberGeneratorModel.FinalFantasy1:
            // Implement Final Fantasy I RNG here
            case RandomNumberGeneratorModel.Doom:
            // Implement Doom RNG here
            default:
                LogConsoleInfo("rng error, default 0");
                r = 0;
                break;
        }

        LogConsoleDebug($"generated random number: {r} using {CurrentRngModel}");
        return r;
    }

    private void InitializeComponent()
    {
        this.ConsoleMultiline = new System.Windows.Forms.TextBox();
        this.ForceShuffleButton = new System.Windows.Forms.Button();
        this.StartButton = new System.Windows.Forms.Button();
        this.ResumeButton = new System.Windows.Forms.Button();
        this.RngComboBox = new System.Windows.Forms.ComboBox();
        this.AlgorithmComboBox = new System.Windows.Forms.ComboBox();
        this.RNGLabel = new System.Windows.Forms.Label();
        this.AlgorithmLabel = new System.Windows.Forms.Label();
        this.DebugLogCheckBox = new System.Windows.Forms.CheckBox();
        this.MinTimeUpDown = new System.Windows.Forms.NumericUpDown();
        this.MaxTimeUpDown = new System.Windows.Forms.NumericUpDown();
        this.MinTimeLabel = new System.Windows.Forms.Label();
        this.MaxTimeLabel = new System.Windows.Forms.Label();
        this.ShuffleTriggerListBox = new System.Windows.Forms.CheckedListBox();
        this.SeedTextBox = new System.Windows.Forms.TextBox();
        this.SeedTextBoxLabel = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.MinTimeUpDown)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.MaxTimeUpDown)).BeginInit();
        this.SuspendLayout();
        // 
        // ConsoleMultiline
        // 
        this.ConsoleMultiline.BackColor = System.Drawing.SystemColors.ControlText;
        this.ConsoleMultiline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.ConsoleMultiline.Cursor = System.Windows.Forms.Cursors.Default;
        this.ConsoleMultiline.Dock = System.Windows.Forms.DockStyle.Right;
        this.ConsoleMultiline.ForeColor = System.Drawing.SystemColors.Control;
        this.ConsoleMultiline.Location = new System.Drawing.Point(184, 0);
        this.ConsoleMultiline.Multiline = true;
        this.ConsoleMultiline.Name = "ConsoleMultiline";
        this.ConsoleMultiline.ReadOnly = true;
        this.ConsoleMultiline.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.ConsoleMultiline.Size = new System.Drawing.Size(400, 361);
        this.ConsoleMultiline.TabIndex = 0;
        this.ConsoleMultiline.Text = "console_1\r\nconsole_2\r\nconsole_3";
        // 
        // ForceShuffleButton
        // 
        this.ForceShuffleButton.Enabled = false;
        this.ForceShuffleButton.Location = new System.Drawing.Point(12, 41);
        this.ForceShuffleButton.Name = "ForceShuffleButton";
        this.ForceShuffleButton.Size = new System.Drawing.Size(166, 23);
        this.ForceShuffleButton.TabIndex = 1;
        this.ForceShuffleButton.Text = "force shuffle";
        this.ForceShuffleButton.UseVisualStyleBackColor = true;
        this.ForceShuffleButton.Click += new System.EventHandler(this.ForceShuffleButton_Click);
        // 
        // StartButton
        // 
        this.StartButton.Enabled = false;
        this.StartButton.Location = new System.Drawing.Point(12, 12);
        this.StartButton.Name = "StartButton";
        this.StartButton.Size = new System.Drawing.Size(72, 23);
        this.StartButton.TabIndex = 2;
        this.StartButton.Text = "start new";
        this.StartButton.UseVisualStyleBackColor = true;
        this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
        // 
        // ResumeButton
        // 
        this.ResumeButton.Enabled = false;
        this.ResumeButton.Location = new System.Drawing.Point(106, 12);
        this.ResumeButton.Name = "ResumeButton";
        this.ResumeButton.Size = new System.Drawing.Size(72, 23);
        this.ResumeButton.TabIndex = 3;
        this.ResumeButton.Text = "resume";
        this.ResumeButton.UseVisualStyleBackColor = true;
        this.ResumeButton.Click += new System.EventHandler(this.ResumeButton_Click);
        // 
        // RNGComboBox
        // 
        this.RngComboBox.FormattingEnabled = true;
        this.RngComboBox.Location = new System.Drawing.Point(12, 89);
        this.RngComboBox.Name = "RNGComboBox";
        this.RngComboBox.Size = new System.Drawing.Size(166, 21);
        this.RngComboBox.TabIndex = 4;
        // 
        // AlgorithmComboBox
        // 
        this.AlgorithmComboBox.FormattingEnabled = true;
        this.AlgorithmComboBox.Location = new System.Drawing.Point(12, 129);
        this.AlgorithmComboBox.Name = "AlgorithmComboBox";
        this.AlgorithmComboBox.Size = new System.Drawing.Size(166, 21);
        this.AlgorithmComboBox.TabIndex = 5;
        // 
        // RNGLabel
        // 
        this.RNGLabel.AutoSize = true;
        this.RNGLabel.Location = new System.Drawing.Point(9, 73);
        this.RNGLabel.Name = "RNGLabel";
        this.RNGLabel.Size = new System.Drawing.Size(128, 13);
        this.RNGLabel.TabIndex = 6;
        this.RNGLabel.Text = "random number generator";
        // 
        // AlgorithmLabel
        // 
        this.AlgorithmLabel.AutoSize = true;
        this.AlgorithmLabel.Location = new System.Drawing.Point(9, 113);
        this.AlgorithmLabel.Name = "AlgorithmLabel";
        this.AlgorithmLabel.Size = new System.Drawing.Size(49, 13);
        this.AlgorithmLabel.TabIndex = 7;
        this.AlgorithmLabel.Text = "algorithm";
        // 
        // DebugLogCheckBox
        // 
        this.DebugLogCheckBox.AutoSize = true;
        this.DebugLogCheckBox.Location = new System.Drawing.Point(12, 332);
        this.DebugLogCheckBox.Name = "DebugLogCheckBox";
        this.DebugLogCheckBox.Size = new System.Drawing.Size(73, 17);
        this.DebugLogCheckBox.TabIndex = 8;
        this.DebugLogCheckBox.Text = "debug log";
        this.DebugLogCheckBox.UseVisualStyleBackColor = true;
        // 
        // MinTimeUpDown
        // 
        this.MinTimeUpDown.Location = new System.Drawing.Point(12, 156);
        this.MinTimeUpDown.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
        this.MinTimeUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
        this.MinTimeUpDown.Name = "MinTimeUpDown";
        this.MinTimeUpDown.Size = new System.Drawing.Size(46, 20);
        this.MinTimeUpDown.TabIndex = 9;
        this.MinTimeUpDown.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
        // 
        // MaxTimeUpDown
        // 
        this.MaxTimeUpDown.Location = new System.Drawing.Point(94, 156);
        this.MaxTimeUpDown.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
        this.MaxTimeUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
        this.MaxTimeUpDown.Name = "MaxTimeUpDown";
        this.MaxTimeUpDown.Size = new System.Drawing.Size(46, 20);
        this.MaxTimeUpDown.TabIndex = 10;
        this.MaxTimeUpDown.Value = new decimal(new int[] {
            45,
            0,
            0,
            0});
        // 
        // MinTimeLabel
        // 
        this.MinTimeLabel.AutoSize = true;
        this.MinTimeLabel.Location = new System.Drawing.Point(64, 159);
        this.MinTimeLabel.Name = "MinTimeLabel";
        this.MinTimeLabel.Size = new System.Drawing.Size(23, 13);
        this.MinTimeLabel.TabIndex = 11;
        this.MinTimeLabel.Text = "min";
        // 
        // MaxTimeLabel
        // 
        this.MaxTimeLabel.AutoSize = true;
        this.MaxTimeLabel.Location = new System.Drawing.Point(146, 158);
        this.MaxTimeLabel.Name = "MaxTimeLabel";
        this.MaxTimeLabel.Size = new System.Drawing.Size(26, 13);
        this.MaxTimeLabel.TabIndex = 12;
        this.MaxTimeLabel.Text = "max";
        // 
        // ShuffleTriggerListBox
        // 
        this.ShuffleTriggerListBox.CheckOnClick = true;
        this.ShuffleTriggerListBox.FormattingEnabled = true;
        this.ShuffleTriggerListBox.Items.AddRange(new object[] {
            "t1",
            "t2",
            "t3"});
        this.ShuffleTriggerListBox.Location = new System.Drawing.Point(12, 182);
        this.ShuffleTriggerListBox.Name = "ShuffleTriggerListBox";
        this.ShuffleTriggerListBox.Size = new System.Drawing.Size(166, 94);
        this.ShuffleTriggerListBox.TabIndex = 13;
        // 
        // SeedTextBox
        // 
        this.SeedTextBox.Location = new System.Drawing.Point(12, 282);
        this.SeedTextBox.MaxLength = 16;
        this.SeedTextBox.Name = "SeedTextBox";
        this.SeedTextBox.Size = new System.Drawing.Size(128, 20);
        this.SeedTextBox.TabIndex = 14;
        this.SeedTextBox.WordWrap = false;
        // 
        // SeedTextBoxLabel
        // 
        this.SeedTextBoxLabel.AutoSize = true;
        this.SeedTextBoxLabel.Location = new System.Drawing.Point(148, 285);
        this.SeedTextBoxLabel.Name = "SeedTextBoxLabel";
        this.SeedTextBoxLabel.Size = new System.Drawing.Size(30, 13);
        this.SeedTextBoxLabel.TabIndex = 15;
        this.SeedTextBoxLabel.Text = "seed";
        // 
        // MiksadoToolForm
        // 
        this.ClientSize = new System.Drawing.Size(584, 361);
        this.Controls.Add(this.SeedTextBoxLabel);
        this.Controls.Add(this.SeedTextBox);
        this.Controls.Add(this.ShuffleTriggerListBox);
        this.Controls.Add(this.MaxTimeLabel);
        this.Controls.Add(this.MinTimeLabel);
        this.Controls.Add(this.MaxTimeUpDown);
        this.Controls.Add(this.MinTimeUpDown);
        this.Controls.Add(this.DebugLogCheckBox);
        this.Controls.Add(this.AlgorithmLabel);
        this.Controls.Add(this.RNGLabel);
        this.Controls.Add(this.AlgorithmComboBox);
        this.Controls.Add(this.RngComboBox);
        this.Controls.Add(this.ResumeButton);
        this.Controls.Add(this.StartButton);
        this.Controls.Add(this.ForceShuffleButton);
        this.Controls.Add(this.ConsoleMultiline);
        this.Name = "MiksadoToolForm";
        this.Load += new System.EventHandler(this.MiksadoToolForm_Load);
        ((System.ComponentModel.ISupportInitialize)(this.MinTimeUpDown)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.MaxTimeUpDown)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    private void MiksadoToolForm_Load(object sender, System.EventArgs e)
    {

    }

    private void StartButton_Click(object sender, System.EventArgs e)
    {
        foreach (string file in Directory.GetFiles(MiksadoStatePath))
        {
            File.Delete(file);
        }
        foreach (string dir in Directory.GetDirectories(MiksadoStatePath))
        {
            Directory.Delete(dir, true);
        }

        LogConsoleDebug("start button clicked");
        BeginSession();
    }

    private void ResumeButton_Click(object sender, System.EventArgs e)
    {
        LogConsoleDebug("resume button clicked");
        BeginSession();
    }

    private void BeginSession()
    {
        if (ActiveShuffleTriggers.Length == 0)
        {
            LogConsoleInfo("no active shuffle triggers, cannot start session");
            return;
        }

        IsActiveSession = true;

        if (ActiveShuffleTriggers.Contains(ShuffleTrigger.Timer) && MiksadoTimer == null)
        {
            LogConsoleDebug("initializing timer trigger");

            MiksadoTimer = new System.Timers.Timer(100);
            MiksadoTimer.Start();
            MiksadoTimer.Elapsed += MiksadoTimer_Elapsed;
        }

        string seedText = SeedTextBox.Text.Trim();

        switch (CurrentRngModel)
        {
            case RandomNumberGeneratorModel.System:
                if (seedText.Length > 0)
                {
                    int seedHash = seedText.GetHashCode();
                    SystemRng = new System.Random(seedHash);
                    LogConsoleDebug($"system RNG initialized with seed: {seedText} and hash {seedHash}");
                }
                else
                {
                    SystemRng = new System.Random();
                    LogConsoleDebug("system RNG initialized without seed");
                }
                break;
            case RandomNumberGeneratorModel.SM64:
            // Implement SM64 RNG initialization here
            case RandomNumberGeneratorModel.FinalFantasy1:
            // Implement Final Fantasy I RNG initialization here
            case RandomNumberGeneratorModel.Doom:
            // Implement Doom RNG initialization here
            default:
                LogConsoleInfo("RNG error, default system RNG");
                SystemRng = new System.Random();
                CurrentRngModel = RandomNumberGeneratorModel.System;
                break;
        }

        StartButton.Enabled = false;
        ResumeButton.Enabled = false;
        MinTimeUpDown.Enabled = false;
        MaxTimeUpDown.Enabled = false;
        ShuffleTriggerListBox.Enabled = false;
        RngComboBox.Enabled = false;
        AlgorithmComboBox.Enabled = false;
        ForceShuffleButton.Enabled = AllGames.Length > 1;

        if (IsActiveSession)
        {
            ShouldShuffle = true;
        }
    }

    private void ForceShuffleButton_Click(object sender, System.EventArgs e)
    {
        LogConsoleDebug("shuffle button clicked");
        ShouldShuffle = true;
    }

    private void MiksadoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (ActiveShuffleTriggers.Contains(ShuffleTrigger.Timer) && !IsShuffling)
        {
            LogConsoleDebug("timer elapsed, set shuffle");
            MiksadoTimer.Stop();
            ShouldShuffle = true;
        }
    }
}