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
        public SC2Config Config { get; set; }

        public SC2CCM(Action<string> messageProcessor, Func<Task<string>> fallbackPathFinder)
        {
            var configTask = SC2Config.Load(fallbackPathFinder);
            configTask.Wait();
            Config = configTask.Result;
            _modFileSystem = new ModFileSystem(Config, messageProcessor);
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
            _modFileSystem.EnsureDirectories();
            _modFileSystem.UnzipCustomCampaigns().ToList();
            _modFileSystem.HandleCustomCampaignDependencies().ToList();
        }

        public IEnumerable<Mod> Mods()
        {
            return _modFileSystem.ModList();
        }

        public void InstallMod(Mod mod)
        {
            _modFileSystem.Install(mod);
        }

        public void Reset(Campaign campaign)
        {
            _modFileSystem.Reset(campaign);
        }
    }
}