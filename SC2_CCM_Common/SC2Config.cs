using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SC2_CCM_Common
{
    public class SC2Config
    {
        private static string _legacyConfigPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.Combine("SC2CCM", "SC2CCM.txt"));
        
        private static string _newConfigPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.Combine("SC2CCM", "SC2CCM.json"));

        private SC2ConfigData _data = new SC2ConfigData();

        public string StarCraft2Dir => Path.GetDirectoryName(_data.StarCraft2Exe)!;
        public string StarCraft2Exe => _data.StarCraft2Exe;

        public ImmutableDictionary<CampaignType, ImmutableDictionary<String, String?>> ModSelectionInfo
        {
            get => _data.ModSelectionInfo;
            private set
            {
                _data.ModSelectionInfo = value;
                Save();
            }
        }
        
        private SC2Config(SC2ConfigData data)
        {
            _data = data;
        }
        

        private static SC2Config NewConfig(string path)
        {
            ImmutableDictionary<string, string?> MakeDictEntry() => ImmutableDictionary.Create<String, String?>().Add("enabled", "off").Add("mod", null);

            var blankConfig = ImmutableDictionary.Create<CampaignType, ImmutableDictionary<String, String?>>()
                .Add(CampaignType.WingsOfLiberty, MakeDictEntry())
                .Add(CampaignType.HeartOfTheSwarm, MakeDictEntry())
                .Add(CampaignType.LegacyOfTheVoid, MakeDictEntry())
                .Add(CampaignType.NovaCovertOps, MakeDictEntry());

            var data = new SC2ConfigData();
            data.StarCraft2Exe = path;
            data.ModSelectionInfo = blankConfig;
            
            return (new SC2Config(data));
        }

        public bool Save()
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
                return false;
            }

            return true;
        }

        private static async Task<SC2Config> InitBlankConfig(Func<Task<string>> fallbackPathFinder, int retryCount)
        {
            Log.Logger.Warning("Initializing from blank config");
            if (!File.Exists(_legacyConfigPath))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_legacyConfigPath)!);
                }
                catch (IOException)
                {
                    throw new ModManagerException("Unable to create configuration file/folder\nTry running this as administrator.");
                }
#if WINDOWS
                try
                {
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Classes\\Blizzard.SC2Save\\shell\\open\\command"))
                    {
                        if (registryKey != null)
                        {
                            object obj = registryKey.GetValue((string) null);
                            if (obj != null)
                            {
                                string directoryName = Path.GetDirectoryName(Path.GetDirectoryName(obj.ToString().Replace(" \"%1\"", "").Trim('"')));
                                NewConfig(directoryName + "\\StarCraft II.exe").Save();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    NewConfig("C:\\Program Files (x86)\\StarCraft II\\StarCraft II.exe").Save();
                }
#else
                var macOsPath = "/Applications/StarCraft II/StarCraft II.app";
                if (Directory.Exists(macOsPath))
                {
                    NewConfig(macOsPath).Save();
                }
#endif
            }

            if (!File.Exists(_legacyConfigPath))
            {
                Log.Logger.Warning("Could not find default StarCraft II!");
                NewConfig(await fallbackPathFinder()).Save();
            }

            return await MigrateLegacyConfig(fallbackPathFinder, retryCount);
        }

        public static Task<SC2Config> Load(Func<Task<string>> fallbackPathFinder, int retryCount = 0)
        {
            if (!Directory.Exists(Path.GetDirectoryName(_legacyConfigPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_legacyConfigPath)!);
            }
            if (!Directory.Exists(Path.GetDirectoryName(_newConfigPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_newConfigPath)!);
            }

            if (!File.Exists(_legacyConfigPath))
            {
                return InitBlankConfig(fallbackPathFinder, retryCount);
            }
            else if (!File.Exists(_newConfigPath))
            {
                return MigrateLegacyConfig(fallbackPathFinder, retryCount);
            }
            else
            {
                var config = FromFile(_newConfigPath);
                
                // If we failed to load our config file, just get a new, blank config
                if (config != null)
                {
                    Log.Logger.Information("Loaded config from {ConfigPath}", _newConfigPath);
                    return Task.FromResult(config);
                }

                if (retryCount < 3)
                {
                    Log.Logger.Error("Unable to load config at {ConfigPath}. Resetting Config and retrying!", _newConfigPath);
                    return InitBlankConfig(fallbackPathFinder, retryCount + 1);
                }
                else
                {
                    Log.Logger.Fatal("Retries failed loading config at {ConfigPath}. Terminating!", _newConfigPath);
                    throw new IOException($"Unable to load config at {_newConfigPath}");
                }
            }
        }

        private static SC2Config? FromFile(string newConfigPath)
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

                return new SC2Config(data);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Unable to load config from {ConfigPath}!", newConfigPath);
                return null;
            }
        }

        private static async Task<SC2Config> MigrateLegacyConfig(Func<Task<string>> fallbackPathFinder, int retryCount)
        {
            Log.Logger.Warning("Migrating from legacy config");
            string str = File.ReadLines(_legacyConfigPath).First<string>();
#if WINDOWS
            if (!File.Exists(str))
#else
            if (!Directory.Exists(str))
#endif
            {
                Log.Logger.Error("Bad StarCraft II location detected in legacy config! {BadPath}", str);
                NewConfig(await fallbackPathFinder()).Save();
            }
            else
            {
                NewConfig(str).Save();
            }

            return await Load(fallbackPathFinder, retryCount);
        }

        public void SetLoadedMod(CampaignType campaignType, string? modName)
        {
            ModSelectionInfo = ModSelectionInfo.SetItem(campaignType,
                ModSelectionInfo[campaignType].SetItem("mod", modName));
        }

        public bool ModsEnabled(CampaignType campaignType)
        {
            return ModSelectionInfo[campaignType]["enabled"] == "on";
        }

        public void SetModEnabled(CampaignType campaignType, bool enabled)
        {
            ModSelectionInfo = ModSelectionInfo.SetItem(campaignType,
                ModSelectionInfo[campaignType].SetItem("enabled", enabled ? "on" : "off"));
        }

        public string? GetLoadedMod(CampaignType campaignType)
        {
            return ModSelectionInfo[campaignType]["mod"];
        }
    }
}

