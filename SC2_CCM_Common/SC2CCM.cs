using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#if WINDOWS
using Microsoft.Win32;
#endif

namespace SC2_CCM_Common
{
    public class SC2CCM
    {
        private ModFileSystem _modFileSystem;
        private SC2Config _config { get; }

        public SC2CCM(Action<string> messageProcessor, Func<Task<string>> fallbackPathFinder)
        {
            var configTask = SC2Config.Load(fallbackPathFinder);
            configTask.Wait();
            _config = configTask.Result;
            _modFileSystem = new ModFileSystem(_config, messageProcessor);
            Load();
        }

        public void Import(params string[] filePaths)
        {
            foreach (var path in filePaths)
            {
                _modFileSystem.Import(path);
            }

            Load();
        }

        public void Load()
        {
            Log.Logger.Debug("Running SC2CCM Load");
            _modFileSystem.EnsureDirectories();
            _modFileSystem.UnzipCustomCampaigns();
            _modFileSystem.HandleCustomCampaignDependencies();
            _modFileSystem.LoadMods();
        }

        public Dictionary<CampaignType, Dictionary<string, Mod>> Mods()
        {
            return _modFileSystem.Mods;
        }

        public Dictionary<string, Mod> Mods(CampaignType campaignType)
        {
            var mods = _modFileSystem.Mods;
            return mods.ContainsKey(campaignType) ? mods[campaignType] : new Dictionary<string, Mod>() ;
        }

        public void InstallMod(Mod mod)
        {
            _modFileSystem.Install(mod);
            _config.SetLoadedMod(mod.GetCampaignType(), mod.Title);
        }

        public void Reset(CampaignType campaignType)
        {
            _modFileSystem.Reset(campaignType);
            _config.SetLoadedMod(campaignType, null);
        }

        public string? GetLoadedModTitle(CampaignType type)
        {
            return _config.GetLoadedMod(type);
        }

        public bool ModsEnabled(CampaignType type)
        {
            return _config.ModsEnabled(type);
        }

        public void DisableMods(CampaignType type)
        {
            _modFileSystem.Reset(type);
            _config.SetModEnabled(type, false);
        }

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