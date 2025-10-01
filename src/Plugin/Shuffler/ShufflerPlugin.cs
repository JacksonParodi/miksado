using BizHawk.Client.Common;
using Miksado.Plugin.Shuffler.Random;
using Miksado.Plugin.Shuffler.Shuffle;
using Miksado.Plugin.Shuffler.Trigger;
using Miksado.Twitch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core.EventArgs.Channel;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using MConstant = Miksado.Constant.Constant;

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
        private IRandomNumberGenerator CurrentRng;
        private IShuffleAlgorithm CurrentShuffleAlgorithm;
        private ShuffleTrigger[] CurrentShuffleTriggers;
        private bool ShouldShuffle = false;
        private bool IsShuffling = false;
        private readonly Dictionary<string, string> GameStateMap = [];
        private readonly List<string> AllGames = [];
        private readonly List<string> AllStates = [];
        private string? CurrentGamePath = null;
        private System.Timers.Timer? ShufflerTimer;
        private DateTime lastTimerTargetTime;
        private DateTime nextTimerTargetTime;
        private DateTime lastShuffleTime;

        private ShufflerState _state;
        private ShufflerState GetState() => _state;
        private void SetState(ShufflerState newState) => _state = newState;
        private bool IsActiveSession() => _state == ShufflerState.Active;
        private bool IsInactive() => _state == ShufflerState.Inactive;
        private bool IsPausedSession() => _state == ShufflerState.Paused;

        public ShufflerPlugin(Logger.Logger logger, ApiContainer APIs) : base(logger, APIs)
        {
            // TODO: add random option to each poll option combobox

            BaseUserControl = new ShufflerUserControl();
            SetState(ShufflerState.Inactive);
            PluginName = "game shuffler";

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

            // load games and states
            string[] rawGameFiles = Directory.GetFiles(MConstant.MiksadoGamePath, "*", SearchOption.AllDirectories);
            AllGames = [.. rawGameFiles.Where(filePath => !MConstant.InvalidGameExtensions.Contains(Path.GetExtension(filePath).ToLower()))];

            Logger.Debug($"found {AllGames.Count} valid game files");

            foreach (string gamePath in AllGames)
            {
                string gameFileName = Path.GetFileNameWithoutExtension(gamePath);
                Logger.Debug($"game file: {gameFileName}");
            }

            string[] rawStateFiles = Directory.GetFiles(MConstant.MiksadoStatePath, "*", SearchOption.AllDirectories);
            AllStates = [.. rawStateFiles.Where(filePath => Path.GetFileName(filePath).StartsWith(MConstant.SaveStatePrefix))];

            Logger.Debug($"found {AllStates.Count} valid state files");

            foreach (string gamePath in AllGames)
            {
                foreach (string statePath in AllStates)
                {
                    string gameFileName = Path.GetFileNameWithoutExtension(gamePath);
                    string stateFileName = Path.GetFileNameWithoutExtension(statePath);

                    if (stateFileName.StartsWith(MConstant.SaveStatePrefix) && stateFileName.EndsWith(gameFileName))
                    {
                        GameStateMap[gamePath] = statePath;
                        break;
                    }
                }
            }
            Logger.Debug($"mapped {GameStateMap.Count} game files to state files");

            // basic validate game files
            if (AllGames.Count == 0)
            {
                Logger.Info("no valid game files found, tool will not function");
                UserControl.StartButton.Enabled = false;
                UserControl.ResumeButton.Enabled = false;
                UserControl.ForceShuffleButton.Enabled = false;
                return;
            }

            if (AllGames.Count == 1)
            {
                Logger.Info("only one valid game file found, shuffling will have no effect");
            }

            UserControl.CooldownUpDown.Value = MConstant.DefaultCooldown;

            UserControl.ChannelPointVotingEnableCheckBox.CheckedChanged += (s, e) =>
            {
                UserControl.ChannelPointVoteCostUpDown.Enabled = UserControl.ChannelPointVotingEnableCheckBox.Checked;
            };

            UserControl.StartButton.Click += OnStartButtonClick;
            UserControl.ResumeButton.Click += OnResumeButtonClick;
            UserControl.PauseUnpauseButton.Click += OnPauseUnpauseButtonClick;
            UserControl.ForceShuffleButton.Click += OnForceShuffleButtonClick;

            UpdateUI();
        }

        private void ShuffleGame()
        {
            if (IsShuffling)
            {
                Logger.Debug("already shuffling");
                return;
            }

            if (lastShuffleTime + TimeSpan.FromSeconds((double)UserControl.CooldownUpDown.Value) > DateTime.Now)
            {
                Logger.Info("cooldown active, skipping shuffle");
                return;
            }

            try
            {
                IsShuffling = true;

                // TODO: way to force a certain game to shuffle, from the poll
                // I don't want to call OpenRom from anywhere else
                // I'd rather modify the logic to force a certain next game

                if (APIs.Emulation.GetGameInfo != null)
                {
                    string newStateStatePath = Path.Combine(
                        MConstant.MiksadoStatePath,
                        $"{MConstant.SaveStatePrefix}{Path.GetFileNameWithoutExtension(CurrentGamePath)}.state"
                        );
                    Logger.Debug($"saving state to: {newStateStatePath}");
                    APIs.SaveState.Save(newStateStatePath);
                    if (CurrentGamePath != null)
                    {
                        GameStateMap[CurrentGamePath] = newStateStatePath;
                    }
                }

                string nextGamePath = CurrentShuffleAlgorithm.NextGamePath(CurrentRng, AllGames, CurrentGamePath);
                Logger.Debug($"next game path: {nextGamePath}");
                APIs.EmuClient.OpenRom(nextGamePath);
                lastShuffleTime = DateTime.Now;
                CurrentGamePath = nextGamePath;
                Logger.Debug($"loaded random game: {Path.GetFileNameWithoutExtension(CurrentGamePath)}");

                if (GameStateMap.ContainsKey(CurrentGamePath))
                {
                    string statePath = GameStateMap[CurrentGamePath];
                    Logger.Debug($"loading state: {Path.GetFileName(statePath)}");
                    APIs.SaveState.Load(statePath);
                }

                else
                {
                    Logger.Debug("no save state found for this game, starting fresh");
                }
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

            IsShuffling = false;
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
            UserControl.StartButton.Enabled = !IsActiveSession() && Enabled;

            if (GameStateMap.Count > 0 && !IsActiveSession() && Enabled)
            {
                UserControl.ResumeButton.Enabled = true;
            }
            else
            {
                UserControl.ResumeButton.Enabled = false;
            }

            UserControl.ForceShuffleButton.Enabled = AllGames.Count > 1 && IsActiveSession() && Enabled;

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
            // blank buttons for now
            UserControl.PollSendButton.Enabled = false;
            UserControl.PollEndButton.Enabled = false;
            UserControl.PollClearButton.Enabled = false;

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

        public override void OnRestart()
        {
            Logger.Debug("shuffler plugin restarted");
        }

        public override void OnUpdateAfter()
        {
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

        private void OnStartButtonClick(object sender, System.EventArgs e)
        {
            foreach (string file in Directory.GetFiles(MConstant.MiksadoStatePath))
            {
                File.Delete(file);
            }
            foreach (string dir in Directory.GetDirectories(MConstant.MiksadoStatePath))
            {
                Directory.Delete(dir, true);
            }

            Logger.Debug("start button clicked");
            // it crashes here, there's something wrong with using the API container outside the entry point class
            Logger.Debug($"shuffler current sysid: {APIs.Emulation.GetSystemId()}");
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
                //Logger.Info($"bits used: {usedAmount} (trigger: {triggerAmount})");
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
        async public override Task OnPollEnd(ChannelPollEndArgs e)
        {
            if (!IsActiveSession() || !Enabled)
            {
                await Task.CompletedTask;
                return;
            }

            Logger.Debug("OnPollEnd called");
            await Task.CompletedTask;
        }
    }
}
