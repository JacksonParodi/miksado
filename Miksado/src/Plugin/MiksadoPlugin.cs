using BizHawk.Client.Common;
using Miksado.Misc;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib.EventSub.Core.EventArgs.Channel;

namespace Miksado.Plugin
{
    abstract class MiksadoPlugin(Logger.Logger logger, ApiContainer APIs, PluginConfig? pluginConfig)
    {
        protected readonly Logger.Logger Logger = logger;
        protected ApiContainer APIs = APIs;
        public string PluginName = "_DEFAULT_PLUGIN_NAME_";
        public bool Enabled = false;
        public UserControl BaseUserControl = null!;
        public PluginConfig PluginConfig = pluginConfig ?? null!;
        public bool Initialized = false;

        public event Action? PluginConfigChanged;
        public void FirePluginConfigChanged()
        {
            PluginConfigChanged?.Invoke();
        }

        public virtual void UpdateUI()
        {
            return;
        }
        public virtual void OnRestart()
        {
            return;
        }
        public virtual void OnUpdateAfter()
        {
            return;
        }
        async public virtual Task OnChannelChatMessage(ChannelChatMessageArgs e)
        {
            await Task.CompletedTask;
        }
        async public virtual Task OnChannelChatNotification(ChannelChatNotificationArgs e)
        {
            await Task.CompletedTask;
        }
        async public virtual Task OnRedemptionAdd(ChannelPointsCustomRewardRedemptionArgs e)
        {
            await Task.CompletedTask;
        }
        async public virtual Task OnBitsUse(ChannelBitsUseArgs e)
        {
            await Task.CompletedTask;
        }
        async public virtual Task OnPollEnd(ChannelPollEndArgs e)
        {
            await Task.CompletedTask;
        }
    }
}
