namespace Miksado;

using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using Miksado.Plugin;
using Miksado.Plugin.Shuffler;
using Miksado.Twitch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.EventSub.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using static Miksado.Constant.Constant;
using LogLevel = Logger.Logger.LogLevel;

// bug: force no debug logs at startup
// todo: more aggressive rom file parsing, to not include non-rom files
// todo: button to complete a game
// todo: rescan folder button
// todo: add socket trigger

[ExternalTool("miksado")]
public sealed class MiksadoToolForm : ToolFormBase, IExternalToolForm
{
    protected override string WindowTitleStatic => "miksado";

    public ApiContainer? MaybeAPIContainer { get; set; }
    private ApiContainer APIs => MaybeAPIContainer!;

    //private IGameInfo? CurrentGameInfo = null;
    //private string? CurrentGamePath = null;
    private readonly Logger.Logger Logger;
    private readonly TwitchClient TwitchClient;
    private List<MiksadoPlugin> Plugins = [];

    public MiksadoToolForm()
    {
        InitializeComponent();
        Logger = new(ConsoleMultiline);
        Logger.Info("miksado initializing...");

        MiksadoVersionLabel.Text = $"miksado v{MajorVersion}.{MinorVersion}.{PatchVersion} by jackson parodi";

        BugReportButton.Click += (s, e) =>
        {
            string message = "i promise i'll eventually make a real bug report system. for now, just message me on Discord: jacksonparodi";
            string caption = "report a bug";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                // Closes the parent form.
                //this.Close();
            }
        };

        TwitchClient = new();
        TwitchClient.EventSubClient.WebsocketConnected += OnWebsocketConnected;
        TwitchClient.EventSubClient.WebsocketDisconnected += OnWebsocketDisconnected;
        TwitchClient.EventSubClient.WebsocketReconnected += OnWebsocketReconnected;
        TwitchClient.EventSubClient.ErrorOccurred += OnErrorOccurred;

        TwitchClient.EventSubClient.ChannelChatMessage += OnChannelChatMessage;
        TwitchClient.EventSubClient.ChannelChatNotification += OnChannelChatNotification;
        TwitchClient.EventSubClient.ChannelPointsCustomRewardRedemptionAdd += OnRedemptionAdd;
        TwitchClient.EventSubClient.ChannelBitsUse += OnBitsUse;
        TwitchClient.EventSubClient.ChannelPollEnd += OnPollEnd;

        string[] paths = [MiksadoPath, MiksadoGamePath, MiksadoStatePath, MiksadoDataPath];
        foreach (string path in paths)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        DebugLogCheckBox.Checked = Logger.CurrentLogLevel == LogLevel.Info;
        DebugLogCheckBox.CheckedChanged += (s, e) =>
        {
            Logger.CurrentLogLevel = DebugLogCheckBox.Checked ? LogLevel.Debug : LogLevel.Info;
            Logger.Debug($"log level set to {Logger.CurrentLogLevel}");
        };

        Logger.Debug("miksado initialized");
        Logger.ScrollToCaret();
    }

    /// <summary>
    /// called when the tool is first opened, and when a new ROM is loaded
    /// </summary>
    public override void Restart()
    {
        if (Plugins.Count == 0)
        {
            //ChatInputPlugin ChatInputPlugin = new(Logger, APIs);
            ShufflerPlugin ShufflerPlugin = new(Logger, APIs);
            Plugins = [ShufflerPlugin];

            foreach (MiksadoPlugin p in Plugins)
            {
                RightDynamicPanel.Controls.Add(p.BaseUserControl);
                p.BaseUserControl.Dock = DockStyle.Fill;
                p.BaseUserControl.Visible = false;
            }

            PluginSelectCheckedListBox.Items.Clear();
            foreach (MiksadoPlugin plugin in Plugins)
            {
                PluginSelectCheckedListBox.Items.Add(plugin.PluginName, false);
            }

            Plugins[1].BaseUserControl.Visible = true;

            PluginSelectCheckedListBox.ItemCheck += (s, e) =>
            {
                foreach (MiksadoPlugin plugin in Plugins)
                {
                    if (Plugins[e.Index] == plugin)
                    {
                        plugin.BaseUserControl.Visible = true;
                    }
                    else
                    {
                        plugin.BaseUserControl.Visible = false;
                    }
                }

                switch (e.NewValue)
                {
                    case CheckState.Checked:
                        Plugins[e.Index].Enabled = true;
                        break;
                    case CheckState.Unchecked:
                        Plugins[e.Index].Enabled = false;
                        break;
                }
                Plugins[e.Index].UpdateUI();

                Logger.Info("active plugins: " + string.Join(", ", Plugins.Where(p => p.Enabled).Select(p => p.PluginName)));
            };
        }

        foreach (MiksadoPlugin plugin in Plugins.Where(p => p.Enabled))
        {
            plugin.OnRestart();
        }
    }

    protected override void UpdateAfter()
    {
        foreach (MiksadoPlugin plugin in Plugins.Where(p => p.Enabled))
        {
            plugin.OnUpdateAfter();
        }
    }

    private void MiksadoVersionLabel_Click(object sender, EventArgs e)
    {
        string[] p = ["jartis", "authorblues", "all of genpo"];
        string message = "many special thanks to the kind people who have helped and supported me:\n";
        message += string.Join("\n", p);
        string caption = "miksado";
        MessageBoxButtons buttons = MessageBoxButtons.OK;
        DialogResult result;

        result = MessageBox.Show(message, caption, buttons);
        if (result == System.Windows.Forms.DialogResult.Yes)
        {
            // Closes the parent form.
            //this.Close();
        }
    }

    async private void TwitchAuthorizeButton_Click(object sender, EventArgs e)
    {
        await TwitchClient.OnAuthorizeButtonClicked(Logger);

        Logger.Debug("token: " + TwitchClient.Token);

        await TwitchClient.ConnectAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Logger.Info($"twitch connection error: {task.Exception?.GetBaseException().Message}");
            }
            else
            {
                Logger.Debug("twitch connected");
            }
        });

        TwitchStatusLabel.Text = "status: ";
        TwitchStatusLabel.Text += TwitchClient.IsConnected() ? "connected" : "disconnected";
    }

    private async Task OnWebsocketConnected(object? sender, WebsocketConnectedArgs e)
    {
        await TwitchClient.OnWebsocketConnected(Logger, sender!, e);

        GetEventSubSubscriptionsResponse res = await TwitchClient.TwitchApi.Helix.EventSub.GetEventSubSubscriptionsAsync();
        foreach (var sub in res.Subscriptions)
        {
            Logger.Debug($"active eventsub subscription: {sub.Type}");
        }
    }

    private async Task OnWebsocketDisconnected(object? sender, WebsocketDisconnectedArgs e)
    {
        TwitchStatusLabel.Text = "status: disconnected";
        await TwitchClient.OnWebsocketDisconnected(sender!, e);
    }

    private async Task OnWebsocketReconnected(object? sender, WebsocketReconnectedArgs e)
    {
        await TwitchClient.OnWebsocketReconnected(sender!, e);
    }

    private async Task OnErrorOccurred(object? sender, ErrorOccuredArgs e)
    {
        TwitchStatusLabel.Text = "status: error";
        await TwitchClient.OnErrorOccurred(sender!, e);
    }

    private async Task OnChannelChatMessage(object? _, ChannelChatMessageArgs e)
    {
        foreach (MiksadoPlugin plugin in Plugins.Where(p => p.Enabled))
        {
            await plugin.OnChannelChatMessage(e);
        }

        await Task.CompletedTask;
    }

    private async Task OnChannelChatNotification(object? _, ChannelChatNotificationArgs e)
    {
        foreach (MiksadoPlugin plugin in Plugins.Where(p => p.Enabled))
        {
            await plugin.OnChannelChatNotification(e);
        }

        await Task.CompletedTask;
    }

    private async Task OnRedemptionAdd(object? _, ChannelPointsCustomRewardRedemptionArgs e)
    {
        foreach (MiksadoPlugin plugin in Plugins.Where(p => p.Enabled))
        {
            await plugin.OnRedemptionAdd(e);
        }
        await Task.CompletedTask;
    }

    private async Task OnBitsUse(object? _, ChannelBitsUseArgs e)
    {
        foreach (MiksadoPlugin plugin in Plugins.Where(p => p.Enabled))
        {
            await plugin.OnBitsUse(e);
        }
    }

    private async Task OnPollEnd(object? sender, ChannelPollEndArgs e)
    {
        foreach (MiksadoPlugin plugin in Plugins.Where(p => p.Enabled))
        {
            await plugin.OnPollEnd(e);
        }
    }

    private void InitializeComponent()
    {
        this.ConsoleMultiline = new System.Windows.Forms.TextBox();
        this.TwitchStatusLabel = new System.Windows.Forms.Label();
        this.FileTouchBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
        this.MiksadoVersionLabel = new System.Windows.Forms.Label();
        this.BugReportButton = new System.Windows.Forms.Button();
        this.DebugLogCheckBox = new System.Windows.Forms.CheckBox();
        this.TwitchAuthorizeButton = new System.Windows.Forms.Button();
        this.LeftSplitContainer = new System.Windows.Forms.SplitContainer();
        this.PluginSelectCheckedListBox = new System.Windows.Forms.CheckedListBox();
        this.LeftSplitLowerFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
        this.LeftSplitOriginalPanel = new System.Windows.Forms.Panel();
        this.RightDynamicPanel = new System.Windows.Forms.Panel();
        ((System.ComponentModel.ISupportInitialize)(this.LeftSplitContainer)).BeginInit();
        this.LeftSplitContainer.Panel1.SuspendLayout();
        this.LeftSplitContainer.Panel2.SuspendLayout();
        this.LeftSplitContainer.SuspendLayout();
        this.LeftSplitLowerFlowLayout.SuspendLayout();
        this.LeftSplitOriginalPanel.SuspendLayout();
        this.SuspendLayout();
        // 
        // ConsoleMultiline
        // 
        this.ConsoleMultiline.BackColor = System.Drawing.SystemColors.ControlText;
        this.ConsoleMultiline.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.ConsoleMultiline.Cursor = System.Windows.Forms.Cursors.Default;
        this.ConsoleMultiline.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.ConsoleMultiline.ForeColor = System.Drawing.SystemColors.Control;
        this.ConsoleMultiline.Location = new System.Drawing.Point(0, 401);
        this.ConsoleMultiline.Margin = new System.Windows.Forms.Padding(0);
        this.ConsoleMultiline.Multiline = true;
        this.ConsoleMultiline.Name = "ConsoleMultiline";
        this.ConsoleMultiline.ReadOnly = true;
        this.ConsoleMultiline.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.ConsoleMultiline.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.ConsoleMultiline.Size = new System.Drawing.Size(784, 160);
        this.ConsoleMultiline.TabIndex = 0;
        this.ConsoleMultiline.Text = "console_line1\r\nconsole_line2\r\nconsole_line3";
        // 
        // TwitchStatusLabel
        // 
        this.TwitchStatusLabel.AutoSize = true;
        this.TwitchStatusLabel.Location = new System.Drawing.Point(3, 152);
        this.TwitchStatusLabel.Name = "TwitchStatusLabel";
        this.TwitchStatusLabel.Size = new System.Drawing.Size(72, 13);
        this.TwitchStatusLabel.TabIndex = 8;
        this.TwitchStatusLabel.Text = "status: NONE";
        // 
        // MiksadoVersionLabel
        // 
        this.MiksadoVersionLabel.AutoSize = true;
        this.MiksadoVersionLabel.Location = new System.Drawing.Point(3, 280);
        this.MiksadoVersionLabel.Name = "MiksadoVersionLabel";
        this.MiksadoVersionLabel.Size = new System.Drawing.Size(58, 13);
        this.MiksadoVersionLabel.TabIndex = 17;
        this.MiksadoVersionLabel.Text = "miksado_v";
        this.MiksadoVersionLabel.Click += new System.EventHandler(this.MiksadoVersionLabel_Click);
        // 
        // BugReportButton
        // 
        this.BugReportButton.Location = new System.Drawing.Point(3, 214);
        this.BugReportButton.Name = "BugReportButton";
        this.BugReportButton.Size = new System.Drawing.Size(120, 40);
        this.BugReportButton.TabIndex = 16;
        this.BugReportButton.Text = "report a bug";
        this.BugReportButton.UseVisualStyleBackColor = true;
        // 
        // DebugLogCheckBox
        // 
        this.DebugLogCheckBox.AutoSize = true;
        this.DebugLogCheckBox.Location = new System.Drawing.Point(3, 260);
        this.DebugLogCheckBox.Name = "DebugLogCheckBox";
        this.DebugLogCheckBox.Size = new System.Drawing.Size(73, 17);
        this.DebugLogCheckBox.TabIndex = 8;
        this.DebugLogCheckBox.Text = "debug log";
        this.DebugLogCheckBox.UseVisualStyleBackColor = true;
        // 
        // TwitchAuthorizeButton
        // 
        this.TwitchAuthorizeButton.Location = new System.Drawing.Point(3, 168);
        this.TwitchAuthorizeButton.Name = "TwitchAuthorizeButton";
        this.TwitchAuthorizeButton.Size = new System.Drawing.Size(120, 40);
        this.TwitchAuthorizeButton.TabIndex = 7;
        this.TwitchAuthorizeButton.Text = "authorize";
        this.TwitchAuthorizeButton.UseVisualStyleBackColor = true;
        this.TwitchAuthorizeButton.Click += new System.EventHandler(this.TwitchAuthorizeButton_Click);
        // 
        // LeftSplitContainer
        // 
        this.LeftSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
        this.LeftSplitContainer.Location = new System.Drawing.Point(0, 0);
        this.LeftSplitContainer.Name = "LeftSplitContainer";
        this.LeftSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
        // 
        // LeftSplitContainer.Panel1
        // 
        this.LeftSplitContainer.Panel1.Controls.Add(this.PluginSelectCheckedListBox);
        // 
        // LeftSplitContainer.Panel2
        // 
        this.LeftSplitContainer.Panel2.Controls.Add(this.LeftSplitLowerFlowLayout);
        this.LeftSplitContainer.Size = new System.Drawing.Size(200, 401);
        this.LeftSplitContainer.SplitterDistance = 104;
        this.LeftSplitContainer.TabIndex = 0;
        // 
        // PluginSelectCheckedListBox
        // 
        this.PluginSelectCheckedListBox.CheckOnClick = true;
        this.PluginSelectCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this.PluginSelectCheckedListBox.FormattingEnabled = true;
        this.PluginSelectCheckedListBox.Items.AddRange(new object[] {
            "plugin1",
            "plugin2",
            "plugin3"});
        this.PluginSelectCheckedListBox.Location = new System.Drawing.Point(0, 0);
        this.PluginSelectCheckedListBox.Name = "PluginSelectCheckedListBox";
        this.PluginSelectCheckedListBox.Size = new System.Drawing.Size(200, 104);
        this.PluginSelectCheckedListBox.TabIndex = 18;
        // 
        // LeftSplitLowerFlowLayout
        // 
        this.LeftSplitLowerFlowLayout.Controls.Add(this.MiksadoVersionLabel);
        this.LeftSplitLowerFlowLayout.Controls.Add(this.DebugLogCheckBox);
        this.LeftSplitLowerFlowLayout.Controls.Add(this.BugReportButton);
        this.LeftSplitLowerFlowLayout.Controls.Add(this.TwitchAuthorizeButton);
        this.LeftSplitLowerFlowLayout.Controls.Add(this.TwitchStatusLabel);
        this.LeftSplitLowerFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        this.LeftSplitLowerFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
        this.LeftSplitLowerFlowLayout.Location = new System.Drawing.Point(0, 0);
        this.LeftSplitLowerFlowLayout.Name = "LeftSplitLowerFlowLayout";
        this.LeftSplitLowerFlowLayout.Size = new System.Drawing.Size(200, 293);
        this.LeftSplitLowerFlowLayout.TabIndex = 0;
        // 
        // LeftSplitOriginalPanel
        // 
        this.LeftSplitOriginalPanel.Controls.Add(this.LeftSplitContainer);
        this.LeftSplitOriginalPanel.Dock = System.Windows.Forms.DockStyle.Left;
        this.LeftSplitOriginalPanel.Location = new System.Drawing.Point(0, 0);
        this.LeftSplitOriginalPanel.Name = "LeftSplitOriginalPanel";
        this.LeftSplitOriginalPanel.Size = new System.Drawing.Size(200, 401);
        this.LeftSplitOriginalPanel.TabIndex = 21;
        // 
        // RightDynamicPanel
        // 
        this.RightDynamicPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.RightDynamicPanel.Location = new System.Drawing.Point(200, 0);
        this.RightDynamicPanel.Name = "RightDynamicPanel";
        this.RightDynamicPanel.Size = new System.Drawing.Size(584, 401);
        this.RightDynamicPanel.TabIndex = 22;
        // 
        // MiksadoToolForm
        // 
        this.ClientSize = new System.Drawing.Size(784, 561);
        this.Controls.Add(this.RightDynamicPanel);
        this.Controls.Add(this.LeftSplitOriginalPanel);
        this.Controls.Add(this.ConsoleMultiline);
        this.Name = "MiksadoToolForm";
        this.LeftSplitContainer.Panel1.ResumeLayout(false);
        this.LeftSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.LeftSplitContainer)).EndInit();
        this.LeftSplitContainer.ResumeLayout(false);
        this.LeftSplitLowerFlowLayout.ResumeLayout(false);
        this.LeftSplitLowerFlowLayout.PerformLayout();
        this.LeftSplitOriginalPanel.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    private TextBox ConsoleMultiline = null!;
    private FolderBrowserDialog FileTouchBrowserDialog = null!;
    private Label MiksadoVersionLabel = null!;
    private Label TwitchStatusLabel = null!;
    private Button BugReportButton = null!;
    private CheckBox DebugLogCheckBox = null!;
    private Button TwitchAuthorizeButton = null!;
    private SplitContainer LeftSplitContainer = null!;
    private CheckedListBox PluginSelectCheckedListBox = null!;
    private Panel LeftSplitOriginalPanel = null!;
    private FlowLayoutPanel LeftSplitLowerFlowLayout = null!;
    private Panel RightDynamicPanel = null!;
}