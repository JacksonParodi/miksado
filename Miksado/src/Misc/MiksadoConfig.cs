using System.IO;
using System.Linq;
using System.Text.Json;

namespace Miksado.Misc
{
    public class MiksadoConfig
    {
        public string Version { get; set; } = "_NOT_SET_";
        public PluginConfig[] PluginConfigs { get; set; } = [];

        public MiksadoConfig()
        {
            Version = Helper.VersionToString(Constant.MajorVersion, Constant.MinorVersion, Constant.PatchVersion);
            Save();
        }

        public void Save()
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true
            };

            // Serialize each plugin config with its actual type
            object[] configsToSerialize = [.. PluginConfigs.Select(config => new
            {
                Type = config.GetType().Name,
                Config = JsonSerializer.SerializeToElement(config, config.GetType(), options)
            })];

            object wrapper = new
            {
                this.Version,
                PluginConfigs = configsToSerialize
            };

            string serialized = JsonSerializer.Serialize(wrapper, options);
            File.WriteAllText(Constant.MConfigFilePath, serialized);
        }

        public static MiksadoConfig Load()
        {
            string configPath = Path.Combine(Constant.MDataDirPath, "config.json");
            if (!File.Exists(configPath))
            {
                return new MiksadoConfig();
            }
            string content = File.ReadAllText(configPath);
            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            try
            {
                MiksadoConfig? config = JsonSerializer.Deserialize<MiksadoConfig>(content, options);
                if (config == null)
                {
                    return new MiksadoConfig();
                }
                return config;
            }
            catch
            {
                return new MiksadoConfig();
            }
        }
    }
}
