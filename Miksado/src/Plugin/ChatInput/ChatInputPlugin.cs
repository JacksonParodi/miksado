using BizHawk.Client.Common;
using Miksado.Misc;

namespace Miksado.Plugin.ChatInput
{
    internal class ChatInputPlugin : MiksadoPlugin
    {
        public ChatInputUserControl UserControl => (ChatInputUserControl)BaseUserControl;

        public ChatInputPlugin(Logger.Logger logger, ApiContainer APIs, PluginConfig? pluginConfig) : base(logger, APIs, pluginConfig)
        {
            BaseUserControl = new ChatInputUserControl();
            PluginName = "chat input";
            Logger.Debug("ChatInputPlugin initialized");
        }
    }
}
