using BizHawk.Client.Common;
using Miksado.Misc;
using Miksado.Twitch;

namespace Miksado.Plugin.ChatInput
{
    internal class ChatInputPlugin : MiksadoPlugin
    {
        public ChatInputUserControl UserControl => (ChatInputUserControl)BaseUserControl;

        public ChatInputPlugin(Logger.Logger logger, ApiContainer APIs, TwitchClient TwitchClient, PluginConfig? pluginConfig) : base(logger, APIs, TwitchClient, pluginConfig)
        {
            BaseUserControl = new ChatInputUserControl();
            PluginName = "chat input";
            Logger.Debug("ChatInputPlugin initialized");
        }
    }
}
