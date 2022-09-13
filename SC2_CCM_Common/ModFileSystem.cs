using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SC2_CCM_Common
{
    public class ModFileSystem
    {
        private readonly Action<string> _messageProcessor;
        private readonly SC2Config _config;
        public string Sc2BasePath => _config.StarCraft2Dir;

        public ModFileSystem(SC2Config config, Action<string> messageProcessor)
        {
            this._config = config;
            this._messageProcessor = messageProcessor;
        }

        private void EnsureDirectoryExistsWithPerms(params string[] directoryPath)
        {
            var dirPath = PathName(directoryPath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var directoryInfo = new DirectoryInfo(dirPath);
            directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
        }

        public void EnsureDirectories()
        {
            EnsureDirectoryExistsWithPerms("Maps");
            EnsureDirectoryExistsWithPerms("Maps", "Campaign");
            EnsureDirectoryExistsWithPerms("Maps", "Campaign", "swarm");
            EnsureDirectoryExistsWithPerms("Maps", "Campaign", "void");
            EnsureDirectoryExistsWithPerms("Maps", "Campaign", "nova");
            EnsureDirectoryExistsWithPerms("Maps", "CustomCampaigns");
            EnsureDirectoryExistsWithPerms("Maps", "Mods");
        }

        public void UnzipCustomCampaigns()
        {
            foreach (var f in Directory.GetFiles(CustomCampaignPath()))
            {
                if (f.EndsWith(".zip"))
                {
                    UnzipCampaign(f);
                }
            }
        }

        private bool UnzipCampaign(string zipFile)
        {
            string noExtension = Path.GetFileNameWithoutExtension(zipFile);
            File.SetAttributes(zipFile, FileAttributes.Normal);

            try
            {
                using (FileStream fileStream = new FileStream(zipFile, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Update))
                    {
                        archive.ExtractToDirectory(PathName("Maps", "CustomCampaigns", noExtension));
                        _messageProcessor($"Unzipped \"{zipFile}\".");
                    }
                }
            }
            catch (Exception)
            {
                _messageProcessor($"ERROR: Could not unzip \"{zipFile}\"!");
                return false;
            }

            try
            {
                File.Delete(zipFile);
            }
            catch (IOException)
            {
                _messageProcessor($"Warning: Could not delete zip file \"{zipFile}\" - file in use.");
            }

            return true;
        }

        public void HandleCustomCampaignDependencies()
        {
            var path = CustomCampaignPath();
            var pattern = "*.SC2Mod";
            foreach (var dependency in Directory.GetFiles(path, pattern, SearchOption.AllDirectories)
                         .Concat(Directory.GetDirectories(path, pattern, SearchOption.AllDirectories)))
            {
                HandleCampaignDependency(dependency);
            }
        }

        private string CustomCampaignPath()
        {
            return PathName("Maps", "CustomCampaigns");
        }

        private string CustomCampaignPath(string fileName)
        {
            return PathName("Maps", "CustomCampaigns", fileName);
        }

        public Dictionary<CampaignType, Dictionary<string, Mod>> Mods { get; set; } = new Dictionary<CampaignType, Dictionary<string, Mod>>();

        public void LoadMods()
        {
            var modEnum =
                from directory in Directory.GetDirectories(CustomCampaignPath(), "*", SearchOption.TopDirectoryOnly)
                let dirInfo = new ModDirectoryInfo(directory)
                where dirInfo.Validate(_messageProcessor)
                select Mod.From(dirInfo);
            Mods = modEnum
                .GroupBy(mod => mod.GetCampaignType())
                .ToDictionary(g => g.Key, g => g.ToDictionary(m => m.Title));
        }

        public class CopyTo
        {
            private ModFileSystem _modFileSystem;
            private string _source;

            public CopyTo(ModFileSystem modFileSystem, string source)
            {
                _modFileSystem = modFileSystem;
                _source = source;
            }

            public void To(params string[] destPath)
            {
                _modFileSystem.CopyFilesAndFolders(_source, destPath);
            }
        }

        public CopyTo CopyFrom(string sourcePath)
        {
            return new CopyTo(this, sourcePath);
        }

        public void CopyFilesAndFolders(string sourcePath, params string[] destPath)
        {
            var targetPath = PathName(destPath);
            foreach (string directory in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(directory.Replace(sourcePath, targetPath));
            }
            foreach (string file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(file, file.Replace(sourcePath, targetPath), true);
            }
        }

        public bool ClearDirectory(params string[] directory)
        {
            var dirPath = PathName(directory);
            return Directory.GetFiles(dirPath)
                .Concat(
                    Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly)
                        .Where(Functional.And(
                            NotDirNameContainsPath("Campaign", "swarm"),
                            NotDirNameContainsPath("Campaign", "void"),
                            NotDirNameContainsPath("Campaign", "nova")
                        ))
                )
                .All(file =>
                {
                    var success = Delete(file);
                    if (!success)
                    {
                        _messageProcessor(
                            $"ERROR: Could not delete campaign file {Path.GetFileNameWithoutExtension(file)} - please exit the campaign and try again.");
                    }

                    return success;
                });
        }

        private Func<string, bool> NotDirNameContainsPath(params string[] pathSegments)
        {
            return dir => !dir.Contains(Path.Combine(pathSegments));
        }

        private bool Delete(string path)
        {
            return DeleteDirectoryIfExists(path) && DeleteFileIfExists(path);
        }

        private bool HandleCampaignDependency(string dependency)
        {
            return Move(Path.GetFileName(dependency), dependency);
        }

        private bool Move(string fileName, string file)
        {
            var path = PathName("Mods", fileName);
            if (!Delete(path))
            {
                _messageProcessor($"ERROR: Could not replace \"{fileName}\" - exit StarCraft II and hit \"Reload\" to fix install properly.");
                return false;
            }


            try
            {
                if (IsDirectory(file))
                {
                    Directory.Move(file, path);
                }
                else
                {
                    File.Move(file, path);
                }
                _messageProcessor($"Moved \"{fileName} to Custom Campaigns folder.");
            }
            catch (IOException e)
            {
                _messageProcessor($"ERROR: ERROR: Could not move \"{fileName}\". {e.Message}");
                return false;
            }

            return true;
        }


        private bool IsDirectory(string file)
        {
            var attrs = File.GetAttributes(file);
            return (attrs & FileAttributes.Directory) != 0;
        }


        private bool DeleteFileIfExists(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool DeleteDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        private string PathName(params string[] path)
        {
            return Path.Combine(path.Prepend(Sc2BasePath).ToArray());
        }

        public void Import(string path)
        {
            if (path.ToLower().EndsWith(".zip"))
            {
                ImportZip(path);
            }
            else if (!Path.HasExtension(path))
            {
                ImportExtensionless(path);
            }
            else
            {
                _messageProcessor($"ERROR: Could not import \"{path}\". Please make sure it is a zip file!");
            }
        }

        private void ImportExtensionless(string path)
        {
            var destPath = CustomCampaignPath(Path.GetFileName(path));
            EnsureDirectoryExistsWithPerms(destPath);
            CopyFrom(path).To(destPath);
            Delete(path);
        }

        private void ImportZip(string path)
        {
            var destFile = CustomCampaignPath(Path.GetFileName(path));
            File.Copy(path, destFile);
        }

        public void Install(Mod mod)
        {
            var campaignDir = CampaignDirectory(mod.GetCampaignType());
            if (ClearDirectory(campaignDir))
            {
                CopyFilesAndFolders(mod.Path, campaignDir);
                _messageProcessor($"Installed mod \"{mod.Title}\" for Campaign \"{CampaignName(mod.GetCampaignType())}\"");
            }
            else
            {
                _messageProcessor($"ERROR: Could not install mod \"{mod.Title}\" for Campaign \"{CampaignName(mod.GetCampaignType())}\" - SC2 Files in use!");
            }
        }

        public void Reset(CampaignType campaignType)
        {
            var campaignDir = CampaignDirectory(campaignType);
            if (ClearDirectory(campaignDir))
            {
                _messageProcessor($"Campaign reset for \"{CampaignName(campaignType)}\"");
            }
            else
            {
                _messageProcessor($"ERROR: Could not reset campaign \"{CampaignName(campaignType)}\" - SC2 Files in use!");
            }
        }

        private string CampaignDirectory(CampaignType campaignType)
        {
            return campaignType switch
            {
                CampaignType.NovaCovertOps => PathName("Maps", "Campaign", "nova"),
                CampaignType.WingsOfLiberty => PathName("Maps", "Campaign"),
                CampaignType.HeartOfTheSwarm => PathName("Maps", "Campaign", "swarm"),
                CampaignType.LegacyOfTheVoid => PathName("Maps", "Campaign", "void"),
                _ => "Unknown"
            };
        }

        private string CampaignName(CampaignType campaignType)
        {
            return campaignType switch
            {
                CampaignType.NovaCovertOps => "Nova Covert Ops",
                CampaignType.WingsOfLiberty => "Wings of Liberty",
                CampaignType.HeartOfTheSwarm => "Heart of the Swarm",
                CampaignType.LegacyOfTheVoid => "Legacy of the Void",
                _ => "Unknown"
            };
        }
    }
}