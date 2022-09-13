#if WINDOWS
using Microsoft.Win32;
#endif

namespace SC2_CCM_Common
{
    /// <summary>
    /// Core logic for StarCraft II Custom Campaign Manger
    /// </summary>
    public class SC2CCM
    {
        /// <summary>
        /// File system for managing mods
        /// </summary>
        private readonly ModFileSystem _modFileSystem;

        /// <summary>
        /// Mod campaigns
        /// </summary>
        private readonly SC2Config _config;

        /// <summary>
        /// Creates a new SC2CCM object
        /// </summary>
        /// <param name="messageProcessor">
        /// Processor for any user-facing messages. Allows UI code to display messages from non-UI code
        /// </param>
        /// <param name="fallbackPathFinder">
        /// Method to retrieve StarCraft II executable if auto-detect fails
        /// </param>
        public SC2CCM(Action<string> messageProcessor, Func<Task<string>> fallbackPathFinder)
        {
            var configTask = SC2Config.Load(fallbackPathFinder);
            configTask.Wait();
            _config = configTask.Result;
            _modFileSystem = new ModFileSystem(_config, messageProcessor);
            Load();
        }

        /// <summary>
        /// Imports files as mods
        /// </summary>
        /// <param name="filePaths">
        /// Collection of full paths to mod files to import
        /// </param>
        public void Import(params string[] filePaths)
        {
            foreach (var path in filePaths)
            {
                _modFileSystem.Import(path);
            }

            Load();
        }

        /// <summary>
        /// Loads state from disc
        /// </summary>
        private void Load()
        {
            Log.Logger.Debug("Running SC2CCM Load");
            _modFileSystem.EnsureDirectories();
            _modFileSystem.UnzipCustomCampaigns();
            _modFileSystem.HandleCustomCampaignDependencies();
            _modFileSystem.LoadMods();
        }

        /// <summary>
        /// Returns all mods with their campaign-and-mod-title mappings
        /// </summary>
        /// <returns></returns>
        public Dictionary<CampaignType, Dictionary<string, Mod>> Mods()
        {
            return _modFileSystem.Mods;
        }

        /// <summary>
        /// Returns a dictionary of all mods for a campaign with the mod title related to the mod
        /// </summary>
        /// <param name="campaignType"></param>
        /// <returns></returns>
        public Dictionary<string, Mod> Mods(CampaignType campaignType)
        {
            var mods = _modFileSystem.Mods;
            return mods.ContainsKey(campaignType) ? mods[campaignType] : new Dictionary<string, Mod>() ;
        }

        /// <summary>
        /// Installs a mod to StarCraft II
        /// </summary>
        /// <param name="mod">Mod to install</param>
        public void InstallMod(Mod mod)
        {
            _modFileSystem.Install(mod);
            _config.SetLoadedMod(mod.CampaignType, mod.Title);
        }

        /// <summary>
        /// Resets a campaign to be un-modded
        /// </summary>
        /// <param name="campaignType">Campaign type to reset</param>
        public void Reset(CampaignType campaignType)
        {
            _modFileSystem.Reset(campaignType);
            _config.SetLoadedMod(campaignType, null);
        }

        /// <summary>
        /// Returns the title for the currently selected mod for a campaign
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string? GetSelectedModTitle(CampaignType type)
        {
            return _config.GetLoadedMod(type);
        }

        /// <summary>
        /// Checks whether mods are enabled for a campaign
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool ModsEnabled(CampaignType type)
        {
            return _config.ModsEnabled(type);
        }

        /// <summary>
        /// Disables mods for a campaign
        /// Will also reset to the un-modded campaign
        /// </summary>
        /// <param name="type"></param>
        public void DisableMods(CampaignType type)
        {
            _modFileSystem.Reset(type);
            _config.SetModEnabled(type, false);
        }

        /// <summary>
        /// Enables mods for a campaign
        /// Will also install any selected mods for that campaign
        /// </summary>
        /// <param name="type"></param>
        public void EnableMods(CampaignType type)
        {
            var modToInstall = _config.GetLoadedMod(type);
            if (modToInstall != null)
            {
                if (Mods(type).ContainsKey(modToInstall))
                {
                    _modFileSystem.Install(Mods(type)[modToInstall]);
                }
                else
                {
                    _modFileSystem.Reset(type);
                    _config.SetLoadedMod(type, null);
                }
            }
            else
            {
                _modFileSystem.Reset(type);
            }
            _config.SetModEnabled(type, true);
        }
    }
}