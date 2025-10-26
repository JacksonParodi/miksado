using BizHawk.Client.Common;
using Miksado.Misc;
using Miksado.Plugin.Shuffler.Random;
using Miksado.Plugin.Shuffler.Shuffle;
using Miksado.Plugin.Shuffler.Trigger;
using Miksado.Twitch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib.Api.Helix.Models.Polls.CreatePoll;
using TwitchLib.EventSub.Core.EventArgs.Channel;
using TwitchLib.EventSub.Core.Models.Polls;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using MConstant = Miksado.Misc.Constant;

namespace Miksado.Plugin.Shuffler
{
    internal class ShufflerPlugin : MiksadoPlugin
    {
        private enum ShufflerState
        {
            Inactive,
            Active,
            Paused,
        }

        public static readonly IRandomNumberGenerator DefaultRandomNumberGenerator = new MajoraRng();
        public static readonly IShuffleAlgorithm DefaultShuffleAlgorithm = new NoImmediateRepeat();
        public static readonly ShuffleTrigger[] DefaultShuffleTriggers = [ShuffleTrigger.Timer];

        public ShufflerUserControl UserControl => (ShufflerUserControl)BaseUserControl;
        public ShufflerConfig ShufflerConfig => (ShufflerConfig)PluginConfig;
        private Dictionary<string, ShufflerGameInfo> _gameInfoMap = [];
        private Dictionary<string, ShufflerGameInfo> GameInfoMap
        {
            get => _gameInfoMap;
            set => _gameInfoMap = value;
        }

        private ShufflerState _state;
        private ShufflerState GetState() => _state;
        private void SetState(ShufflerState newState) => _state = newState;
        private bool IsActiveSession() => _state == ShufflerState.Active;
        private bool IsInactive() => _state == ShufflerState.Inactive;
        private bool IsPausedSession() => _state == ShufflerState.Paused;

        private IRandomNumberGenerator CurrentRng;
        private IShuffleAlgorithm CurrentShuffleAlgorithm;
        private ShuffleTrigger[] CurrentShuffleTriggers;
        private bool ShouldShuffle = false;
        private bool IsShuffling = false;
        private string? CurrentGamePath = null;
        private System.Timers.Timer? ShufflerTimer;
        private DateTime lastTimerTargetTime;
        private DateTime nextTimerTargetTime;
        private DateTime lastShuffleTime;
        private readonly Dictionary<CheckBox, ComboBox> PollOptionsMap;
        private string? ActivePollId = null;
        private string? ForcedNextGamePath = null;
        private static readonly string RandomPollGameSlug = "random";
        // keys: choice IDs, values: game full paths w/ extensions
        private Dictionary<string, string>? ActivePollMap = null;

        public ShufflerPlugin(
            Logger.Logger logger,
            ApiContainer APIs,
            TwitchClient TwitchClient,
            PluginConfig? pluginConfig
            ) : base(logger, APIs, TwitchClient, pluginConfig)
        {
            BaseUserControl = new ShufflerUserControl();
            SetState(ShufflerState.Inactive);
            PluginName = "game shuffler";

            switch (pluginConfig == null)
            {
                case true:
                    // if no provided map from config, build a new one
                    PluginConfig = new ShufflerConfig();

                    // load games and states
                    string[] rawGameFiles = Directory.GetFiles(MConstant.MGameDirPath, "*", SearchOption.AllDirectories);
                    string[] allGames = [.. rawGameFiles.Where(filePath => !MConstant.InvalidGameExtensions.Contains(Path.GetExtension(filePath).ToLower()))];
                    Logger.Info($"found {allGames.Length} valid game files");

                    foreach (string gamePath in allGames)
                    {
                        string gameFileName = Path.GetFileNameWithoutExtension(gamePath);
                        Logger.Debug($"game file: {gameFileName}");
                    }

                    string[] rawStateFiles = Directory.GetFiles(MConstant.MStateDirPath, "*", SearchOption.AllDirectories);
                    string[] allStates = [.. rawStateFiles.Where(filePath => Path.GetFileName(filePath).StartsWith(MConstant.SaveStatePrefix))];
                    Logger.Debug($"found {allStates.Length} valid state files");

                    GameInfoMap.Clear();
                    foreach (string gamePath in allGames)
                    {
                        string gameFileName = Path.GetFileNameWithoutExtension(gamePath);
                        string? statePathForThisGame = null;
                        foreach (string path in allStates)
                        {
                            string stateFileName = Path.GetFileNameWithoutExtension(path);

                            if (stateFileName.StartsWith(MConstant.SaveStatePrefix) && stateFileName.EndsWith(gameFileName))
                            {
                                statePathForThisGame = path;
                                break;
                            }
                        }
                        ShufflerGameInfo info = new()
                        {
                            GamePath = gamePath,
                        };
                        if (statePathForThisGame != null)
                        {
                            info.StatePath = statePathForThisGame;
                        }
                        GameInfoMap[Path.GetFileNameWithoutExtension(gamePath)] = info;
                    }

                    Logger.Info($"mapped {GameInfoMap.Count} game files");
                    FirePluginConfigChanged();
                    break;

                case false:
                    PluginConfig = pluginConfig;
                    if (pluginConfig is ShufflerConfig config)
                    {
                        GameInfoMap.Clear();
                        GameInfoMap = config.GameInfoMap;
                        // maybe validate the provided map?
                    }
                    break;
            }

            // setup rng
            UserControl.RngComboBox.Items.AddRange(RngFactory.GetAvailableRngNames());
            UserControl.RngComboBox.SelectedIndexChanged += (s, e) =>
            {
                string selectedRngName = UserControl.RngComboBox.SelectedItem.ToString();
                CurrentRng = RngFactory.CreateRng(selectedRngName);
                Logger.Info($"RNG set to {CurrentRng}");
            };
            UserControl.RngComboBox.SelectedIndex = 0;

            if (CurrentRng == null)
            {
                CurrentRng = DefaultRandomNumberGenerator;
                Logger.Info($"RNG set to default {CurrentRng}");
            }

            // setup algorithm
            UserControl.AlgorithmComboBox.Items.AddRange(ShuffleAlgorithmFactory.GetAvailableShuffleAlgorithmNames());
            UserControl.AlgorithmComboBox.SelectedIndexChanged += (s, e) =>
            {
                string selectedAlgorithmName = UserControl.AlgorithmComboBox.SelectedItem.ToString();
                CurrentShuffleAlgorithm = ShuffleAlgorithmFactory.CreateShuffleAlgorithm(selectedAlgorithmName);
                Logger.Info($"shuffle algorithm set to {CurrentShuffleAlgorithm}");
            };
            UserControl.AlgorithmComboBox.SelectedIndex = 0;

            if (CurrentShuffleAlgorithm == null)
            {
                CurrentShuffleAlgorithm = DefaultShuffleAlgorithm;
                Logger.Info($"shuffle algorithm set to default {CurrentShuffleAlgorithm}");
            }

            // setup shuffle trigger settings
            CurrentShuffleTriggers = [];
            UserControl.FileTouchEnableCheckBox.CheckedChanged += (s, e) =>
            {
                switch (UserControl.FileTouchEnableCheckBox.Checked)
                {
                    case true:
                        AddShuffleTrigger(ShuffleTrigger.File);
                        break;
                    case false:
                        RemoveShuffleTrigger(ShuffleTrigger.File);
                        break;
                }
            };

            UserControl.TimerEnableCheckBox.CheckedChanged += (s, e) =>
            {
                switch (UserControl.TimerEnableCheckBox.Checked)
                {
                    case true:
                        AddShuffleTrigger(ShuffleTrigger.Timer);
                        break;
                    case false:
                        RemoveShuffleTrigger(ShuffleTrigger.Timer);
                        break;
                }
            };

            UserControl.TwitchSubEnableCheckBox.CheckedChanged += (s, e) =>
            {
                switch (UserControl.TwitchSubEnableCheckBox.Checked)
                {
                    case true:
                        // probably want to check authorization at some point here
                        AddShuffleTrigger(ShuffleTrigger.TwitchSub);
                        break;
                    case false:
                        RemoveShuffleTrigger(ShuffleTrigger.TwitchSub);
                        break;
                }
            };

            UserControl.TwitchBitsEnableCheckBox.CheckedChanged += (s, e) =>
            {
                switch (UserControl.TwitchBitsEnableCheckBox.Checked)
                {
                    case true:
                        // probably want to check authorization at some point here
                        AddShuffleTrigger(ShuffleTrigger.TwitchBits);
                        break;
                    case false:
                        RemoveShuffleTrigger(ShuffleTrigger.TwitchBits);
                        break;
                }
            };

            UserControl.TwitchRedemptionEnableCheckBox.CheckedChanged += (s, e) =>
            {
                switch (UserControl.TwitchRedemptionEnableCheckBox.Checked)
                {
                    case true:
                        // probably want to check authorization at some point here
                        AddShuffleTrigger(ShuffleTrigger.TwitchRedemption);
                        break;
                    case false:
                        RemoveShuffleTrigger(ShuffleTrigger.TwitchRedemption);
                        break;
                }
            };

            UserControl.TwitchChatCommandCheckBox.CheckedChanged += (s, e) =>
            {
                switch (UserControl.TwitchChatCommandCheckBox.Checked)
                {
                    case true:
                        // probably want to check authorization at some point here
                        AddShuffleTrigger(ShuffleTrigger.TwitchChatCommand);
                        break;
                    case false:
                        RemoveShuffleTrigger(ShuffleTrigger.TwitchChatCommand);
                        break;
                }
            };

            UserControl.ShowProgressBarCheckBox.CheckedChanged += (s, e) =>
            {
                if (UserControl.ShowProgressBarCheckBox.Checked)
                {
                    if (UserControl.TimerProgressBar != null)
                    {
                        UserControl.TimerProgressBar.Visible = true;
                    }
                }
                else
                {
                    if (UserControl.TimerProgressBar != null)
                    {
                        UserControl.TimerProgressBar.Visible = false;
                    }
                }
            };
            UserControl.TimerProgressBar.Visible = UserControl.ShowProgressBarCheckBox.Checked;

            // basic validate game files
            if (GameInfoMap.Count == 0)
            {
                Logger.Info("no valid game files found, tool will not function");
                UserControl.StartNewButton.Enabled = false;
                UserControl.ResumeButton.Enabled = false;
                UserControl.ForceShuffleButton.Enabled = false;
            }

            if (GameInfoMap.Count == 1)
            {
                Logger.Info("only one valid game file found, shuffling will have no effect");
            }

            UserControl.CooldownUpDown.Value = MConstant.DefaultCooldown;

            UserControl.ChannelPointVotingEnableCheckBox.CheckedChanged += (s, e) =>
            {
                UserControl.ChannelPointVoteCostUpDown.Enabled = UserControl.ChannelPointVotingEnableCheckBox.Checked;
            };

            UserControl.StartNewButton.Click += OnStartNewButtonClick;
            UserControl.ResumeButton.Click += OnResumeButtonClick;
            UserControl.PauseUnpauseButton.Click += OnPauseUnpauseButtonClick;
            UserControl.ForceShuffleButton.Click += OnForceShuffleButtonClick;
            UserControl.GameFinishButton.Click += OnGameFinishButtonClick;
            UserControl.PollSendButton.Click += OnPollSendButtonClick;
            UserControl.PollEndButton.Click += OnPollEndButtonClick;
            UserControl.PollClearButton.Click += OnPollClearButtonClick;

            UserControl.PollTitleTextBox.Text = "next game?";
            UserControl.ChannelPointVoteCostUpDown.Value = 100;

            PollOptionsMap = new()
            {
                { UserControl.PollOption1EnableCheckBox, UserControl.PollOption1ComboBox },
                { UserControl.PollOption2EnableCheckBox, UserControl.PollOption2ComboBox },
                { UserControl.PollOption3EnableCheckBox, UserControl.PollOption3ComboBox },
                { UserControl.PollOption4EnableCheckBox, UserControl.PollOption4ComboBox },
                { UserControl.PollOption5EnableCheckBox, UserControl.PollOption5ComboBox },
            };

            foreach (var kv in PollOptionsMap)
            {
                CheckBox checkBox = kv.Key;
                ComboBox comboBox = kv.Value;

                comboBox.BeginUpdate();

                checkBox.CheckedChanged += (s, e) =>
                {
                    comboBox.Enabled = checkBox.Checked;
                    UpdateUI();
                };

                comboBox.Items.Clear();
                comboBox.Items.AddRange([.. GameInfoMap.Values.Select(info => Path.GetFileNameWithoutExtension(info.GamePath))]);
                comboBox.Items.Add(RandomPollGameSlug);
                comboBox.Enabled = checkBox.Checked;

                checkBox.Checked = false;

                comboBox.EndUpdate();
            }

            UpdateUI();
        }

        private void ShuffleGame()
        {
            if (IsShuffling)
            {
                Logger.Info("already shuffling");
                return;
            }

            if (lastShuffleTime + TimeSpan.FromSeconds((double)UserControl.CooldownUpDown.Value) > DateTime.Now)
            {
                Logger.Info("cooldown active, skipping shuffle");
                return;
            }

            if (IsAllGamesCompleted())
            {
                Logger.Info("all games completed, cannot shuffle");
                SetState(ShufflerState.Inactive);
                UpdateUI();
                return;
            }

            try
            {
                IsShuffling = true;

                // if we have a current playing game, save the state for it
                if (APIs.Emulation.GetGameInfo != null && CurrentGamePath != null)
                {
                    string newStatePath = GetSaveStatePath(CurrentGamePath);
                    Logger.Debug($"saving state to: {newStatePath}");
                    APIs.SaveState.Save(newStatePath);
                    GameInfoMap[Path.GetFileNameWithoutExtension(CurrentGamePath)].StatePath = newStatePath;
                }

                bool shuffleSuccessful = false;
                string nextGamePath = "";
                string[] availableGamePaths = [.. GameInfoMap.Values.Where(info => !info.IsCompleted).Select(info => info.GamePath)];

                if (availableGamePaths.Length == 0)
                {
                    Logger.Info("no available games to shuffle to for some reason");
                    SetState(ShufflerState.Inactive);
                    UpdateUI();
                    return;
                }

                while (!shuffleSuccessful)
                {
                    if (ForcedNextGamePath != null)
                    {
                        nextGamePath = ForcedNextGamePath;
                        Logger.Debug($"forcing next game path: {nextGamePath}");
                    }
                    else
                    {
                        nextGamePath = CurrentShuffleAlgorithm.NextGamePath(CurrentRng, availableGamePaths, CurrentGamePath);
                    }

                    Logger.Debug($"next game path: {nextGamePath}");
                    shuffleSuccessful = APIs.EmuClient.OpenRom(nextGamePath);

                    if (!shuffleSuccessful)
                    {
                        GameInfoMap = GameInfoMap.Where(kv => kv.Value.GamePath != nextGamePath).ToDictionary(kv => kv.Key, kv => kv.Value);
                        Logger.Error($"failed to load game: {nextGamePath}, removing from GameInfoMap");
                    }
                }

                lastShuffleTime = DateTime.Now;
                GameInfoMap[Path.GetFileNameWithoutExtension(nextGamePath)].LastPlayed = lastShuffleTime;
                CurrentGamePath = nextGamePath;
                Logger.Debug($"loaded new game: {Path.GetFileNameWithoutExtension(CurrentGamePath)}");

                if (GameInfoMap[Path.GetFileNameWithoutExtension(CurrentGamePath)].StatePath == GetSaveStatePath(CurrentGamePath))
                {
                    string? saveStatePath = GameInfoMap[Path.GetFileNameWithoutExtension(CurrentGamePath)].StatePath;
                    if (saveStatePath != null)
                    {
                        Logger.Debug($"loading state: {Path.GetFileName(saveStatePath)}");
                        bool _loadSaveStateSuccessful = APIs.SaveState.Load(saveStatePath);
                    }
                }
                else
                {
                    Logger.Debug("no save state found for this game, starting fresh");
                }

                UpdateUI();
            }
            catch (System.Exception ex)
            {
                Logger.Info($"error shuffling game: {ex.Message}");
                throw new Exception("error shuffling game", ex);
            }
            finally
            {
                if (CurrentShuffleTriggers.Contains(ShuffleTrigger.File))
                {
                    // delete the trigger file
                }

                if (CurrentShuffleTriggers.Contains(ShuffleTrigger.Timer))
                {
                    int minSeconds = (int)UserControl.TimerMinTimeUpDown.Value;
                    int maxSeconds = (int)UserControl.TimerMaxTimeUpDown.Value;
                    int r = CurrentRng.Next();
                    int nextInterval = (r % (maxSeconds - minSeconds + 1) + minSeconds) * 1000;
                    if (nextInterval < 0)
                    {
                        nextInterval = -1 * nextInterval;
                    }

                    lastTimerTargetTime = DateTime.Now;
                    nextTimerTargetTime = lastTimerTargetTime.AddMilliseconds(nextInterval);
                    Logger.Debug($"next shuffle in {nextInterval / 1000} seconds");

                    if (ShufflerTimer != null)
                    {
                        ShufflerTimer.Interval = nextInterval;
                        ShufflerTimer.Start();
                    }
                    else
                    {
                        Logger.Info("timer trigger enabled but timer not initialized");
                    }
                }
            }

            ForcedNextGamePath = null;
            IsShuffling = false;
            SaveShufflerConfig();
        }

        private void BeginSession()
        {
            SetState(ShufflerState.Active);

            Logger.Debug($"starting session with rng {CurrentRng.GetType().Name} and algorithm {CurrentShuffleAlgorithm.GetType()}");

            if (CurrentShuffleTriggers.Length == 0)
            {
                Logger.Info("no active shuffle triggers set, use force button");
            }
            else
            {
                Logger.Debug($"triggers: {CurrentShuffleTriggers}");
            }

            if (CurrentShuffleTriggers.Contains(ShuffleTrigger.Timer))
            {
                Logger.Debug("initializing timer");

                // this doesn't handle if the session was paused then unpaused
                // I might want to save the remaining time instead
                // this is fine for now

                if (ShufflerTimer != null)
                {
                    ShufflerTimer.Stop();
                    ShufflerTimer.Dispose();
                    ShufflerTimer = null;
                }

                ShufflerTimer = new System.Timers.Timer(100);
                ShufflerTimer.Start();
                ShufflerTimer.Elapsed += OnMiksadoTimerElapsed;
            }

            string seedText = UserControl.SeedTextBox.Text.Trim();
            if (seedText.Length > 0)
            {
                int seedHash = seedText.GetHashCode();
                Logger.Debug($"setting rng seed from text: {seedText} ({seedHash})");
                CurrentRng.SetSeed(seedHash);
            }

            UpdateUI();
            ShouldShuffle = true;
        }

        public override void UpdateUI()
        {
            UserControl.StartNewButton.Enabled = !IsActiveSession() && Enabled;

            bool isAtLeastOneSaveState = GameInfoMap.Values.Any(info => info.StatePath != null && File.Exists(info.StatePath));

            if (GameInfoMap.Count > 0 && isAtLeastOneSaveState && !IsActiveSession() && Enabled)
            {
                UserControl.ResumeButton.Enabled = true;
            }
            else
            {
                UserControl.ResumeButton.Enabled = false;
            }

            UserControl.ForceShuffleButton.Enabled = GameInfoMap.Count > 1 && IsActiveSession() && Enabled;

            UserControl.PauseUnpauseButton.Enabled = !IsInactive() && Enabled;
            if (IsActiveSession())
            {
                UserControl.PauseUnpauseButton.Text = "pause";
            }
            if (IsPausedSession())
            {
                UserControl.PauseUnpauseButton.Text = "unpause";
            }
            if (IsInactive())
            {
                UserControl.PauseUnpauseButton.Text = "...";
            }

            UserControl.GameFinishButton.Enabled = IsActiveSession() && Enabled && !IsAllGamesCompleted();

            UserControl.RngComboBox.Enabled = !IsActiveSession();
            UserControl.AlgorithmComboBox.Enabled = !IsActiveSession();
            UserControl.SeedTextBox.Enabled = !IsActiveSession();
            UserControl.CooldownUpDown.Enabled = !IsActiveSession();

            UserControl.TimerEnableCheckBox.Enabled = !IsActiveSession();
            UserControl.TimerMinTimeUpDown.Enabled = !IsActiveSession();
            UserControl.TimerMaxTimeUpDown.Enabled = !IsActiveSession();

            // file touch not yet implemented
            //UserControl.FileTouchEnableCheckBox.Enabled = !IsActiveSession();
            //UserControl.FileTouchDialogButton.Enabled = !IsActiveSession();
            UserControl.FileTouchEnableCheckBox.Enabled = false;
            UserControl.FileTouchDialogButton.Enabled = false;

            UserControl.TwitchSubEnableCheckBox.Enabled = !IsActiveSession();
            UserControl.TwitchSubTierComboBox.Enabled = !IsActiveSession();

            // UI logic for poll active/inactive
            int enabledPollOptions = PollOptionsMap.Keys.Count(cb => cb.Checked);
            switch (enabledPollOptions > 1)
            {
                case true:
                    if (!TwitchClient.IsConnected() || ActivePollId != null)
                    {
                        UserControl.PollSendButton.Enabled = false;
                    }
                    else
                    {
                        UserControl.PollSendButton.Enabled = IsActiveSession();
                    }

                    break;
                case false:
                    UserControl.PollSendButton.Enabled = false;
                    break;
            }

            if (ActivePollId != null)
            {
                UserControl.PollEndButton.Enabled = IsActiveSession();
                UserControl.PollClearButton.Enabled = IsActiveSession();
            }
            else
            {
                UserControl.PollEndButton.Enabled = false;
                UserControl.PollClearButton.Enabled = false;
            }

            UserControl.TwitchBitsEnableCheckBox.Enabled = !IsActiveSession();
            UserControl.TwitchBitsMinimumUpDown.Enabled = !IsActiveSession();

            UserControl.TwitchChatCommandCheckBox.Enabled = !IsActiveSession();
            UserControl.TwitchChatCommandTextBox.Enabled = !IsActiveSession();
            UserControl.TwitchCommandModEnableCheckBox.Enabled = !IsActiveSession();

            UserControl.TwitchRedemptionEnableCheckBox.Enabled = !IsActiveSession();
            UserControl.TwitchRedemptionTitleTextBox.Enabled = !IsActiveSession();
        }

        private void AddShuffleTrigger(ShuffleTrigger trigger)
        {
            if (!CurrentShuffleTriggers.Contains(trigger))
            {
                CurrentShuffleTriggers = [.. CurrentShuffleTriggers, trigger];
                Logger.Info($"added shuffle trigger: {trigger}");
            }
            else
            {
                Logger.Info($"shuffle trigger already present: {trigger}");
            }

            Logger.Debug($"current shuffle triggers: {string.Join(", ", CurrentShuffleTriggers)}");
        }

        private void RemoveShuffleTrigger(ShuffleTrigger trigger)
        {
            if (CurrentShuffleTriggers.Contains(trigger))
            {
                CurrentShuffleTriggers = [.. CurrentShuffleTriggers.Where(t => t != trigger)];
                Logger.Info($"removed shuffle trigger: {trigger}");
            }
            else
            {
                Logger.Info($"shuffle trigger not present: {trigger}");
            }

            Logger.Debug($"current shuffle triggers: {string.Join(", ", CurrentShuffleTriggers)}");
        }

        private string GetSaveStatePath(string gamePath)
        {
            return Path.Combine(MConstant.MStateDirPath, GetSaveStateFileName(gamePath));
        }

        private string GetSaveStateFileName(string gamePath)
        {
            string gameFileName = Path.GetFileNameWithoutExtension(gamePath);
            return $"{MConstant.SaveStatePrefix}{gameFileName}.state";
        }

        private bool IsAllGamesCompleted()
        {
            return GameInfoMap.Values.All(info => info.IsCompleted);
        }

        private void SaveShufflerConfig()
        {
            ShufflerConfig.GameInfoMap = GameInfoMap;
            FirePluginConfigChanged();
        }

        public override void OnRestart()
        {
            Logger.Debug("shuffler plugin restarted");
        }

        public override void OnUpdateAfter()
        {
            if (!Initialized)
            {
                Initialized = true;
                Logger.Debug("shuffler plugin initialized");
                FirePluginConfigChanged();
            }

            if (
                CurrentShuffleTriggers.Contains(ShuffleTrigger.Timer) &&
                ShufflerTimer != null &&
                ShufflerTimer.Enabled &&
                UserControl.TimerProgressBar != null
                )
            {
                double totalInterval = (nextTimerTargetTime - lastTimerTargetTime).TotalMilliseconds;
                double elapsedInterval = (DateTime.Now - lastTimerTargetTime).TotalMilliseconds;
                double progress = Math.Max(0.0, Math.Min(1.0, elapsedInterval / totalInterval));
                int progressPercent = (int)(progress * 100);
                UserControl.TimerProgressBar.Value = progressPercent;
            }

            if (ShouldShuffle & !IsShuffling & IsActiveSession() && Enabled)
            {
                Logger.Debug("shuffling game");
                ShouldShuffle = false;
                ShuffleGame();
            }
        }

        private void OnStartNewButtonClick(object sender, System.EventArgs e)
        {
            foreach (string file in Directory.GetFiles(MConstant.MStateDirPath))
            {
                File.Delete(file);
            }
            foreach (string dir in Directory.GetDirectories(MConstant.MStateDirPath))
            {
                Directory.Delete(dir, true);
            }

            foreach (var key in GameInfoMap.Keys.ToList())
            {
                GameInfoMap[key].IsCompleted = false;
                GameInfoMap[key].LastPlayed = DateTime.MinValue;
                GameInfoMap[key].StatePath = null;
            }

            Logger.Debug("start button clicked");
            BeginSession();
        }

        private void OnResumeButtonClick(object sender, System.EventArgs e)
        {
            Logger.Debug("resume button clicked");
            BeginSession();
        }

        private void OnPauseUnpauseButtonClick(object sender, System.EventArgs e)
        {
            switch (GetState())
            {
                case ShufflerState.Active:
                    Logger.Debug("pause button clicked");
                    SetState(ShufflerState.Paused);
                    ShufflerTimer?.Stop();
                    break;
                case ShufflerState.Paused:
                    Logger.Debug("unpause button clicked");
                    SetState(ShufflerState.Active);
                    if (ShufflerTimer != null)
                    {
                        ShufflerTimer.Start();
                        lastTimerTargetTime = DateTime.Now;
                        nextTimerTargetTime = lastTimerTargetTime.AddMilliseconds(ShufflerTimer.Interval);
                    }
                    break;
                case ShufflerState.Inactive:
                    Logger.Debug("pause/unpause button clicked but session inactive");
                    break;
            }

            UpdateUI();
        }

        private void OnForceShuffleButtonClick(object sender, System.EventArgs e)
        {
            Logger.Debug("shuffle button clicked");
            ShouldShuffle = true;
        }

        private void OnMiksadoTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (CurrentShuffleTriggers.Contains(ShuffleTrigger.Timer) && !IsShuffling && ShufflerTimer != null)
            {
                Logger.Debug("timer elapsed, set shuffle");
                ShufflerTimer.Stop();
                if (IsActiveSession() && Enabled)
                {
                    ShouldShuffle = true;
                }
            }
        }

        async public override Task OnChannelChatMessage(ChannelChatMessageArgs e)
        {
            if (!CurrentShuffleTriggers.Contains(ShuffleTrigger.TwitchChatCommand) || !IsActiveSession() || !Enabled)
            {
                return;
            }

            bool isMod = e.Payload.Event.IsModerator;
            bool isBroadcaster = e.Payload.Event.IsBroadcaster;
            bool modEnabled = UserControl.TwitchCommandModEnableCheckBox.Checked;
            string targetCommand = UserControl.TwitchChatCommandTextBox.Text.Trim();
            string chatText = e.Payload.Event.Message.Text.Trim();
            string displayName = e.Payload.Event.ChatterUserName;

            Logger.Debug($"{displayName}: {chatText}");

            if (chatText == targetCommand)
            {
                if (isBroadcaster)
                {
                    Logger.Info($"broadcaster {displayName} used command {targetCommand}, shuffling!");
                    ShouldShuffle = true;
                }
                else if (modEnabled && isMod)
                {
                    Logger.Info($"mod {displayName} used command {targetCommand}, shuffling!");
                    ShouldShuffle = true;
                }
            }

            await Task.CompletedTask;
        }

        async public override Task OnChannelChatNotification(ChannelChatNotificationArgs e)
        {
            Logger.Debug("OnChannelChatNotification called");

            if (!CurrentShuffleTriggers.Contains(ShuffleTrigger.TwitchSub) || !IsActiveSession() || !Enabled)
            {
                return;
            }

            Logger.Debug($"Processing chat notification of type: {e.Payload.Event.NoticeType}");

            SubTier? targetSubTier = null;
            switch (UserControl.TwitchSubTierComboBox.SelectedIndex)
            {
                case 0:
                    targetSubTier = SubTier.Tier1;
                    break;
                case 1:
                    targetSubTier = SubTier.Tier2;
                    break;
                case 2:
                    targetSubTier = SubTier.Tier3;
                    break;
            }

            Logger.Debug($"Target sub tier set to: {targetSubTier}");

            if (targetSubTier == null)
            {
                Logger.Error("no target sub tier selected, cannot process subscription notification");
                return;
            }

            ChannelChatNotification eventData = e.Payload.Event;
            SubTier? subTier = null;
            string? subTierString = null;

            Logger.Debug("Checking notification type to extract sub tier");

            if (eventData.Sub != null)
            {
                subTierString = eventData.Sub.SubTier;
                Logger.Debug($"Found Sub notification with tier: {subTierString}");
            }
            else if (eventData.Resub != null)
            {
                subTierString = eventData.Resub.SubTier;
                Logger.Debug($"Found Resub notification with tier: {subTierString}");
            }
            else if (eventData.SubGift != null)
            {
                subTierString = eventData.SubGift.SubTier;
                Logger.Debug($"Found SubGift notification with tier: {subTierString}");
            }
            else if (eventData.CommunitySubGift != null)
            {
                subTierString = eventData.CommunitySubGift.SubTier;
                Logger.Debug($"Found CommunitySubGift notification with tier: {subTierString}");
            }
            else if (eventData.PrimePaidUpgrade != null)
            {
                subTierString = eventData.PrimePaidUpgrade.SubTier;
                Logger.Debug($"Found PrimePaidUpgrade notification with tier: {subTierString}");
            }
            else
            {
                Logger.Debug("No recognized subscription event type found in notification");
            }
            // TODO: shared chat equivalents to all these

            switch (subTierString == null)
            {
                case true:
                    Logger.Info("could not determine sub tier from notification");
                    return;
                case false:
                    Logger.Debug($"Converting sub tier string '{subTierString}' to SubTier enum");
                    switch (subTierString)
                    {
                        case "1000":
                            subTier = SubTier.Tier1;
                            Logger.Debug("Converted to Tier1");
                            break;
                        case "2000":
                            subTier = SubTier.Tier2;
                            Logger.Debug("Converted to Tier2");
                            break;
                        case "3000":
                            subTier = SubTier.Tier3;
                            Logger.Debug("Converted to Tier3");
                            break;
                        default:
                            Logger.Debug($"Unknown sub tier string: {subTierString}");
                            break;
                    }
                    break;
            }

            Logger.Debug($"chat notification: {eventData.NoticeType} and sub tier: {subTier}");
            Logger.Debug($"Checking if sub tier {subTier} meets target tier {targetSubTier} requirement");

            switch (targetSubTier)
            {
                case SubTier.Tier1:
                    if (subTier == SubTier.Tier1 | subTier == SubTier.Tier2 | subTier == SubTier.Tier3)
                    {
                        Logger.Debug("Sub tier meets Tier1+ requirement, setting ShouldShuffle = true");
                        ShouldShuffle = true;
                    }
                    else
                    {
                        Logger.Debug("Sub tier does not meet Tier1+ requirement");
                    }
                    break;
                case SubTier.Tier2:
                    if (subTier == SubTier.Tier2 | subTier == SubTier.Tier3)
                    {
                        Logger.Debug("Sub tier meets Tier2+ requirement, setting ShouldShuffle = true");
                        ShouldShuffle = true;
                    }
                    else
                    {
                        Logger.Debug("Sub tier does not meet Tier2+ requirement");
                    }
                    break;
                case SubTier.Tier3:
                    if (subTier == SubTier.Tier3)
                    {
                        Logger.Debug("Sub tier meets Tier3 requirement, setting ShouldShuffle = true");
                        ShouldShuffle = true;
                    }
                    else
                    {
                        Logger.Debug("Sub tier does not meet Tier3 requirement");
                    }
                    break;
            }

            Logger.Debug("OnChannelChatNotification processing complete");

            await Task.CompletedTask;
        }

        async public override Task OnRedemptionAdd(ChannelPointsCustomRewardRedemptionArgs e)
        {
            if (!CurrentShuffleTriggers.Contains(ShuffleTrigger.TwitchRedemption) || !IsActiveSession() || !Enabled)
            {
                await Task.CompletedTask;
                return;
            }

            string redemptionUser = e.Payload.Event.UserName;
            string targetRewardTitle = UserControl.TwitchRedemptionTitleTextBox.Text.Trim();
            string redeemedTitle = e.Payload.Event.Reward.Title.Trim();
            string redeemedId = e.Payload.Event.Reward.Id.Trim();
            Logger.Debug($"redemption by {redemptionUser}: {redeemedTitle} ({redeemedId})");

            if (targetRewardTitle.Length > 0 && redeemedTitle.Equals(targetRewardTitle, StringComparison.OrdinalIgnoreCase))
            {
                Logger.Info($"{redemptionUser} redeemed {redeemedTitle}, shuffling!");
                ShouldShuffle = true;
            }
            else
            {
                Logger.Info($"{redemptionUser} redeemed {redeemedTitle}, not the correct reward to trigger shuffle");
            }

            await Task.CompletedTask;
        }

        async public override Task OnBitsUse(ChannelBitsUseArgs e)
        {
            Logger.Debug("OnBitsUse called");

            if (!CurrentShuffleTriggers.Contains(ShuffleTrigger.TwitchBits) || !IsActiveSession() || !Enabled)
            {
                return;
            }

            string bitsUser = e.Payload.Event.UserName;
            int usedAmount = e.Payload.Event.Bits;
            int triggerAmount = (int)UserControl.TwitchBitsMinimumUpDown.Value;

            Logger.Debug($"Processing bits event - User: {bitsUser}, Amount: {usedAmount}, Trigger threshold: {triggerAmount}");

            if (usedAmount >= triggerAmount)
            {
                Logger.Info($"{bitsUser} used {usedAmount} bits (trigger: {triggerAmount}), shuffling!");
                Logger.Debug("Setting ShouldShuffle = true due to bits threshold met");
                ShouldShuffle = true;
            }
            else
            {
                Logger.Info($"{bitsUser} used {usedAmount} bits (trigger: {triggerAmount}), not enough to trigger shuffle");
                Logger.Debug("Bits amount below threshold, no shuffle triggered");
            }

            Logger.Debug("OnBitsUse processing complete");
            await Task.CompletedTask;
        }

        public override async Task OnPollEnd(ChannelPollEndArgs e)
        {
            string completedPollId = e.Payload.Event.Id;
            Logger.Debug($"poll ended with id: {completedPollId}");

            if (!IsActiveSession() || !Enabled)
            {
                await Task.CompletedTask;
                return;
            }

            if (ActivePollMap == null)
            {
                Logger.Debug("no active poll map, cannot process poll end");
                await Task.CompletedTask;
                return;
            }

            PollChoice winningChoice = new()
            {
                Votes = -1,
            };
            foreach (PollChoice choice in e.Payload.Event.Choices)
            {
                if (choice.Votes > winningChoice.Votes)
                {
                    winningChoice = choice;
                }
            }

            Logger.Info($"poll ended, winning choice: {winningChoice.Title} with {winningChoice.Votes} votes");

            string winningChoiceId = winningChoice.Id;
            string winningGamePath = ActivePollMap.ContainsKey(winningChoiceId) ? ActivePollMap[winningChoiceId] : "";

            //string winningGamePath = GameInfoMap.Values
            //    .Where(info => Path.GetFileNameWithoutExtension(info.GamePath) == winningChoice.Title)
            //    .Select(info => info.GamePath)
            //    .FirstOrDefault() ?? "";

            if (winningGamePath != "" && completedPollId == ActivePollId)
            {
                Logger.Info($"shuffling to winning game: {winningChoice.Title}");
                ForcedNextGamePath = winningGamePath;
                ShouldShuffle = true;
                // TODO: add timer to clear ActivePollId after some delay in case of hung or invalid poll
                ActivePollId = null;
            }
            else
            {
                Logger.Info("winning choice does not correspond to a valid game, not shuffling");
            }

            UpdateUI();
            await Task.CompletedTask;
            return;
            //await Task.CompletedTask;
        }

        private void OnPollClearButtonClick(object sender, EventArgs e)
        {
            return;
        }

        private void OnPollEndButtonClick(object sender, EventArgs e)
        {
            return;
        }

        private async void OnPollSendButtonClick(object sender, EventArgs e)
        {
            Choice[] choices = [];

            foreach (var kv in PollOptionsMap)
            {
                CheckBox checkBox = kv.Key;
                ComboBox comboBox = kv.Value;

                if (checkBox.Checked && comboBox.Enabled)
                {
                    Choice thisChoice = new() { Title = comboBox.SelectedItem as string ?? "" };

                    if (
                        thisChoice.Title != "" &&
                        thisChoice.Title.Length > 0 &&
                        !choices.Any((c) => c.Title == thisChoice.Title && thisChoice.Title != RandomPollGameSlug))
                    {
                        choices = [.. choices, thisChoice];
                    }
                }
            }

            if (choices.Any((c) => c.Title == RandomPollGameSlug))
            {
                foreach (Choice c in choices)
                {
                    if (c.Title == RandomPollGameSlug)
                    {
                        List<string> availableGames = [.. GameInfoMap.Values.Where(info => !info.IsCompleted).Select(info => Path.GetFileNameWithoutExtension(info.GamePath))];
                        if (availableGames.Count == 0)
                        {
                            Logger.Info("no available games to add for random poll option");
                            MessageBox.Show("no available games to add for random poll option", "cannot create poll", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int retryMax = 16;
                        int retryCount = 0;
                        bool searching = true;
                        while (searching && retryCount < retryMax)
                        {
                            int r = CurrentRng.Next();
                            string candidateGame = availableGames[r % availableGames.Count];

                            if (!choices.Any((choice) => choice.Title == candidateGame))
                            {
                                c.Title = candidateGame;
                                searching = false;
                            }
                            retryCount++;
                        }

                        if (searching)
                        {
                            Logger.Info("failed to select unique game for random poll option");
                        }
                    }
                }
            }

            if (choices.Any((c) => c.Title == RandomPollGameSlug))
            {
                choices = [.. choices.Where(c => c.Title != RandomPollGameSlug)];
            }

            string[] reqChoicePaths = [];
            int choiceTitleMaxLength = 25;

            foreach (Choice c in choices)
            {
                string fullTitle = c.Title;

                c.Title = Regex.Replace(c.Title, @"\([^)]*\)", "");
                c.Title = Regex.Replace(c.Title, @"\s+", " ").Trim();

                if (c.Title.Length > choiceTitleMaxLength)
                {
                    c.Title = Regex.Replace(c.Title, @"\s+", "");
                }

                if (c.Title.Length > choiceTitleMaxLength)
                {
                    List<string> filterWords = ["and", "the", "of", "in", "a", "to", "for", "on", "with", "at"];
                    filterWords.AddRange(["by", "an", "be", "is", "it", "as", "from", "that", "this"]);

                    foreach (string word in filterWords)
                    {
                        c.Title = Regex.Replace(c.Title, $@"\b{word}\b", "", RegexOptions.IgnoreCase).Trim();
                        if (c.Title.Length <= choiceTitleMaxLength)
                        {
                            break;
                        }
                    }
                }

                if (c.Title.Length > choiceTitleMaxLength)
                {
                    c.Title = c.Title[..choiceTitleMaxLength].Trim();
                }

                reqChoicePaths = [.. reqChoicePaths, GameInfoMap[fullTitle].GamePath];
            }

            if (choices.Length < 2)
            {
                Logger.Info("need at least two poll options to create poll");
                MessageBox.Show("please enable and select at least two poll options", "cannot create poll", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (choices.Length > 5)
            {
                Logger.Info("cannot have more than five poll options");
                MessageBox.Show("cannot have more than five poll options", "cannot create poll", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Debug($"Twitch ID passed: {TwitchClient.TwitchId}");
            Logger.Debug($"Twitch token is null: {TwitchClient.Token == null}");

            Logger.Debug("printing poll choices:");
            foreach (Choice choice in choices)
            {
                Logger.Debug($"- {choice.Title}");
            }

            CreatePollRequest req = new()
            {
                BroadcasterId = TwitchClient.TwitchId,
                Title = UserControl.PollTitleTextBox.Text,
                Choices = choices,
                ChannelPointsVotingEnabled = UserControl.ChannelPointVotingEnableCheckBox.Enabled,
                ChannelPointsPerVote = (int)UserControl.ChannelPointVoteCostUpDown.Value,
                DurationSeconds = (int)UserControl.PollDurationUpDown.Value,
            };

            try
            {
                CreatePollResponse res = await TwitchClient.TwitchApi.Helix.Polls.CreatePollAsync(req, TwitchClient.Token);
                TwitchLib.Api.Helix.Models.Polls.Choice[] resChoices = res.Data[0].Choices;

                ActivePollMap = [];
                for (int i = 0; i < resChoices.Length; i++)
                {
                    TwitchLib.Api.Helix.Models.Polls.Choice rc = resChoices[i];
                    ActivePollMap.Add(rc.Id, reqChoicePaths[i]);
                }

                ActivePollId = res.Data[0].Id;
                Logger.Debug($"created poll with id: {ActivePollId}");
                UpdateUI();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Logger.Debug($"error creating poll: {ex.Message}");
            }
        }

        private void OnGameFinishButtonClick(object sender, EventArgs e)
        {
            if (!IsActiveSession() || !Enabled)
            {
                return;
            }

            GameInfoMap[Path.GetFileNameWithoutExtension(CurrentGamePath)].IsCompleted = true;
            Logger.Info($"marked game {Path.GetFileNameWithoutExtension(CurrentGamePath)} as completed");
            SaveShufflerConfig();

            if (IsAllGamesCompleted())
            {
                ShouldShuffle = false;
                Logger.Info("all games completed!");
                SetState(ShufflerState.Inactive);
                UpdateUI();
            }
            else
            {
                Logger.Info("shuffling to next game");
                ShouldShuffle = true;
            }
        }
    }
}