using BizHawk.Client.Common;

namespace Miksado.Plugin.ChatInput
{
    internal class ChatInputPlugin : MiksadoPlugin
    {
        public ChatInputUserControl UserControl => (ChatInputUserControl)BaseUserControl;

        public ChatInputPlugin(Logger.Logger logger, ApiContainer APIs) : base(logger, APIs)
        {
            BaseUserControl = new ChatInputUserControl();
            PluginName = "chat input";
            Logger.Debug("ChatInputPlugin initialized");
        }
    }
}
