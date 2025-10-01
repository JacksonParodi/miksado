using System;
using System.Windows.Forms;

namespace Miksado
{
    partial class ShufflerUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ShufflerTabControl = new System.Windows.Forms.TabControl();
            this.MainTabPage = new System.Windows.Forms.TabPage();
            this.MainTabFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TransportButtonFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.StartNewButton = new System.Windows.Forms.Button();
            this.ResumeButton = new System.Windows.Forms.Button();
            this.PauseUnpauseButton = new System.Windows.Forms.Button();
            this.ForceShuffleButton = new System.Windows.Forms.Button();
            this.GameFinishButton = new System.Windows.Forms.Button();
            this.SeedFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.SeedTextBoxLabel = new System.Windows.Forms.Label();
            this.SeedTextBox = new System.Windows.Forms.TextBox();
            this.AlgorithmFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.AlgorithmLabel = new System.Windows.Forms.Label();
            this.AlgorithmComboBox = new System.Windows.Forms.ComboBox();
            this.RngFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.RNGLabel = new System.Windows.Forms.Label();
            this.RngComboBox = new System.Windows.Forms.ComboBox();
            this.CooldownFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.CooldownLabel = new System.Windows.Forms.Label();
            this.CooldownUpDown = new System.Windows.Forms.NumericUpDown();
            this.TimerTabPage = new System.Windows.Forms.TabPage();
            this.TimerTabFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TimerEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.TimerMinFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TimerMinTimeUpDown = new System.Windows.Forms.NumericUpDown();
            this.MinTimeLabel = new System.Windows.Forms.Label();
            this.TimerMaxFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TimerMaxTimeUpDown = new System.Windows.Forms.NumericUpDown();
            this.MaxTimeLabel = new System.Windows.Forms.Label();
            this.ShowProgressBarCheckBox = new System.Windows.Forms.CheckBox();
            this.TimerProgressBar = new System.Windows.Forms.ProgressBar();
            this.FileTabPage = new System.Windows.Forms.TabPage();
            this.FileFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.FileTouchEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.FileTouchDialogButton = new System.Windows.Forms.Button();
            this.FileTouchPathDisplayTextBox = new System.Windows.Forms.TextBox();
            this.SubTabPage = new System.Windows.Forms.TabPage();
            this.SubTabFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TwitchSubEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.TwitchSubFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TwitchSubTierLabel = new System.Windows.Forms.Label();
            this.TwitchSubTierComboBox = new System.Windows.Forms.ComboBox();
            this.PollTabPage = new System.Windows.Forms.TabPage();
            this.PollFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollTitleTextBox = new System.Windows.Forms.TextBox();
            this.PollOption12FlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollOption1FlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollOption1ComboBox = new System.Windows.Forms.ComboBox();
            this.PollOption1EnableCheckBox = new System.Windows.Forms.CheckBox();
            this.PollOption2FlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollOption2ComboBox = new System.Windows.Forms.ComboBox();
            this.PollOption2EnableCheckBox = new System.Windows.Forms.CheckBox();
            this.PollOption34FlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollOption3FlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollOption3ComboBox = new System.Windows.Forms.ComboBox();
            this.PollOption3EnableCheckBox = new System.Windows.Forms.CheckBox();
            this.PollOption4FlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollOption4ComboBox = new System.Windows.Forms.ComboBox();
            this.PollOption4EnableCheckBox = new System.Windows.Forms.CheckBox();
            this.PollOption5FlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollOption5ComboBox = new System.Windows.Forms.ComboBox();
            this.PollOption5EnableCheckBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ChannelPointVotingCostLabel = new System.Windows.Forms.Label();
            this.ChannelPointVoteCostUpDown = new System.Windows.Forms.NumericUpDown();
            this.ChannelPointVotingEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.PollDurationFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollDurationLabel = new System.Windows.Forms.Label();
            this.PollDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.PollButtonFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.PollSendButton = new System.Windows.Forms.Button();
            this.PollEndButton = new System.Windows.Forms.Button();
            this.PollClearButton = new System.Windows.Forms.Button();
            this.BitsTabPage = new System.Windows.Forms.TabPage();
            this.BitsFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TwitchBitsEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.BitsAmountFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.BitsAmountLabel = new System.Windows.Forms.Label();
            this.TwitchBitsMinimumUpDown = new System.Windows.Forms.NumericUpDown();
            this.ChatCommandTabPage = new System.Windows.Forms.TabPage();
            this.ChatCommandFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TwitchChatCommandCheckBox = new System.Windows.Forms.CheckBox();
            this.TwitchCommandModEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.ChatCommandTextFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.ChatCommandLabel = new System.Windows.Forms.Label();
            this.TwitchChatCommandTextBox = new System.Windows.Forms.TextBox();
            this.RedemptionTabPage = new System.Windows.Forms.TabPage();
            this.TwitchRedeemFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TwitchRedemptionEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.RedemptionTitleFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.TwitchRedemptionTitleTextBox = new System.Windows.Forms.TextBox();
            this.RedemptionTitleLabel = new System.Windows.Forms.Label();
            this.ShufflerTabControl.SuspendLayout();
            this.MainTabPage.SuspendLayout();
            this.MainTabFlowLayout.SuspendLayout();
            this.TransportButtonFlowLayout.SuspendLayout();
            this.SeedFlowLayout.SuspendLayout();
            this.AlgorithmFlowLayout.SuspendLayout();
            this.RngFlowLayout.SuspendLayout();
            this.CooldownFlowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CooldownUpDown)).BeginInit();
            this.TimerTabPage.SuspendLayout();
            this.TimerTabFlowLayout.SuspendLayout();
            this.TimerMinFlowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimerMinTimeUpDown)).BeginInit();
            this.TimerMaxFlowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimerMaxTimeUpDown)).BeginInit();
            this.FileTabPage.SuspendLayout();
            this.FileFlowLayout.SuspendLayout();
            this.SubTabPage.SuspendLayout();
            this.SubTabFlowLayout.SuspendLayout();
            this.TwitchSubFlowLayout.SuspendLayout();
            this.PollTabPage.SuspendLayout();
            this.PollFlowLayout.SuspendLayout();
            this.PollOption12FlowLayout.SuspendLayout();
            this.PollOption1FlowLayout.SuspendLayout();
            this.PollOption2FlowLayout.SuspendLayout();
            this.PollOption34FlowLayout.SuspendLayout();
            this.PollOption3FlowLayout.SuspendLayout();
            this.PollOption4FlowLayout.SuspendLayout();
            this.PollOption5FlowLayout.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChannelPointVoteCostUpDown)).BeginInit();
            this.PollDurationFlowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PollDurationUpDown)).BeginInit();
            this.PollButtonFlowLayout.SuspendLayout();
            this.BitsTabPage.SuspendLayout();
            this.BitsFlowLayout.SuspendLayout();
            this.BitsAmountFlowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TwitchBitsMinimumUpDown)).BeginInit();
            this.ChatCommandTabPage.SuspendLayout();
            this.ChatCommandFlowLayout.SuspendLayout();
            this.ChatCommandTextFlowLayout.SuspendLayout();
            this.RedemptionTabPage.SuspendLayout();
            this.TwitchRedeemFlowLayout.SuspendLayout();
            this.RedemptionTitleFlowLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // ShufflerTabControl
            // 
            this.ShufflerTabControl.Controls.Add(this.MainTabPage);
            this.ShufflerTabControl.Controls.Add(this.TimerTabPage);
            this.ShufflerTabControl.Controls.Add(this.FileTabPage);
            this.ShufflerTabControl.Controls.Add(this.SubTabPage);
            this.ShufflerTabControl.Controls.Add(this.PollTabPage);
            this.ShufflerTabControl.Controls.Add(this.BitsTabPage);
            this.ShufflerTabControl.Controls.Add(this.ChatCommandTabPage);
            this.ShufflerTabControl.Controls.Add(this.RedemptionTabPage);
            this.ShufflerTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShufflerTabControl.Location = new System.Drawing.Point(0, 0);
            this.ShufflerTabControl.Name = "ShufflerTabControl";
            this.ShufflerTabControl.SelectedIndex = 0;
            this.ShufflerTabControl.Size = new System.Drawing.Size(600, 600);
            this.ShufflerTabControl.TabIndex = 18;
            // 
            // MainTabPage
            // 
            this.MainTabPage.Controls.Add(this.MainTabFlowLayout);
            this.MainTabPage.Location = new System.Drawing.Point(4, 22);
            this.MainTabPage.Name = "MainTabPage";
            this.MainTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MainTabPage.Size = new System.Drawing.Size(592, 574);
            this.MainTabPage.TabIndex = 0;
            this.MainTabPage.Text = "shuffler";
            this.MainTabPage.UseVisualStyleBackColor = true;
            // 
            // MainTabFlowLayout
            // 
            this.MainTabFlowLayout.Controls.Add(this.TransportButtonFlowLayout);
            this.MainTabFlowLayout.Controls.Add(this.SeedFlowLayout);
            this.MainTabFlowLayout.Controls.Add(this.AlgorithmFlowLayout);
            this.MainTabFlowLayout.Controls.Add(this.RngFlowLayout);
            this.MainTabFlowLayout.Controls.Add(this.CooldownFlowLayout);
            this.MainTabFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.MainTabFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.MainTabFlowLayout.Name = "MainTabFlowLayout";
            this.MainTabFlowLayout.Size = new System.Drawing.Size(586, 568);
            this.MainTabFlowLayout.TabIndex = 23;
            // 
            // TransportButtonFlowLayout
            // 
            this.TransportButtonFlowLayout.Controls.Add(this.StartNewButton);
            this.TransportButtonFlowLayout.Controls.Add(this.ResumeButton);
            this.TransportButtonFlowLayout.Controls.Add(this.PauseUnpauseButton);
            this.TransportButtonFlowLayout.Controls.Add(this.ForceShuffleButton);
            this.TransportButtonFlowLayout.Controls.Add(this.GameFinishButton);
            this.TransportButtonFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.TransportButtonFlowLayout.Name = "TransportButtonFlowLayout";
            this.TransportButtonFlowLayout.Size = new System.Drawing.Size(517, 55);
            this.TransportButtonFlowLayout.TabIndex = 18;
            // 
            // StartButton
            // 
            this.StartNewButton.Enabled = false;
            this.StartNewButton.Location = new System.Drawing.Point(3, 3);
            this.StartNewButton.Name = "StartButton";
            this.StartNewButton.Size = new System.Drawing.Size(80, 32);
            this.StartNewButton.TabIndex = 2;
            this.StartNewButton.Text = "new";
            this.StartNewButton.UseVisualStyleBackColor = true;
            // 
            // ResumeButton
            // 
            this.ResumeButton.Enabled = false;
            this.ResumeButton.Location = new System.Drawing.Point(89, 3);
            this.ResumeButton.Name = "ResumeButton";
            this.ResumeButton.Size = new System.Drawing.Size(80, 32);
            this.ResumeButton.TabIndex = 3;
            this.ResumeButton.Text = "resume";
            this.ResumeButton.UseVisualStyleBackColor = true;
            // 
            // PauseUnpauseButton
            // 
            this.PauseUnpauseButton.Location = new System.Drawing.Point(175, 3);
            this.PauseUnpauseButton.Name = "PauseUnpauseButton";
            this.PauseUnpauseButton.Size = new System.Drawing.Size(80, 32);
            this.PauseUnpauseButton.TabIndex = 4;
            this.PauseUnpauseButton.Text = "pau/unpau";
            this.PauseUnpauseButton.UseVisualStyleBackColor = true;
            // 
            // ForceShuffleButton
            // 
            this.ForceShuffleButton.Enabled = false;
            this.ForceShuffleButton.Location = new System.Drawing.Point(261, 3);
            this.ForceShuffleButton.Name = "ForceShuffleButton";
            this.ForceShuffleButton.Size = new System.Drawing.Size(80, 32);
            this.ForceShuffleButton.TabIndex = 1;
            this.ForceShuffleButton.Text = "force shuffle";
            this.ForceShuffleButton.UseVisualStyleBackColor = true;
            // 
            // GameFinishButton
            // 
            this.GameFinishButton.Location = new System.Drawing.Point(347, 3);
            this.GameFinishButton.Name = "GameFinishButton";
            this.GameFinishButton.Size = new System.Drawing.Size(75, 32);
            this.GameFinishButton.TabIndex = 5;
            this.GameFinishButton.Text = "finish game";
            this.GameFinishButton.UseVisualStyleBackColor = true;
            // 
            // SeedFlowLayout
            // 
            this.SeedFlowLayout.Controls.Add(this.SeedTextBoxLabel);
            this.SeedFlowLayout.Controls.Add(this.SeedTextBox);
            this.SeedFlowLayout.Location = new System.Drawing.Point(3, 64);
            this.SeedFlowLayout.Name = "SeedFlowLayout";
            this.SeedFlowLayout.Size = new System.Drawing.Size(310, 46);
            this.SeedFlowLayout.TabIndex = 20;
            // 
            // SeedTextBoxLabel
            // 
            this.SeedTextBoxLabel.AutoSize = true;
            this.SeedTextBoxLabel.Location = new System.Drawing.Point(3, 0);
            this.SeedTextBoxLabel.Name = "SeedTextBoxLabel";
            this.SeedTextBoxLabel.Size = new System.Drawing.Size(30, 13);
            this.SeedTextBoxLabel.TabIndex = 15;
            this.SeedTextBoxLabel.Text = "seed";
            // 
            // SeedTextBox
            // 
            this.SeedTextBox.Location = new System.Drawing.Point(39, 3);
            this.SeedTextBox.MaxLength = 16;
            this.SeedTextBox.Name = "SeedTextBox";
            this.SeedTextBox.Size = new System.Drawing.Size(216, 20);
            this.SeedTextBox.TabIndex = 14;
            this.SeedTextBox.WordWrap = false;
            // 
            // AlgorithmFlowLayout
            // 
            this.AlgorithmFlowLayout.Controls.Add(this.AlgorithmLabel);
            this.AlgorithmFlowLayout.Controls.Add(this.AlgorithmComboBox);
            this.AlgorithmFlowLayout.Location = new System.Drawing.Point(3, 116);
            this.AlgorithmFlowLayout.Name = "AlgorithmFlowLayout";
            this.AlgorithmFlowLayout.Size = new System.Drawing.Size(304, 44);
            this.AlgorithmFlowLayout.TabIndex = 21;
            // 
            // AlgorithmLabel
            // 
            this.AlgorithmLabel.AutoSize = true;
            this.AlgorithmLabel.Location = new System.Drawing.Point(3, 0);
            this.AlgorithmLabel.Name = "AlgorithmLabel";
            this.AlgorithmLabel.Size = new System.Drawing.Size(49, 13);
            this.AlgorithmLabel.TabIndex = 7;
            this.AlgorithmLabel.Text = "algorithm";
            // 
            // AlgorithmComboBox
            // 
            this.AlgorithmComboBox.FormattingEnabled = true;
            this.AlgorithmComboBox.Location = new System.Drawing.Point(58, 3);
            this.AlgorithmComboBox.Name = "AlgorithmComboBox";
            this.AlgorithmComboBox.Size = new System.Drawing.Size(197, 21);
            this.AlgorithmComboBox.TabIndex = 5;
            // 
            // RngFlowLayout
            // 
            this.RngFlowLayout.Controls.Add(this.RNGLabel);
            this.RngFlowLayout.Controls.Add(this.RngComboBox);
            this.RngFlowLayout.Location = new System.Drawing.Point(3, 166);
            this.RngFlowLayout.Name = "RngFlowLayout";
            this.RngFlowLayout.Size = new System.Drawing.Size(285, 43);
            this.RngFlowLayout.TabIndex = 19;
            // 
            // RNGLabel
            // 
            this.RNGLabel.AutoSize = true;
            this.RNGLabel.Location = new System.Drawing.Point(3, 0);
            this.RNGLabel.Name = "RNGLabel";
            this.RNGLabel.Size = new System.Drawing.Size(22, 13);
            this.RNGLabel.TabIndex = 6;
            this.RNGLabel.Text = "rng";
            // 
            // RngComboBox
            // 
            this.RngComboBox.FormattingEnabled = true;
            this.RngComboBox.Location = new System.Drawing.Point(31, 3);
            this.RngComboBox.Name = "RngComboBox";
            this.RngComboBox.Size = new System.Drawing.Size(204, 21);
            this.RngComboBox.TabIndex = 4;
            // 
            // CooldownFlowLayout
            // 
            this.CooldownFlowLayout.Controls.Add(this.CooldownLabel);
            this.CooldownFlowLayout.Controls.Add(this.CooldownUpDown);
            this.CooldownFlowLayout.Location = new System.Drawing.Point(3, 215);
            this.CooldownFlowLayout.Name = "CooldownFlowLayout";
            this.CooldownFlowLayout.Size = new System.Drawing.Size(200, 37);
            this.CooldownFlowLayout.TabIndex = 22;
            // 
            // CooldownLabel
            // 
            this.CooldownLabel.AutoSize = true;
            this.CooldownLabel.Location = new System.Drawing.Point(3, 0);
            this.CooldownLabel.Name = "CooldownLabel";
            this.CooldownLabel.Size = new System.Drawing.Size(53, 13);
            this.CooldownLabel.TabIndex = 0;
            this.CooldownLabel.Text = "cooldown";
            // 
            // CooldownUpDown
            // 
            this.CooldownUpDown.Location = new System.Drawing.Point(62, 3);
            this.CooldownUpDown.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.CooldownUpDown.Name = "CooldownUpDown";
            this.CooldownUpDown.Size = new System.Drawing.Size(53, 20);
            this.CooldownUpDown.TabIndex = 17;
            this.CooldownUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // TimerTabPage
            // 
            this.TimerTabPage.Controls.Add(this.TimerTabFlowLayout);
            this.TimerTabPage.Location = new System.Drawing.Point(4, 22);
            this.TimerTabPage.Name = "TimerTabPage";
            this.TimerTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.TimerTabPage.Size = new System.Drawing.Size(592, 574);
            this.TimerTabPage.TabIndex = 1;
            this.TimerTabPage.Text = "timer";
            this.TimerTabPage.UseVisualStyleBackColor = true;
            // 
            // TimerTabFlowLayout
            // 
            this.TimerTabFlowLayout.Controls.Add(this.TimerEnableCheckBox);
            this.TimerTabFlowLayout.Controls.Add(this.TimerMinFlowLayout);
            this.TimerTabFlowLayout.Controls.Add(this.TimerMaxFlowLayout);
            this.TimerTabFlowLayout.Controls.Add(this.ShowProgressBarCheckBox);
            this.TimerTabFlowLayout.Controls.Add(this.TimerProgressBar);
            this.TimerTabFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimerTabFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.TimerTabFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.TimerTabFlowLayout.Name = "TimerTabFlowLayout";
            this.TimerTabFlowLayout.Size = new System.Drawing.Size(586, 568);
            this.TimerTabFlowLayout.TabIndex = 18;
            // 
            // TimerEnableCheckBox
            // 
            this.TimerEnableCheckBox.AutoSize = true;
            this.TimerEnableCheckBox.Location = new System.Drawing.Point(3, 3);
            this.TimerEnableCheckBox.Name = "TimerEnableCheckBox";
            this.TimerEnableCheckBox.Size = new System.Drawing.Size(58, 17);
            this.TimerEnableCheckBox.TabIndex = 13;
            this.TimerEnableCheckBox.Text = "enable";
            this.TimerEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // TimerMinFlowLayout
            // 
            this.TimerMinFlowLayout.Controls.Add(this.TimerMinTimeUpDown);
            this.TimerMinFlowLayout.Controls.Add(this.MinTimeLabel);
            this.TimerMinFlowLayout.Location = new System.Drawing.Point(3, 26);
            this.TimerMinFlowLayout.Name = "TimerMinFlowLayout";
            this.TimerMinFlowLayout.Size = new System.Drawing.Size(114, 37);
            this.TimerMinFlowLayout.TabIndex = 16;
            // 
            // TimerMinTimeUpDown
            // 
            this.TimerMinTimeUpDown.Location = new System.Drawing.Point(3, 3);
            this.TimerMinTimeUpDown.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.TimerMinTimeUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.TimerMinTimeUpDown.Name = "TimerMinTimeUpDown";
            this.TimerMinTimeUpDown.Size = new System.Drawing.Size(46, 20);
            this.TimerMinTimeUpDown.TabIndex = 9;
            this.TimerMinTimeUpDown.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // MinTimeLabel
            // 
            this.MinTimeLabel.AutoSize = true;
            this.MinTimeLabel.Location = new System.Drawing.Point(55, 0);
            this.MinTimeLabel.Name = "MinTimeLabel";
            this.MinTimeLabel.Size = new System.Drawing.Size(23, 13);
            this.MinTimeLabel.TabIndex = 11;
            this.MinTimeLabel.Text = "min";
            // 
            // TimerMaxFlowLayout
            // 
            this.TimerMaxFlowLayout.Controls.Add(this.TimerMaxTimeUpDown);
            this.TimerMaxFlowLayout.Controls.Add(this.MaxTimeLabel);
            this.TimerMaxFlowLayout.Location = new System.Drawing.Point(3, 69);
            this.TimerMaxFlowLayout.Name = "TimerMaxFlowLayout";
            this.TimerMaxFlowLayout.Size = new System.Drawing.Size(114, 36);
            this.TimerMaxFlowLayout.TabIndex = 17;
            // 
            // TimerMaxTimeUpDown
            // 
            this.TimerMaxTimeUpDown.Location = new System.Drawing.Point(3, 3);
            this.TimerMaxTimeUpDown.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.TimerMaxTimeUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.TimerMaxTimeUpDown.Name = "TimerMaxTimeUpDown";
            this.TimerMaxTimeUpDown.Size = new System.Drawing.Size(46, 20);
            this.TimerMaxTimeUpDown.TabIndex = 10;
            this.TimerMaxTimeUpDown.Value = new decimal(new int[] {
            45,
            0,
            0,
            0});
            // 
            // MaxTimeLabel
            // 
            this.MaxTimeLabel.AutoSize = true;
            this.MaxTimeLabel.Location = new System.Drawing.Point(55, 0);
            this.MaxTimeLabel.Name = "MaxTimeLabel";
            this.MaxTimeLabel.Size = new System.Drawing.Size(26, 13);
            this.MaxTimeLabel.TabIndex = 12;
            this.MaxTimeLabel.Text = "max";
            // 
            // ShowProgressBarCheckBox
            // 
            this.ShowProgressBarCheckBox.AutoSize = true;
            this.ShowProgressBarCheckBox.Location = new System.Drawing.Point(3, 111);
            this.ShowProgressBarCheckBox.Name = "ShowProgressBarCheckBox";
            this.ShowProgressBarCheckBox.Size = new System.Drawing.Size(112, 17);
            this.ShowProgressBarCheckBox.TabIndex = 15;
            this.ShowProgressBarCheckBox.Text = "show progress bar";
            this.ShowProgressBarCheckBox.UseVisualStyleBackColor = true;
            // 
            // TimerProgressBar
            // 
            this.TimerProgressBar.Location = new System.Drawing.Point(3, 134);
            this.TimerProgressBar.Name = "TimerProgressBar";
            this.TimerProgressBar.Size = new System.Drawing.Size(207, 23);
            this.TimerProgressBar.TabIndex = 14;
            // 
            // FileTabPage
            // 
            this.FileTabPage.Controls.Add(this.FileFlowLayout);
            this.FileTabPage.Location = new System.Drawing.Point(4, 22);
            this.FileTabPage.Name = "FileTabPage";
            this.FileTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.FileTabPage.Size = new System.Drawing.Size(592, 574);
            this.FileTabPage.TabIndex = 2;
            this.FileTabPage.Text = "file";
            this.FileTabPage.UseVisualStyleBackColor = true;
            // 
            // FileFlowLayout
            // 
            this.FileFlowLayout.Controls.Add(this.FileTouchEnableCheckBox);
            this.FileFlowLayout.Controls.Add(this.FileTouchDialogButton);
            this.FileFlowLayout.Controls.Add(this.FileTouchPathDisplayTextBox);
            this.FileFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.FileFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.FileFlowLayout.Name = "FileFlowLayout";
            this.FileFlowLayout.Size = new System.Drawing.Size(586, 568);
            this.FileFlowLayout.TabIndex = 3;
            // 
            // FileTouchEnableCheckBox
            // 
            this.FileTouchEnableCheckBox.AutoSize = true;
            this.FileTouchEnableCheckBox.Location = new System.Drawing.Point(3, 3);
            this.FileTouchEnableCheckBox.Name = "FileTouchEnableCheckBox";
            this.FileTouchEnableCheckBox.Size = new System.Drawing.Size(104, 17);
            this.FileTouchEnableCheckBox.TabIndex = 0;
            this.FileTouchEnableCheckBox.Text = "enable file touch";
            this.FileTouchEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // FileTouchDialogButton
            // 
            this.FileTouchDialogButton.Location = new System.Drawing.Point(3, 26);
            this.FileTouchDialogButton.Name = "FileTouchDialogButton";
            this.FileTouchDialogButton.Size = new System.Drawing.Size(75, 23);
            this.FileTouchDialogButton.TabIndex = 1;
            this.FileTouchDialogButton.Text = "set folder";
            this.FileTouchDialogButton.UseVisualStyleBackColor = true;
            // 
            // FileTouchPathDisplayTextBox
            // 
            this.FileTouchPathDisplayTextBox.Location = new System.Drawing.Point(3, 55);
            this.FileTouchPathDisplayTextBox.MaxLength = 256;
            this.FileTouchPathDisplayTextBox.Name = "FileTouchPathDisplayTextBox";
            this.FileTouchPathDisplayTextBox.ReadOnly = true;
            this.FileTouchPathDisplayTextBox.Size = new System.Drawing.Size(156, 20);
            this.FileTouchPathDisplayTextBox.TabIndex = 2;
            // 
            // SubTabPage
            // 
            this.SubTabPage.Controls.Add(this.SubTabFlowLayout);
            this.SubTabPage.Location = new System.Drawing.Point(4, 22);
            this.SubTabPage.Name = "SubTabPage";
            this.SubTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.SubTabPage.Size = new System.Drawing.Size(592, 574);
            this.SubTabPage.TabIndex = 3;
            this.SubTabPage.Text = "sub";
            this.SubTabPage.UseVisualStyleBackColor = true;
            // 
            // SubTabFlowLayout
            // 
            this.SubTabFlowLayout.Controls.Add(this.TwitchSubEnableCheckBox);
            this.SubTabFlowLayout.Controls.Add(this.TwitchSubFlowLayout);
            this.SubTabFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SubTabFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SubTabFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.SubTabFlowLayout.Name = "SubTabFlowLayout";
            this.SubTabFlowLayout.Size = new System.Drawing.Size(586, 568);
            this.SubTabFlowLayout.TabIndex = 16;
            // 
            // TwitchSubEnableCheckBox
            // 
            this.TwitchSubEnableCheckBox.AutoSize = true;
            this.TwitchSubEnableCheckBox.Location = new System.Drawing.Point(3, 3);
            this.TwitchSubEnableCheckBox.Name = "TwitchSubEnableCheckBox";
            this.TwitchSubEnableCheckBox.Size = new System.Drawing.Size(58, 17);
            this.TwitchSubEnableCheckBox.TabIndex = 1;
            this.TwitchSubEnableCheckBox.Text = "enable";
            this.TwitchSubEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // TwitchSubFlowLayout
            // 
            this.TwitchSubFlowLayout.Controls.Add(this.TwitchSubTierLabel);
            this.TwitchSubFlowLayout.Controls.Add(this.TwitchSubTierComboBox);
            this.TwitchSubFlowLayout.Location = new System.Drawing.Point(3, 26);
            this.TwitchSubFlowLayout.Name = "TwitchSubFlowLayout";
            this.TwitchSubFlowLayout.Size = new System.Drawing.Size(221, 47);
            this.TwitchSubFlowLayout.TabIndex = 12;
            // 
            // TwitchSubTierLabel
            // 
            this.TwitchSubTierLabel.AutoSize = true;
            this.TwitchSubTierLabel.Location = new System.Drawing.Point(3, 0);
            this.TwitchSubTierLabel.Name = "TwitchSubTierLabel";
            this.TwitchSubTierLabel.Size = new System.Drawing.Size(21, 13);
            this.TwitchSubTierLabel.TabIndex = 5;
            this.TwitchSubTierLabel.Text = "tier";
            // 
            // TwitchSubTierComboBox
            // 
            this.TwitchSubTierComboBox.FormattingEnabled = true;
            this.TwitchSubTierComboBox.Items.AddRange(new object[] {
            "tier 1",
            "tier 2",
            "tier 3"});
            this.TwitchSubTierComboBox.Location = new System.Drawing.Point(30, 3);
            this.TwitchSubTierComboBox.Name = "TwitchSubTierComboBox";
            this.TwitchSubTierComboBox.Size = new System.Drawing.Size(134, 21);
            this.TwitchSubTierComboBox.TabIndex = 4;
            // 
            // PollTabPage
            // 
            this.PollTabPage.Controls.Add(this.PollFlowLayout);
            this.PollTabPage.Location = new System.Drawing.Point(4, 22);
            this.PollTabPage.Name = "PollTabPage";
            this.PollTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.PollTabPage.Size = new System.Drawing.Size(592, 574);
            this.PollTabPage.TabIndex = 4;
            this.PollTabPage.Text = "poll";
            this.PollTabPage.UseVisualStyleBackColor = true;
            // 
            // PollFlowLayout
            // 
            this.PollFlowLayout.Controls.Add(this.PollTitleTextBox);
            this.PollFlowLayout.Controls.Add(this.PollOption12FlowLayout);
            this.PollFlowLayout.Controls.Add(this.PollOption34FlowLayout);
            this.PollFlowLayout.Controls.Add(this.PollOption5FlowLayout);
            this.PollFlowLayout.Controls.Add(this.flowLayoutPanel1);
            this.PollFlowLayout.Controls.Add(this.PollDurationFlowLayout);
            this.PollFlowLayout.Controls.Add(this.PollButtonFlowLayout);
            this.PollFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PollFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.PollFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.PollFlowLayout.Name = "PollFlowLayout";
            this.PollFlowLayout.Size = new System.Drawing.Size(586, 568);
            this.PollFlowLayout.TabIndex = 0;
            // 
            // PollTitleTextBox
            // 
            this.PollTitleTextBox.Location = new System.Drawing.Point(3, 3);
            this.PollTitleTextBox.Name = "PollTitleTextBox";
            this.PollTitleTextBox.Size = new System.Drawing.Size(200, 20);
            this.PollTitleTextBox.TabIndex = 6;
            this.PollTitleTextBox.Text = "_POLL_TITLE_";
            // 
            // PollOption12FlowLayout
            // 
            this.PollOption12FlowLayout.Controls.Add(this.PollOption1FlowLayout);
            this.PollOption12FlowLayout.Controls.Add(this.PollOption2FlowLayout);
            this.PollOption12FlowLayout.Location = new System.Drawing.Point(3, 29);
            this.PollOption12FlowLayout.Name = "PollOption12FlowLayout";
            this.PollOption12FlowLayout.Size = new System.Drawing.Size(425, 55);
            this.PollOption12FlowLayout.TabIndex = 24;
            // 
            // PollOption1FlowLayout
            // 
            this.PollOption1FlowLayout.Controls.Add(this.PollOption1ComboBox);
            this.PollOption1FlowLayout.Controls.Add(this.PollOption1EnableCheckBox);
            this.PollOption1FlowLayout.Location = new System.Drawing.Point(3, 3);
            this.PollOption1FlowLayout.Name = "PollOption1FlowLayout";
            this.PollOption1FlowLayout.Size = new System.Drawing.Size(200, 39);
            this.PollOption1FlowLayout.TabIndex = 18;
            // 
            // PollOption1ComboBox
            // 
            this.PollOption1ComboBox.FormattingEnabled = true;
            this.PollOption1ComboBox.Location = new System.Drawing.Point(3, 3);
            this.PollOption1ComboBox.Name = "PollOption1ComboBox";
            this.PollOption1ComboBox.Size = new System.Drawing.Size(121, 21);
            this.PollOption1ComboBox.TabIndex = 1;
            // 
            // PollOption1EnableCheckBox
            // 
            this.PollOption1EnableCheckBox.AutoSize = true;
            this.PollOption1EnableCheckBox.Location = new System.Drawing.Point(130, 3);
            this.PollOption1EnableCheckBox.Name = "PollOption1EnableCheckBox";
            this.PollOption1EnableCheckBox.Size = new System.Drawing.Size(15, 14);
            this.PollOption1EnableCheckBox.TabIndex = 13;
            this.PollOption1EnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // PollOption2FlowLayout
            // 
            this.PollOption2FlowLayout.Controls.Add(this.PollOption2ComboBox);
            this.PollOption2FlowLayout.Controls.Add(this.PollOption2EnableCheckBox);
            this.PollOption2FlowLayout.Location = new System.Drawing.Point(209, 3);
            this.PollOption2FlowLayout.Name = "PollOption2FlowLayout";
            this.PollOption2FlowLayout.Size = new System.Drawing.Size(200, 37);
            this.PollOption2FlowLayout.TabIndex = 19;
            // 
            // PollOption2ComboBox
            // 
            this.PollOption2ComboBox.FormattingEnabled = true;
            this.PollOption2ComboBox.Location = new System.Drawing.Point(3, 3);
            this.PollOption2ComboBox.Name = "PollOption2ComboBox";
            this.PollOption2ComboBox.Size = new System.Drawing.Size(121, 21);
            this.PollOption2ComboBox.TabIndex = 2;
            // 
            // PollOption2EnableCheckBox
            // 
            this.PollOption2EnableCheckBox.AutoSize = true;
            this.PollOption2EnableCheckBox.Location = new System.Drawing.Point(130, 3);
            this.PollOption2EnableCheckBox.Name = "PollOption2EnableCheckBox";
            this.PollOption2EnableCheckBox.Size = new System.Drawing.Size(15, 14);
            this.PollOption2EnableCheckBox.TabIndex = 14;
            this.PollOption2EnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // PollOption34FlowLayout
            // 
            this.PollOption34FlowLayout.Controls.Add(this.PollOption3FlowLayout);
            this.PollOption34FlowLayout.Controls.Add(this.PollOption4FlowLayout);
            this.PollOption34FlowLayout.Location = new System.Drawing.Point(3, 90);
            this.PollOption34FlowLayout.Name = "PollOption34FlowLayout";
            this.PollOption34FlowLayout.Size = new System.Drawing.Size(425, 67);
            this.PollOption34FlowLayout.TabIndex = 25;
            // 
            // PollOption3FlowLayout
            // 
            this.PollOption3FlowLayout.Controls.Add(this.PollOption3ComboBox);
            this.PollOption3FlowLayout.Controls.Add(this.PollOption3EnableCheckBox);
            this.PollOption3FlowLayout.Location = new System.Drawing.Point(3, 3);
            this.PollOption3FlowLayout.Name = "PollOption3FlowLayout";
            this.PollOption3FlowLayout.Size = new System.Drawing.Size(200, 46);
            this.PollOption3FlowLayout.TabIndex = 20;
            // 
            // PollOption3ComboBox
            // 
            this.PollOption3ComboBox.FormattingEnabled = true;
            this.PollOption3ComboBox.Location = new System.Drawing.Point(3, 3);
            this.PollOption3ComboBox.Name = "PollOption3ComboBox";
            this.PollOption3ComboBox.Size = new System.Drawing.Size(121, 21);
            this.PollOption3ComboBox.TabIndex = 3;
            // 
            // PollOption3EnableCheckBox
            // 
            this.PollOption3EnableCheckBox.AutoSize = true;
            this.PollOption3EnableCheckBox.Location = new System.Drawing.Point(130, 3);
            this.PollOption3EnableCheckBox.Name = "PollOption3EnableCheckBox";
            this.PollOption3EnableCheckBox.Size = new System.Drawing.Size(15, 14);
            this.PollOption3EnableCheckBox.TabIndex = 15;
            this.PollOption3EnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // PollOption4FlowLayout
            // 
            this.PollOption4FlowLayout.Controls.Add(this.PollOption4ComboBox);
            this.PollOption4FlowLayout.Controls.Add(this.PollOption4EnableCheckBox);
            this.PollOption4FlowLayout.Location = new System.Drawing.Point(209, 3);
            this.PollOption4FlowLayout.Name = "PollOption4FlowLayout";
            this.PollOption4FlowLayout.Size = new System.Drawing.Size(200, 45);
            this.PollOption4FlowLayout.TabIndex = 21;
            // 
            // PollOption4ComboBox
            // 
            this.PollOption4ComboBox.FormattingEnabled = true;
            this.PollOption4ComboBox.Location = new System.Drawing.Point(3, 3);
            this.PollOption4ComboBox.Name = "PollOption4ComboBox";
            this.PollOption4ComboBox.Size = new System.Drawing.Size(121, 21);
            this.PollOption4ComboBox.TabIndex = 4;
            // 
            // PollOption4EnableCheckBox
            // 
            this.PollOption4EnableCheckBox.AutoSize = true;
            this.PollOption4EnableCheckBox.Location = new System.Drawing.Point(130, 3);
            this.PollOption4EnableCheckBox.Name = "PollOption4EnableCheckBox";
            this.PollOption4EnableCheckBox.Size = new System.Drawing.Size(15, 14);
            this.PollOption4EnableCheckBox.TabIndex = 16;
            this.PollOption4EnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // PollOption5FlowLayout
            // 
            this.PollOption5FlowLayout.Controls.Add(this.PollOption5ComboBox);
            this.PollOption5FlowLayout.Controls.Add(this.PollOption5EnableCheckBox);
            this.PollOption5FlowLayout.Location = new System.Drawing.Point(3, 163);
            this.PollOption5FlowLayout.Name = "PollOption5FlowLayout";
            this.PollOption5FlowLayout.Size = new System.Drawing.Size(200, 42);
            this.PollOption5FlowLayout.TabIndex = 22;
            // 
            // PollOption5ComboBox
            // 
            this.PollOption5ComboBox.FormattingEnabled = true;
            this.PollOption5ComboBox.Items.AddRange(new object[] {
            "game1",
            "game2"});
            this.PollOption5ComboBox.Location = new System.Drawing.Point(3, 3);
            this.PollOption5ComboBox.Name = "PollOption5ComboBox";
            this.PollOption5ComboBox.Size = new System.Drawing.Size(121, 21);
            this.PollOption5ComboBox.TabIndex = 5;
            // 
            // PollOption5EnableCheckBox
            // 
            this.PollOption5EnableCheckBox.AutoSize = true;
            this.PollOption5EnableCheckBox.Location = new System.Drawing.Point(130, 3);
            this.PollOption5EnableCheckBox.Name = "PollOption5EnableCheckBox";
            this.PollOption5EnableCheckBox.Size = new System.Drawing.Size(15, 14);
            this.PollOption5EnableCheckBox.TabIndex = 17;
            this.PollOption5EnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.ChannelPointVotingCostLabel);
            this.flowLayoutPanel1.Controls.Add(this.ChannelPointVoteCostUpDown);
            this.flowLayoutPanel1.Controls.Add(this.ChannelPointVotingEnableCheckBox);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 211);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(350, 39);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // ChannelPointVotingCostLabel
            // 
            this.ChannelPointVotingCostLabel.AutoSize = true;
            this.ChannelPointVotingCostLabel.Location = new System.Drawing.Point(3, 0);
            this.ChannelPointVotingCostLabel.Name = "ChannelPointVotingCostLabel";
            this.ChannelPointVotingCostLabel.Size = new System.Drawing.Size(94, 13);
            this.ChannelPointVotingCostLabel.TabIndex = 10;
            this.ChannelPointVotingCostLabel.Text = "channel point cost";
            // 
            // ChannelPointVoteCostUpDown
            // 
            this.ChannelPointVoteCostUpDown.Location = new System.Drawing.Point(103, 3);
            this.ChannelPointVoteCostUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ChannelPointVoteCostUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ChannelPointVoteCostUpDown.Name = "ChannelPointVoteCostUpDown";
            this.ChannelPointVoteCostUpDown.Size = new System.Drawing.Size(101, 20);
            this.ChannelPointVoteCostUpDown.TabIndex = 9;
            this.ChannelPointVoteCostUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ChannelPointVotingEnableCheckBox
            // 
            this.ChannelPointVotingEnableCheckBox.AutoSize = true;
            this.ChannelPointVotingEnableCheckBox.Location = new System.Drawing.Point(210, 3);
            this.ChannelPointVotingEnableCheckBox.Name = "ChannelPointVotingEnableCheckBox";
            this.ChannelPointVotingEnableCheckBox.Size = new System.Drawing.Size(122, 17);
            this.ChannelPointVotingEnableCheckBox.TabIndex = 8;
            this.ChannelPointVotingEnableCheckBox.Text = "channel point voting";
            this.ChannelPointVotingEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // PollDurationFlowLayout
            // 
            this.PollDurationFlowLayout.Controls.Add(this.PollDurationLabel);
            this.PollDurationFlowLayout.Controls.Add(this.PollDurationUpDown);
            this.PollDurationFlowLayout.Location = new System.Drawing.Point(3, 256);
            this.PollDurationFlowLayout.Name = "PollDurationFlowLayout";
            this.PollDurationFlowLayout.Size = new System.Drawing.Size(274, 44);
            this.PollDurationFlowLayout.TabIndex = 12;
            // 
            // PollDurationLabel
            // 
            this.PollDurationLabel.AutoSize = true;
            this.PollDurationLabel.Location = new System.Drawing.Point(3, 0);
            this.PollDurationLabel.Name = "PollDurationLabel";
            this.PollDurationLabel.Size = new System.Drawing.Size(45, 13);
            this.PollDurationLabel.TabIndex = 8;
            this.PollDurationLabel.Text = "duration";
            // 
            // PollDurationUpDown
            // 
            this.PollDurationUpDown.Location = new System.Drawing.Point(54, 3);
            this.PollDurationUpDown.Maximum = new decimal(new int[] {
            1800,
            0,
            0,
            0});
            this.PollDurationUpDown.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.PollDurationUpDown.Name = "PollDurationUpDown";
            this.PollDurationUpDown.Size = new System.Drawing.Size(88, 20);
            this.PollDurationUpDown.TabIndex = 7;
            this.PollDurationUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // PollButtonFlowLayout
            // 
            this.PollButtonFlowLayout.Controls.Add(this.PollSendButton);
            this.PollButtonFlowLayout.Controls.Add(this.PollEndButton);
            this.PollButtonFlowLayout.Controls.Add(this.PollClearButton);
            this.PollButtonFlowLayout.Location = new System.Drawing.Point(3, 306);
            this.PollButtonFlowLayout.Name = "PollButtonFlowLayout";
            this.PollButtonFlowLayout.Size = new System.Drawing.Size(274, 53);
            this.PollButtonFlowLayout.TabIndex = 23;
            // 
            // PollSendButton
            // 
            this.PollSendButton.Location = new System.Drawing.Point(3, 3);
            this.PollSendButton.Name = "PollSendButton";
            this.PollSendButton.Size = new System.Drawing.Size(75, 23);
            this.PollSendButton.TabIndex = 0;
            this.PollSendButton.Text = "send";
            this.PollSendButton.UseVisualStyleBackColor = true;
            // 
            // PollEndButton
            // 
            this.PollEndButton.Location = new System.Drawing.Point(84, 3);
            this.PollEndButton.Name = "PollEndButton";
            this.PollEndButton.Size = new System.Drawing.Size(75, 23);
            this.PollEndButton.TabIndex = 0;
            this.PollEndButton.Text = "end";
            this.PollEndButton.UseVisualStyleBackColor = true;
            // 
            // PollClearButton
            // 
            this.PollClearButton.Location = new System.Drawing.Point(165, 3);
            this.PollClearButton.Name = "PollClearButton";
            this.PollClearButton.Size = new System.Drawing.Size(75, 23);
            this.PollClearButton.TabIndex = 1;
            this.PollClearButton.Text = "clear";
            this.PollClearButton.UseVisualStyleBackColor = true;
            // 
            // BitsTabPage
            // 
            this.BitsTabPage.Controls.Add(this.BitsFlowLayout);
            this.BitsTabPage.Location = new System.Drawing.Point(4, 22);
            this.BitsTabPage.Name = "BitsTabPage";
            this.BitsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.BitsTabPage.Size = new System.Drawing.Size(592, 574);
            this.BitsTabPage.TabIndex = 5;
            this.BitsTabPage.Text = "bits";
            this.BitsTabPage.UseVisualStyleBackColor = true;
            // 
            // BitsFlowLayout
            // 
            this.BitsFlowLayout.Controls.Add(this.TwitchBitsEnableCheckBox);
            this.BitsFlowLayout.Controls.Add(this.BitsAmountFlowLayout);
            this.BitsFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BitsFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.BitsFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.BitsFlowLayout.Name = "BitsFlowLayout";
            this.BitsFlowLayout.Size = new System.Drawing.Size(586, 568);
            this.BitsFlowLayout.TabIndex = 16;
            // 
            // TwitchBitsEnableCheckBox
            // 
            this.TwitchBitsEnableCheckBox.AutoSize = true;
            this.TwitchBitsEnableCheckBox.Location = new System.Drawing.Point(3, 3);
            this.TwitchBitsEnableCheckBox.Name = "TwitchBitsEnableCheckBox";
            this.TwitchBitsEnableCheckBox.Size = new System.Drawing.Size(58, 17);
            this.TwitchBitsEnableCheckBox.TabIndex = 2;
            this.TwitchBitsEnableCheckBox.Text = "enable";
            this.TwitchBitsEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // BitsAmountFlowLayout
            // 
            this.BitsAmountFlowLayout.Controls.Add(this.BitsAmountLabel);
            this.BitsAmountFlowLayout.Controls.Add(this.TwitchBitsMinimumUpDown);
            this.BitsAmountFlowLayout.Location = new System.Drawing.Point(3, 26);
            this.BitsAmountFlowLayout.Name = "BitsAmountFlowLayout";
            this.BitsAmountFlowLayout.Size = new System.Drawing.Size(200, 42);
            this.BitsAmountFlowLayout.TabIndex = 7;
            // 
            // BitsAmountLabel
            // 
            this.BitsAmountLabel.AutoSize = true;
            this.BitsAmountLabel.Location = new System.Drawing.Point(3, 0);
            this.BitsAmountLabel.Name = "BitsAmountLabel";
            this.BitsAmountLabel.Size = new System.Drawing.Size(42, 13);
            this.BitsAmountLabel.TabIndex = 6;
            this.BitsAmountLabel.Text = "amount";
            // 
            // TwitchBitsMinimumUpDown
            // 
            this.TwitchBitsMinimumUpDown.Location = new System.Drawing.Point(51, 3);
            this.TwitchBitsMinimumUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.TwitchBitsMinimumUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TwitchBitsMinimumUpDown.Name = "TwitchBitsMinimumUpDown";
            this.TwitchBitsMinimumUpDown.Size = new System.Drawing.Size(47, 20);
            this.TwitchBitsMinimumUpDown.TabIndex = 5;
            this.TwitchBitsMinimumUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ChatCommandTabPage
            // 
            this.ChatCommandTabPage.Controls.Add(this.ChatCommandFlowLayout);
            this.ChatCommandTabPage.Location = new System.Drawing.Point(4, 22);
            this.ChatCommandTabPage.Name = "ChatCommandTabPage";
            this.ChatCommandTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ChatCommandTabPage.Size = new System.Drawing.Size(592, 574);
            this.ChatCommandTabPage.TabIndex = 6;
            this.ChatCommandTabPage.Text = "command";
            this.ChatCommandTabPage.UseVisualStyleBackColor = true;
            // 
            // ChatCommandFlowLayout
            // 
            this.ChatCommandFlowLayout.Controls.Add(this.TwitchChatCommandCheckBox);
            this.ChatCommandFlowLayout.Controls.Add(this.TwitchCommandModEnableCheckBox);
            this.ChatCommandFlowLayout.Controls.Add(this.ChatCommandTextFlowLayout);
            this.ChatCommandFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatCommandFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ChatCommandFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.ChatCommandFlowLayout.Name = "ChatCommandFlowLayout";
            this.ChatCommandFlowLayout.Size = new System.Drawing.Size(586, 568);
            this.ChatCommandFlowLayout.TabIndex = 14;
            // 
            // TwitchChatCommandCheckBox
            // 
            this.TwitchChatCommandCheckBox.AutoSize = true;
            this.TwitchChatCommandCheckBox.Location = new System.Drawing.Point(3, 3);
            this.TwitchChatCommandCheckBox.Name = "TwitchChatCommandCheckBox";
            this.TwitchChatCommandCheckBox.Size = new System.Drawing.Size(58, 17);
            this.TwitchChatCommandCheckBox.TabIndex = 3;
            this.TwitchChatCommandCheckBox.Text = "enable";
            this.TwitchChatCommandCheckBox.UseVisualStyleBackColor = true;
            // 
            // TwitchCommandModEnableCheckBox
            // 
            this.TwitchCommandModEnableCheckBox.AutoSize = true;
            this.TwitchCommandModEnableCheckBox.Location = new System.Drawing.Point(3, 26);
            this.TwitchCommandModEnableCheckBox.Name = "TwitchCommandModEnableCheckBox";
            this.TwitchCommandModEnableCheckBox.Size = new System.Drawing.Size(66, 17);
            this.TwitchCommandModEnableCheckBox.TabIndex = 9;
            this.TwitchCommandModEnableCheckBox.Text = "for mods";
            this.TwitchCommandModEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // ChatCommandTextFlowLayout
            // 
            this.ChatCommandTextFlowLayout.Controls.Add(this.ChatCommandLabel);
            this.ChatCommandTextFlowLayout.Controls.Add(this.TwitchChatCommandTextBox);
            this.ChatCommandTextFlowLayout.Location = new System.Drawing.Point(3, 49);
            this.ChatCommandTextFlowLayout.Name = "ChatCommandTextFlowLayout";
            this.ChatCommandTextFlowLayout.Size = new System.Drawing.Size(289, 47);
            this.ChatCommandTextFlowLayout.TabIndex = 10;
            // 
            // ChatCommandLabel
            // 
            this.ChatCommandLabel.AutoSize = true;
            this.ChatCommandLabel.Location = new System.Drawing.Point(3, 0);
            this.ChatCommandLabel.Name = "ChatCommandLabel";
            this.ChatCommandLabel.Size = new System.Drawing.Size(77, 13);
            this.ChatCommandLabel.TabIndex = 11;
            this.ChatCommandLabel.Text = "chat command";
            // 
            // TwitchChatCommandTextBox
            // 
            this.TwitchChatCommandTextBox.Location = new System.Drawing.Point(86, 3);
            this.TwitchChatCommandTextBox.MaxLength = 32;
            this.TwitchChatCommandTextBox.Name = "TwitchChatCommandTextBox";
            this.TwitchChatCommandTextBox.Size = new System.Drawing.Size(181, 20);
            this.TwitchChatCommandTextBox.TabIndex = 6;
            this.TwitchChatCommandTextBox.Text = "!shuffle";
            // 
            // RedemptionTabPage
            // 
            this.RedemptionTabPage.Controls.Add(this.TwitchRedeemFlowLayout);
            this.RedemptionTabPage.Location = new System.Drawing.Point(4, 22);
            this.RedemptionTabPage.Name = "RedemptionTabPage";
            this.RedemptionTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.RedemptionTabPage.Size = new System.Drawing.Size(592, 574);
            this.RedemptionTabPage.TabIndex = 7;
            this.RedemptionTabPage.Text = "redemption";
            this.RedemptionTabPage.UseVisualStyleBackColor = true;
            // 
            // TwitchRedeemFlowLayout
            // 
            this.TwitchRedeemFlowLayout.Controls.Add(this.TwitchRedemptionEnableCheckBox);
            this.TwitchRedeemFlowLayout.Controls.Add(this.RedemptionTitleFlowLayout);
            this.TwitchRedeemFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TwitchRedeemFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.TwitchRedeemFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.TwitchRedeemFlowLayout.Name = "TwitchRedeemFlowLayout";
            this.TwitchRedeemFlowLayout.Size = new System.Drawing.Size(586, 568);
            this.TwitchRedeemFlowLayout.TabIndex = 15;
            // 
            // TwitchRedemptionEnableCheckBox
            // 
            this.TwitchRedemptionEnableCheckBox.AutoSize = true;
            this.TwitchRedemptionEnableCheckBox.Location = new System.Drawing.Point(3, 3);
            this.TwitchRedemptionEnableCheckBox.Name = "TwitchRedemptionEnableCheckBox";
            this.TwitchRedemptionEnableCheckBox.Size = new System.Drawing.Size(58, 17);
            this.TwitchRedemptionEnableCheckBox.TabIndex = 10;
            this.TwitchRedemptionEnableCheckBox.Text = "enable";
            this.TwitchRedemptionEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // RedemptionTitleFlowLayout
            // 
            this.RedemptionTitleFlowLayout.Controls.Add(this.TwitchRedemptionTitleTextBox);
            this.RedemptionTitleFlowLayout.Controls.Add(this.RedemptionTitleLabel);
            this.RedemptionTitleFlowLayout.Location = new System.Drawing.Point(3, 26);
            this.RedemptionTitleFlowLayout.Name = "RedemptionTitleFlowLayout";
            this.RedemptionTitleFlowLayout.Size = new System.Drawing.Size(297, 52);
            this.RedemptionTitleFlowLayout.TabIndex = 13;
            // 
            // TwitchRedemptionTitleTextBox
            // 
            this.TwitchRedemptionTitleTextBox.Location = new System.Drawing.Point(3, 3);
            this.TwitchRedemptionTitleTextBox.Name = "TwitchRedemptionTitleTextBox";
            this.TwitchRedemptionTitleTextBox.Size = new System.Drawing.Size(161, 20);
            this.TwitchRedemptionTitleTextBox.TabIndex = 11;
            this.TwitchRedemptionTitleTextBox.Text = "SHUFFLE_GAME_PLEASE";
            // 
            // RedemptionTitleLabel
            // 
            this.RedemptionTitleLabel.AutoSize = true;
            this.RedemptionTitleLabel.Location = new System.Drawing.Point(170, 0);
            this.RedemptionTitleLabel.Name = "RedemptionTitleLabel";
            this.RedemptionTitleLabel.Size = new System.Drawing.Size(88, 13);
            this.RedemptionTitleLabel.TabIndex = 12;
            this.RedemptionTitleLabel.Text = "redemption name";
            // 
            // ShufflerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.ShufflerTabControl);
            this.Name = "ShufflerUserControl";
            this.Size = new System.Drawing.Size(600, 600);
            this.ShufflerTabControl.ResumeLayout(false);
            this.MainTabPage.ResumeLayout(false);
            this.MainTabFlowLayout.ResumeLayout(false);
            this.TransportButtonFlowLayout.ResumeLayout(false);
            this.SeedFlowLayout.ResumeLayout(false);
            this.SeedFlowLayout.PerformLayout();
            this.AlgorithmFlowLayout.ResumeLayout(false);
            this.AlgorithmFlowLayout.PerformLayout();
            this.RngFlowLayout.ResumeLayout(false);
            this.RngFlowLayout.PerformLayout();
            this.CooldownFlowLayout.ResumeLayout(false);
            this.CooldownFlowLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CooldownUpDown)).EndInit();
            this.TimerTabPage.ResumeLayout(false);
            this.TimerTabFlowLayout.ResumeLayout(false);
            this.TimerTabFlowLayout.PerformLayout();
            this.TimerMinFlowLayout.ResumeLayout(false);
            this.TimerMinFlowLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimerMinTimeUpDown)).EndInit();
            this.TimerMaxFlowLayout.ResumeLayout(false);
            this.TimerMaxFlowLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimerMaxTimeUpDown)).EndInit();
            this.FileTabPage.ResumeLayout(false);
            this.FileFlowLayout.ResumeLayout(false);
            this.FileFlowLayout.PerformLayout();
            this.SubTabPage.ResumeLayout(false);
            this.SubTabFlowLayout.ResumeLayout(false);
            this.SubTabFlowLayout.PerformLayout();
            this.TwitchSubFlowLayout.ResumeLayout(false);
            this.TwitchSubFlowLayout.PerformLayout();
            this.PollTabPage.ResumeLayout(false);
            this.PollFlowLayout.ResumeLayout(false);
            this.PollFlowLayout.PerformLayout();
            this.PollOption12FlowLayout.ResumeLayout(false);
            this.PollOption1FlowLayout.ResumeLayout(false);
            this.PollOption1FlowLayout.PerformLayout();
            this.PollOption2FlowLayout.ResumeLayout(false);
            this.PollOption2FlowLayout.PerformLayout();
            this.PollOption34FlowLayout.ResumeLayout(false);
            this.PollOption3FlowLayout.ResumeLayout(false);
            this.PollOption3FlowLayout.PerformLayout();
            this.PollOption4FlowLayout.ResumeLayout(false);
            this.PollOption4FlowLayout.PerformLayout();
            this.PollOption5FlowLayout.ResumeLayout(false);
            this.PollOption5FlowLayout.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChannelPointVoteCostUpDown)).EndInit();
            this.PollDurationFlowLayout.ResumeLayout(false);
            this.PollDurationFlowLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PollDurationUpDown)).EndInit();
            this.PollButtonFlowLayout.ResumeLayout(false);
            this.BitsTabPage.ResumeLayout(false);
            this.BitsFlowLayout.ResumeLayout(false);
            this.BitsFlowLayout.PerformLayout();
            this.BitsAmountFlowLayout.ResumeLayout(false);
            this.BitsAmountFlowLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TwitchBitsMinimumUpDown)).EndInit();
            this.ChatCommandTabPage.ResumeLayout(false);
            this.ChatCommandFlowLayout.ResumeLayout(false);
            this.ChatCommandFlowLayout.PerformLayout();
            this.ChatCommandTextFlowLayout.ResumeLayout(false);
            this.ChatCommandTextFlowLayout.PerformLayout();
            this.RedemptionTabPage.ResumeLayout(false);
            this.TwitchRedeemFlowLayout.ResumeLayout(false);
            this.TwitchRedeemFlowLayout.PerformLayout();
            this.RedemptionTitleFlowLayout.ResumeLayout(false);
            this.RedemptionTitleFlowLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public TabControl ShufflerTabControl;
        public TabPage MainTabPage;
        public TabPage TimerTabPage;
        public CheckBox ShowProgressBarCheckBox;
        public ProgressBar TimerProgressBar;
        public CheckBox TimerEnableCheckBox;
        public NumericUpDown TimerMaxTimeUpDown;
        public Label MaxTimeLabel;
        public NumericUpDown TimerMinTimeUpDown;
        public Label MinTimeLabel;
        public TabPage FileTabPage;
        public TextBox FileTouchPathDisplayTextBox;
        public Button FileTouchDialogButton;
        public CheckBox FileTouchEnableCheckBox;
        public TabPage SubTabPage;
        public ComboBox TwitchSubTierComboBox;
        public CheckBox TwitchSubEnableCheckBox;
        public Button ResumeButton;
        public ComboBox RngComboBox;
        public Label SeedTextBoxLabel;
        public Button ForceShuffleButton;
        public TextBox SeedTextBox;
        public Label AlgorithmLabel;
        public Button PauseUnpauseButton;
        public Label RNGLabel;
        public ComboBox AlgorithmComboBox;
        public Button StartNewButton;
        public NumericUpDown CooldownUpDown;
        public NumericUpDown TwitchBitsMinimumUpDown;
        public CheckBox TwitchBitsEnableCheckBox;
        public CheckBox TwitchChatCommandCheckBox;
        public CheckBox TwitchCommandModEnableCheckBox;
        public TextBox TwitchChatCommandTextBox;
        public CheckBox TwitchRedemptionEnableCheckBox;
        public TextBox TwitchRedemptionTitleTextBox;
        public Label TwitchSubTierLabel;
        public FlowLayoutPanel TransportButtonFlowLayout;
        public FlowLayoutPanel AlgorithmFlowLayout;
        public FlowLayoutPanel SeedFlowLayout;
        public FlowLayoutPanel RngFlowLayout;
        public FlowLayoutPanel CooldownFlowLayout;
        public Label CooldownLabel;
        public FlowLayoutPanel MainTabFlowLayout;
        public FlowLayoutPanel TimerMaxFlowLayout;
        public FlowLayoutPanel TimerMinFlowLayout;
        public FlowLayoutPanel TimerTabFlowLayout;
        public FlowLayoutPanel TwitchSubFlowLayout;
        public FlowLayoutPanel SubTabFlowLayout;
        public TabPage PollTabPage;
        public FlowLayoutPanel PollFlowLayout;
        public Button PollSendButton;
        public ComboBox PollOption1ComboBox;
        public ComboBox PollOption2ComboBox;
        public ComboBox PollOption3ComboBox;
        public ComboBox PollOption4ComboBox;
        public ComboBox PollOption5ComboBox;
        public TextBox PollTitleTextBox;
        public NumericUpDown PollDurationUpDown;
        public CheckBox ChannelPointVotingEnableCheckBox;
        public NumericUpDown ChannelPointVoteCostUpDown;
        public Label ChannelPointVotingCostLabel;
        public FlowLayoutPanel flowLayoutPanel1;
        public FlowLayoutPanel PollDurationFlowLayout;
        public Label PollDurationLabel;
        public CheckBox PollOption1EnableCheckBox;
        public CheckBox PollOption2EnableCheckBox;
        public CheckBox PollOption3EnableCheckBox;
        public CheckBox PollOption4EnableCheckBox;
        public CheckBox PollOption5EnableCheckBox;
        public FlowLayoutPanel PollOption1FlowLayout;
        public FlowLayoutPanel PollOption2FlowLayout;
        public FlowLayoutPanel PollOption3FlowLayout;
        public FlowLayoutPanel PollOption4FlowLayout;
        public FlowLayoutPanel PollOption5FlowLayout;
        public FlowLayoutPanel PollButtonFlowLayout;
        public Button PollEndButton;
        public Button PollClearButton;
        public TabPage BitsTabPage;
        public TabPage ChatCommandTabPage;
        public FlowLayoutPanel FileFlowLayout;
        public FlowLayoutPanel BitsFlowLayout;
        public FlowLayoutPanel ChatCommandFlowLayout;
        public TabPage RedemptionTabPage;
        public FlowLayoutPanel ChatCommandTextFlowLayout;
        public Label ChatCommandLabel;
        public FlowLayoutPanel BitsAmountFlowLayout;
        public Label BitsAmountLabel;
        public FlowLayoutPanel TwitchRedeemFlowLayout;
        public FlowLayoutPanel RedemptionTitleFlowLayout;
        public Label RedemptionTitleLabel;
        public FlowLayoutPanel PollOption12FlowLayout;
        public FlowLayoutPanel PollOption34FlowLayout;
        public Button GameFinishButton;
    }
}
