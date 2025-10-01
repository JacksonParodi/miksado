namespace MiksadoInstaller
{
    public class Program
    {
        private static readonly string MiksadoPath = Path.GetFullPath("miksado");
        private static readonly string GamePath = Path.Combine(MiksadoPath, "game");
        private static readonly string StatePath = Path.Combine(MiksadoPath, "state");
        private static readonly string DataPath = Path.Combine(MiksadoPath, "data");
        private static readonly string LibDir = Path.GetFullPath("miksado_lib");
        private static readonly string ExternalToolsDir = Path.GetFullPath("ExternalTools");
        private static readonly string RunScriptContents = "start .\\EmuHawk.exe --open-ext-tool-dll=Miksado";
        private static readonly string RunScriptPath = Path.GetFullPath("run_miksado.bat");
        private static readonly bool IsDebugMode = true;

        // readme notes:
        // games must be placed in miksado/game
        // extract in BizHawk base directory
        // optionally run with created run_miksado.bat script
        // in Bizhawk, externaltools -> miksado to run
        // specify bizhawk version
        // maybe create ExternalTools if not exists?

        private static void Main()
        {
            if (IsDebugMode)
            {
                Console.WriteLine($"LibDir: {LibDir}");
                Console.WriteLine($"ExternalToolsDir: {ExternalToolsDir}");
            }

            if (!Directory.Exists(LibDir))
            {
                ShowError("miksado_lib directory not found. try unzipping it again in Bizhawk base directory");
            }

            if (!Directory.Exists(ExternalToolsDir))
            {
                ShowError("ExternalTools directory not found. is installer in BizHawk base directory?");
            }

            string[] libFilesToCopy = [.. Directory.GetFiles(LibDir).Where(s => !Path.GetFileName(s).StartsWith("BizHawk"))];

            if (IsDebugMode)
            {
                Console.WriteLine($"{libFilesToCopy.Length} files to copy: ");
                foreach (string f in libFilesToCopy)
                {
                    Console.WriteLine(" - " + f);
                }
            }

            int libFilesCopied = 0;
            foreach (string f in libFilesToCopy)
            {
                string destFile = Path.Combine(ExternalToolsDir, Path.GetFileName(f));

                try
                {
                    File.Copy(f, destFile, true);
                    libFilesCopied++;
                }
                catch (Exception ex)
                {
                    ShowError("failed to copy " + f + " to " + destFile + ": " + ex.Message);
                }
            }

            if (IsDebugMode)
            {
                Console.WriteLine($"copied {libFilesCopied}/{libFilesToCopy.Length} files to {ExternalToolsDir}");
            }
            if (libFilesCopied != libFilesToCopy.Length)
            {
                ShowError("not all files copied successfully. installation failed");
            }

            string[] paths = [MiksadoPath, GamePath, StatePath, DataPath];
            foreach (string path in paths)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                    if (IsDebugMode)
                    {
                        Console.WriteLine("created directory: " + path);
                    }
                }
            }

            try
            {
                File.WriteAllText(RunScriptPath, RunScriptContents);
                if (IsDebugMode)
                {
                    Console.WriteLine("created run script: " + RunScriptPath);
                }
            }
            catch (Exception ex)
            {
                ShowError("failed to create run script: " + ex.Message);
            }

            return;
        }

        private static void ShowError(string e)
        {
            Console.WriteLine("miksado installation failed: ");
            Console.WriteLine(e);
            Console.WriteLine("press any key to exit...");
            Console.ReadLine();
            return;
        }
    }
}