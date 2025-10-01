using System.Windows.Forms;

namespace Miksado.Logger
{
    internal class Logger
    {
        public enum LogLevel
        {
            Info,
            Debug,
            Error
        }

        private readonly TextBox _console;
        public LogLevel CurrentLogLevel { get; set; } = LogLevel.Debug;


        public Logger(TextBox console)
        {
            _console = console;
            _console.Clear();

            //_console.Multiline = true;
            //_console.ScrollBars = ScrollBars.Vertical;
            //_console.WordWrap = false;
            //_console.ReadOnly = true;
        }

        private void WriteConsoleMultiline(string text)
        {
            if (_console.InvokeRequired)
            {
                _console.Invoke(new System.Action(() => WriteConsoleMultiline(text)));
                return;
            }

            if (_console.Text.Length > 0)
            {
                _console.AppendText("\r\n");
            }

            _console.AppendText(text);
        }

        public void Info(string text)
        {
            if (CurrentLogLevel == LogLevel.Info || CurrentLogLevel == LogLevel.Debug)
            {
                WriteConsoleMultiline($"[INFO] {text}");
            }
        }

        public void Debug(string text)
        {
            if (CurrentLogLevel == LogLevel.Debug)
            {
                WriteConsoleMultiline($"[DEBUG] {text}");
            }
        }

        public void Error(string text)
        {
            WriteConsoleMultiline($"[ERROR] {text}");
        }

        public void ScrollToCaret()
        {
            if (_console.InvokeRequired)
            {
                _console.Invoke(new System.Action(ScrollToCaret));
                return;
            }
            _console.SelectionStart = _console.Text.Length;
            _console.ScrollToCaret();
        }
    }
}
