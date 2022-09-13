using System.IO.Compression;

namespace SC2_CCM_Common
{
    /// <summary>
    /// Manages file system operations for a mod
    /// </summary>
    public class ModFileSystem
    {
        /// <summary>
        /// Message processor for user-facing messages
        /// </summary>
        private readonly Action<string> _messageProcessor;
        
        /// <summary>
        /// Configuration object
        /// </summary>
        private readonly SC2Config _config;
        
        /// <summary>
        /// Base path for all files that go in SC2 directory
        /// </summary>
        private string Sc2BasePath => _config.StarCraft2Dir;

        /// <summary>
        /// Dictionary of campaign types to a dictionary of titles to mods
        /// </summary>
        public Dictionary<CampaignType, Dictionary<string, Mod>> Mods { get; set; } = new();

        /// <summary>
        /// Create a mod file system object
        /// </summary>
        /// <param name="config"></param>
        /// <param name="messageProcessor"></param>
        public ModFileSystem(SC2Config config, Action<string> messageProcessor)
        {
            this._config = config;
            this._messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Ensures a specified directory exists and is writable
        /// </summary>
        /// <param name="directoryPath">
        /// Path for the directory
        /// </param>
        private void EnsureDirectoryExistsWithPerms(params string[] directoryPath)
        {
            var dirPath = PathRelToSc2(directoryPath);
            if (!Directory.Exists(dirPath))
            {
                Log.Logger.Debug("Creating directory {Directory}", dirPath);
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception ex)
                {
                    Log.Logger.Fatal(ex, "Could not create directory {Directory}", dirPath);
                    throw;
                }
            }

            var directoryInfo = new DirectoryInfo(dirPath);
            directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
        }

        /// <summary>
        /// Ensures the map directories exist for StarCraft II
        /// </summary>
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

        /// <summary>
        /// Unzips all zipped custom campaigns
        /// </summary>
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

        /// <summary>
        /// Unzips a single campaign zip file
        /// </summary>
        /// <param name="zipFile"></param>
        private void UnzipCampaign(string zipFile)
        {
            string noExtension = Path.GetFileNameWithoutExtension(zipFile);
            File.SetAttributes(zipFile, FileAttributes.Normal);

            // Unzip
            try
            {
                using var fileStream = new FileStream(zipFile, FileMode.Open);
                using var archive = new ZipArchive(fileStream, ZipArchiveMode.Update);

                Log.Logger.Debug("Unzipping Campaign {ZipFile}", zipFile);
                archive.ExtractToDirectory(PathRelToSc2("Maps", "CustomCampaigns", noExtension));
                _messageProcessor($"Unzipped \"{zipFile}\".");
            }
            catch (Exception)
            {
                Log.Logger.Error("Could not unzip {ZipFile}", zipFile);
                _messageProcessor($"ERROR: Could not unzip \"{zipFile}\"!");
                return;
            }

            // Cleanup
            try
            {
                Log.Logger.Debug("Deleting {ZipFile}", zipFile);
                File.Delete(zipFile);
            }
            catch (IOException)
            {
                Log.Logger.Warning("Could not delete {ZipFile}", zipFile);
                _messageProcessor($"Warning: Could not delete zip file \"{zipFile}\" - file in use.");
            }
        }

        /// <summary>
        /// Handles custom campaign dependencies
        /// </summary>
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

        /// <summary>
        /// Gets the path for custom campaigns
        /// </summary>
        /// <returns></returns>
        private string CustomCampaignPath()
        {
            return PathRelToSc2("Maps", "CustomCampaigns");
        }

        /// <summary>
        /// Gets the custom campaign path for a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string CustomCampaignPath(string fileName)
        {
            return PathRelToSc2("Maps", "CustomCampaigns", fileName);
        }

        /// <summary>
        /// Loads all available mods into the Mods property
        /// </summary>
        public void LoadMods()
        {
            var modEnum =
                from directory in Directory.GetDirectories(CustomCampaignPath(), "*", SearchOption.TopDirectoryOnly)
                let dirInfo = new ModDirectoryInfo(directory)
                where dirInfo.Validate(_messageProcessor)
                select Mod.From(dirInfo);
            Mods = modEnum
                .GroupBy(mod => mod.CampaignType)
                .ToDictionary(g => g.Key, g => g.ToDictionary(m => m.Title));
        }

        /// <summary>
        /// Copies files and folders from a source path to a dest path
        /// </summary>
        /// <param name="sourcePath">Source path</param>
        /// <param name="destPath">Destination path pieces</param>
        private void CopyFilesAndFolders(string sourcePath, params string[] destPath)
        {
            var targetPath = PathRelToSc2(destPath);
            Log.Logger.Debug("Copying from {SourcePath} to {DestPath}", sourcePath, targetPath);
            foreach (string directory in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(directory.Replace(sourcePath, targetPath));
            }
            foreach (string file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(file, file.Replace(sourcePath, targetPath), true);
            }
        }

        /// <summary>
        /// Clears all files from a directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private bool ClearDirectory(params string[] directory)
        {
            var dirPath = PathRelToSc2(directory);
            Log.Logger.Debug("Clearing Directory {Directory}", dirPath);
            return Directory.GetFiles(dirPath)
                .Concat(
                    Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly)
                        .Where(Functional.And(
                            DirPathDoesNotContain("Campaign", "swarm"),
                            DirPathDoesNotContain("Campaign", "void"),
                            DirPathDoesNotContain("Campaign", "nova")
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

        /// <summary>
        /// Returns a method which checks whether a directory path does not contain a path
        /// </summary>
        /// <param name="pathSegments">Path segments to check against</param>
        /// <returns></returns>
        private static Func<string, bool> DirPathDoesNotContain(params string[] pathSegments)
        {
            return dir => !dir.Contains(Path.Combine(pathSegments));
        }

        /// <summary>
        /// Deletes a file or directory
        /// </summary>
        /// <param name="path">Path of file or directory</param>
        /// <returns></returns>
        private static bool Delete(string path)
        {
            return DeleteDirectoryIfExists(path) && DeleteFileIfExists(path);
        }

        /// <summary>
        /// Handles moving a campaign's dependency
        /// </summary>
        /// <param name="dependency"></param>
        private void HandleCampaignDependency(string dependency)
        {
            Move(dependency);
        }

        /// <summary>
        /// Moves a file into our Mod directory
        /// </summary>
        /// <param name="file">File to move</param>
        private void Move(string file)
        {
            var fileName = Path.GetFileName(file);
            var path = PathRelToSc2("Mods", fileName);
            if (!Delete(path))
            {
                Log.Logger.Error("Could not move {File} to {Path}. Unable to delete existing file", file, path);
                _messageProcessor($"ERROR: Could not replace \"{fileName}\" - exit StarCraft II and hit \"Reload\" to fix install properly.");
                return;
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
                Log.Logger.Debug("Moved {File} to {Path}", file, path);
                _messageProcessor($"Moved \"{fileName} to Custom Campaigns folder.");
            }
            catch (IOException e)
            {
                Log.Logger.Error(e, "Failed to move {File} to {Path}", file, path);
                _messageProcessor($"ERROR: ERROR: Could not move \"{fileName}\". {e.Message}");
            }
        }

        /// <summary>
        /// Checks if a path is a directory or not
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static bool IsDirectory(string file)
        {
            var attrs = File.GetAttributes(file);
            return (attrs & FileAttributes.Directory) != 0;
        }

        /// <summary>
        /// Deletes a file if it exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool DeleteFileIfExists(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "Could not delete {File}", path);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Deletes a directory if it exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool DeleteDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "Could not delete {Directory}", path);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts path segments into a file path relative to the StarCraft II folder
        /// </summary>
        /// <param name="pathSegments">Path segments to convert into a path</param>
        /// <returns></returns>
        private string PathRelToSc2(params string[] pathSegments)
        {
            return Path.Combine(pathSegments.Prepend(Sc2BasePath).ToArray());
        }

        /// <summary>
        /// Imports a file as a mod
        /// </summary>
        /// <param name="path">Path to a zip file</param>
        public void Import(string path)
        {
            if (path.ToLower().EndsWith(".zip"))
            {
                Log.Logger.Information("Importing {Mod} as zip", path);
                ImportZip(path);
            }
            else if (!Path.HasExtension(path))
            {
                Log.Logger.Information("Importing {Mod} as extensionless", path);
                ImportExtensionless(path);
            }
            else
            {
                Log.Logger.Error("Unable to determine how to import {Mod}", path);
                _messageProcessor($"ERROR: Could not import \"{path}\". Please make sure it is a zip file!");
            }
        }

        /// <summary>
        /// Import an extensionless file
        /// </summary>
        /// <param name="path"></param>
        private void ImportExtensionless(string path)
        {
            var destPath = CustomCampaignPath(Path.GetFileName(path));
            EnsureDirectoryExistsWithPerms(destPath);
            CopyFilesAndFolders(path, destPath);
            Delete(path);
        }

        /// <summary>
        /// Import a zip file
        /// </summary>
        /// <param name="path"></param>
        private void ImportZip(string path)
        {
            var destFile = CustomCampaignPath(Path.GetFileName(path));
            File.Copy(path, destFile);
        }

        /// <summary>
        /// Install a mod into StarCraft II
        /// </summary>
        /// <param name="mod"></param>
        public void Install(Mod mod)
        {
            var campaignDir = CampaignDirectory(mod.CampaignType);
            if (ClearDirectory(campaignDir))
            {
                Log.Logger.Information("Installing {Mod}", mod);
                CopyFilesAndFolders(mod.Path, campaignDir);
                _messageProcessor($"Installed mod \"{mod.Title}\" for Campaign \"{Campaign.CampaignName(mod.CampaignType)}\"");
            }
            else
            {
                Log.Logger.Information("Could not install mod {Mod}, unable to clear {Dir}", mod, campaignDir);
                _messageProcessor($"ERROR: Could not install mod \"{mod.Title}\" for Campaign \"{Campaign.CampaignName(mod.CampaignType)}\" - SC2 Files in use!");
            }
        }

        /// <summary>
        /// Resets a StarCraft II campaign to be a default campaign
        /// </summary>
        /// <param name="campaignType"></param>
        public void Reset(CampaignType campaignType)
        {
            var campaignDir = CampaignDirectory(campaignType);
            if (ClearDirectory(campaignDir))
            {
                Log.Logger.Information("Resetting campaign for {Campaign}", Campaign.CampaignName(campaignType));
                _messageProcessor($"Campaign reset for \"{Campaign.CampaignName(campaignType)}\"");
            }
            else
            {
                Log.Logger.Error("Failed to reset {Campaign}, unable to clear {Dir}", Campaign.CampaignName(campaignType), campaignDir);
                _messageProcessor($"ERROR: Could not reset campaign \"{Campaign.CampaignName(campaignType)}\" - SC2 Files in use!");
            }
        }

        /// <summary>
        /// Returns the directory for a campaign type
        /// </summary>
        /// <param name="campaignType"></param>
        /// <returns></returns>
        private string CampaignDirectory(CampaignType campaignType)
        {
            return campaignType switch
            {
                CampaignType.NovaCovertOps => PathRelToSc2("Maps", "Campaign", "nova"),
                CampaignType.WingsOfLiberty => PathRelToSc2("Maps", "Campaign"),
                CampaignType.HeartOfTheSwarm => PathRelToSc2("Maps", "Campaign", "swarm"),
                CampaignType.LegacyOfTheVoid => PathRelToSc2("Maps", "Campaign", "void"),
                _ => "Unknown"
            };
        }
    }
}