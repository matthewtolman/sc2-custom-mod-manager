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

        public ImmutableDictionary<Campaign, ImmutableDictionary<String, String?>> ModSelectionInfo
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

            var blankConfig = ImmutableDictionary.Create<Campaign, ImmutableDictionary<String, String?>>()
                .Add(Campaign.WingsOfLiberty, MakeDictEntry())
                .Add(Campaign.HeartOfTheSwarm, MakeDictEntry())
                .Add(Campaign.LegacyOfTheVoid, MakeDictEntry())
                .Add(Campaign.NovaCovertOps, MakeDictEntry());

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
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save! {e.Message}; {e.StackTrace}");
                return false;
            }

            return true;
        }

        private static async Task<SC2Config> InitBlankConfig(Func<Task<string>> fallbackPathFinder)
        {
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
                NewConfig(await fallbackPathFinder()).Save();
            }

            return await MigrateLegacyConfig(fallbackPathFinder);
        }

        public static Task<SC2Config> Load(Func<Task<string>> fallbackPathFinder)
        {
            if (!File.Exists(_legacyConfigPath))
            {
                return InitBlankConfig(fallbackPathFinder);
            }
            else if (!File.Exists(_newConfigPath))
            {
                return MigrateLegacyConfig(fallbackPathFinder);
            }
            else
            {
                var config = FromFile(_newConfigPath);
                
                // If we failed to load our config file, just get a new, blank config
                if (config == null)
                {
                    return InitBlankConfig(fallbackPathFinder);
                }
                return Task.FromResult(config);
            }
        }

        private static SC2Config? FromFile(string newConfigPath)
        {
            try
            {
                var data = JsonSerializer.Deserialize<SC2ConfigData>(File.ReadAllText(newConfigPath));
                if (data == null)
                {
                    return null;
                }

                return new SC2Config(data);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<SC2Config> MigrateLegacyConfig(Func<Task<string>> fallbackPathFinder)
        {
            
            string str = File.ReadLines(_legacyConfigPath).First<string>();
#if WINDOWS
            if (!File.Exists(str))
#else
            if (!Directory.Exists(str))
#endif
            {
                NewConfig(await fallbackPathFinder()).Save();
            }
            else
            {
                NewConfig(str).Save();
            }

            return await Load(fallbackPathFinder);
        }

        public void SetLoadedMod(Campaign campaign, string? modName)
        {
            ModSelectionInfo = ModSelectionInfo.SetItem(campaign,
                ModSelectionInfo[campaign].SetItem("mod", modName));
        }

        public bool ModsEnabled(Campaign campaign)
        {
            return ModSelectionInfo[campaign]["enabled"] == "on";
        }

        public void SetModEnabled(Campaign campaign, bool enabled)
        {
            ModSelectionInfo = ModSelectionInfo.SetItem(campaign,
                ModSelectionInfo[campaign].SetItem("enabled", enabled ? "on" : "off"));
        }

        public string? GetLoadedMod(Campaign campaign)
        {
            return ModSelectionInfo[campaign]["mod"];
        }
    }
}

