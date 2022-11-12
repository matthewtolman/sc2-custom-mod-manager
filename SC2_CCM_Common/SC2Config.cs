using System.Collections.Immutable;
using System.Text.Json;

#if WINDOWS
using Microsoft.Win32;
#endif

namespace SC2_CCM_Common
{
    /// <summary>
    /// Represents Configuration for SC2 CCM
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class SC2Config
    {
        /// <summary>
        /// Default legacy config path
        /// </summary>
        private static string LegacyConfigPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.Combine("SC2CCM", "SC2CCM.txt"));
        
        /// <summary>
        /// Default new config path
        /// </summary>
        private static string NewConfigPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.Combine("SC2CCM", "SC2CCM.json"));

        /// <summary>
        /// Current legacy config path for SC2Config object
        /// </summary>
        private readonly string _legacyConfigPath;
        
        /// <summary>
        /// Current new config path for SC2Config object
        /// </summary>
        private readonly string _newConfigPath;
        
        /// <summary>
        /// Underlying data for SC2Config
        /// </summary>
        private readonly SC2ConfigData _data;

        /// <summary>
        /// StarCraft II Executable directory
        /// </summary>
        public string StarCraft2Dir => Path.GetDirectoryName(_data.StarCraft2Exe)!;
        
        /// <summary>
        /// StarCraft II Executable
        /// </summary>
        public string StarCraft2Exe
        {
            get => _data.StarCraft2Exe;
            set
            {
                _data.StarCraft2Exe = value;
                Save();
            }
        }

        /// <summary>
        /// Convenience wrapper around mod selection info which also does automatic saves on change
        /// </summary>
        private ImmutableDictionary<CampaignType, ImmutableDictionary<String, String?>> ModSelectionInfo
        {
            get => _data.ModSelectionInfo;
            set
            {
                _data.ModSelectionInfo = value;
                Save();
            }
        }
        
        /// <summary>
        /// Constructor for SC2Config
        /// </summary>
        /// <param name="data">Underlying data</param>
        /// <param name="legacyConfigPath">Legacy config file path</param>
        /// <param name="newConfigPath">New config file path</param>
        private SC2Config(SC2ConfigData data, string legacyConfigPath, string newConfigPath)
        {
            _data = data;
            _legacyConfigPath = legacyConfigPath;
            _newConfigPath = newConfigPath;
        }

        /// <summary>
        /// Sets the currently loaded campaign mod for the given campaign type
        /// </summary>
        /// <param name="campaignType"></param>
        /// <param name="modName"></param>
        public void SetLoadedMod(CampaignType campaignType, string? modName)
        {
            ModSelectionInfo = ModSelectionInfo.SetItem(campaignType,
                ModSelectionInfo[campaignType].SetItem("mod", modName));
        }

        /// <summary>
        /// Returns whether campaign mods are enabled for the given campaign type
        /// </summary>
        /// <param name="campaignType"></param>
        /// <returns></returns>
        public bool ModsEnabled(CampaignType campaignType)
        {
            return ModSelectionInfo[campaignType]["enabled"] == "on";
        }

        /// <summary>
        /// Sets whether mods are enabled for the given campaign type
        /// </summary>
        /// <param name="campaignType"></param>
        /// <param name="enabled"></param>
        public void SetModEnabled(CampaignType campaignType, bool enabled)
        {
            ModSelectionInfo = ModSelectionInfo.SetItem(campaignType,
                ModSelectionInfo[campaignType].SetItem("enabled", enabled ? "on" : "off"));
        }

        /// <summary>
        /// Gets the loaded mod for the given campaign type
        /// </summary>
        /// <param name="campaignType"></param>
        /// <returns></returns>
        public string? GetLoadedMod(CampaignType campaignType)
        {
            return ModSelectionInfo[campaignType]["mod"];
        }
        
        /// <summary>
        /// Returns a new, blank configuration
        /// </summary>
        /// <param name="starCraft2Path">Path to StarCraft II Executable</param>
        /// <param name="legacyConfigPath">Path for legacy configuration file</param>
        /// <param name="newConfigPath">Path for new configuration file</param>
        /// <returns></returns>
        private static SC2Config NewConfig(string starCraft2Path, string legacyConfigPath, string newConfigPath)
        {
            ImmutableDictionary<string, string?> MakeDictEntry() => ImmutableDictionary.Create<String, String?>().Add("enabled", "off").Add("mod", null);

            var blankConfig = ImmutableDictionary.Create<CampaignType, ImmutableDictionary<String, String?>>()
                .Add(CampaignType.WingsOfLiberty, MakeDictEntry())
                .Add(CampaignType.HeartOfTheSwarm, MakeDictEntry())
                .Add(CampaignType.LegacyOfTheVoid, MakeDictEntry())
                .Add(CampaignType.NovaCovertOps, MakeDictEntry());

            var data = new SC2ConfigData
            {
                StarCraft2Exe = starCraft2Path,
                ModSelectionInfo = blankConfig
            };

            return new SC2Config(data, legacyConfigPath, newConfigPath);
        }

        /// <summary>
        /// Saves the current configuration
        /// </summary>
        private void Save()
        {
            try
            {
                File.WriteAllText(_legacyConfigPath, StarCraft2Exe);
                File.WriteAllText(_newConfigPath, JsonSerializer.Serialize(_data));
                Log.Logger.Information("Saved config. Legacy: {LegacyPath}, New: {NewPath}", _legacyConfigPath, _newConfigPath);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Could not save config!");
                Console.WriteLine($"Unable to save! {e.Message}; {e.StackTrace}");
            }
        }

        /// <summary>
        /// Initializes a blank configuration and saves it to disk. Also handles auto-detecting the StarCraft II executable
        /// </summary>
        /// <param name="fallbackPathFinder">Fallback path finder in case auto-detect fails to find StarCraft II</param>
        /// <param name="legacyPath">Legacy configuration file path</param>
        /// <param name="newPath">New configuration file path</param>
        /// <returns>Configuration object</returns>
        /// <exception cref="ModManagerException"></exception>
        private static SC2Config InitBlankConfig(string legacyPath, string newPath)
        {
            Log.Logger.Warning("Initializing from blank config");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(legacyPath)!);
            }
            catch (IOException e)
            {
                Log.Logger.Error(e, "Could not create configuration file!");
                throw new ModManagerException("Unable to create configuration file/folder\nTry running this as administrator.");
            }
#if WINDOWS2
            try
            {
                using RegistryKey? registryKey = Registry.LocalMachine.OpenSubKey("Software\\Classes\\Blizzard.SC2Save\\shell\\open\\command");
                if (registryKey != null)
                {
                    var obj = registryKey.GetValue(null);
                    if (obj != null)
                    {
                        string directoryName = Path.GetDirectoryName(Path.GetDirectoryName(obj.ToString().Replace(" \"%1\"", "").Trim('"')))!;
                        var config = NewConfig(directoryName + "\\StarCraft II.exe", legacyPath, newPath);
                        config.Save();
                        return config;
                    }
                }
            }
            catch (Exception ex)
            {
                var config = NewConfig("C:\\Program Files (x86)\\StarCraft II\\StarCraft II.exe", legacyPath, newPath);
                config.Save();
                return config;
            }
#else
            var macOsPath = "/Applications/StarCraft II/StarCraft II.app";
            if (Directory.Exists(macOsPath))
            {
                var config = NewConfig(macOsPath,legacyPath, newPath);
                config.Save();
                return config;
            }
            else if (Directory.Exists("/Applications/StarCraft 2/StarCraft II.app"))
            {
                var config = NewConfig("/Applications/StarCraft 2/StarCraft II.app",legacyPath, newPath);
                config.Save();
                return config;
            }
            else if (Directory.Exists("/Applications/StarCraft 2/StarCraft 2.app"))
            {
                var config = NewConfig("/Applications/StarCraft 2/StarCraft 2.app",legacyPath, newPath);
                config.Save();
                return config;
            }
#endif

            Log.Logger.Warning("Could not find default StarCraft II!");
            var cfg = NewConfig("", legacyPath, newPath);
            cfg.Save();
            return cfg;
        }

        /// <summary>
        /// Loads the SC2CCM Config file and returns an object representing that file
        /// Will create a new config and auto-detect StarCraft II if needed
        /// </summary>
        /// <param name="fallbackPathFinder">Fallback method to call in case can't auto-detect StarCraft II config</param>
        /// <param name="legacyPath">Legacy config file path</param>
        /// <param name="newPath">New config file path</param>
        /// <returns></returns>
        public static SC2Config Load(string? legacyPath = null, string? newPath = null)
        {
            var legacyConfigPath = legacyPath ?? LegacyConfigPath;
            var newConfigPath = newPath ?? NewConfigPath;
            
            if (!Directory.Exists(Path.GetDirectoryName(legacyConfigPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(legacyConfigPath)!);
            }
            if (!Directory.Exists(Path.GetDirectoryName(newConfigPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newConfigPath)!);
            }

            if (!File.Exists(legacyConfigPath))
            {
                return InitBlankConfig(legacyPath: legacyConfigPath, newPath: newConfigPath);
            }
            else if (!File.Exists(newConfigPath))
            {
                return MigrateLegacyConfig(legacyPath: legacyConfigPath, newPath: newConfigPath);
            }
            else
            {
                var config = FromFile(newConfigPath, legacyConfigPath);
                
                // If we failed to load our config file, just get a new, blank config
                if (config != null)
                {
                    Log.Logger.Information("Loaded config from {ConfigPath}", newConfigPath);
                    return config;
                }

                Log.Logger.Error("Unable to load config at {ConfigPath}. Resetting Config and retrying!", newConfigPath);
                File.Delete(legacyConfigPath);
                File.Delete(newConfigPath);
                return InitBlankConfig(legacyConfigPath, newConfigPath);
            }
        }

        /// <summary>
        /// Creates an SC2Config from a JSON file
        /// </summary>
        /// <param name="newConfigPath">Path to the JSON file</param>
        /// <param name="legacyConfigPath">Path to the legacy config file (if not provided, will do best guess)</param>
        /// <returns></returns>
        private static SC2Config? FromFile(string newConfigPath, string? legacyConfigPath = null)
        {
            try
            {
                var text = File.ReadAllText(newConfigPath);
                var data = JsonSerializer.Deserialize<SC2ConfigData>(text);
                if (data == null)
                {
                    Log.Logger.Error("Unable to parse JSON into SC2ConfigData! {Json}", text);
                    return null;
                }

                legacyConfigPath ??= Path.Combine(Path.GetDirectoryName(newConfigPath)!, "SC2CCM.txt");

                return new SC2Config(data, newConfigPath, legacyConfigPath);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Unable to load config from {ConfigPath}!", newConfigPath);
                return null;
            }
        }

        /// <summary>
        /// Migrates the legacy configuration txt-only file to the newer JSON file.
        /// Legacy file used for compatability with GiantGrantGames CCM (for future Windows port of this app).
        /// </summary>
        /// <param name="fallbackPathFinder"></param>
        /// <param name="legacyPath"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        private static SC2Config MigrateLegacyConfig(string legacyPath, string newPath)
        {
            Log.Logger.Warning("Migrating from legacy config");
            string str = File.ReadLines(legacyPath).First();
#if WINDOWS
            if (!File.Exists(str))
#else
            if (!Directory.Exists(str))
#endif
            {
                // If the config is corrupted, clear it and try again
                Log.Logger.Error("Bad StarCraft II location detected in legacy config! Resetting config {BadPath}", str);
                File.Delete(legacyPath);
                return InitBlankConfig(legacyPath, newPath);
            }
            else
            {
                var config = NewConfig(str, legacyPath, newPath);
                config.Save();
                return config;
            }
        }
    }
}

