using System;
using System.IO;
using Serilog.Core;

namespace SC2_CCM_Common
{
    /// <summary>
    /// Information for a mod directory
    /// </summary>
    public class ModDirectoryInfo
    {
        /// <summary>
        /// Collection of metadata text files found in mod
        /// </summary>
        public readonly string[] MetadataTxtFiles;
        
        /// <summary>
        /// Directory for mod information
        /// </summary>
        private readonly string _directory;

        /// <summary>
        /// Create Information object for a mod directory
        /// </summary>
        /// <param name="dir">Directory to pull mod information from</param>
        public ModDirectoryInfo(string dir)
        {
            MetadataTxtFiles = System.IO.Directory.GetFiles(dir, "metadata.txt", SearchOption.AllDirectories);
            _directory = dir;
        }

        /// <summary>
        /// Validates mod directory information
        /// </summary>
        /// <param name="messageProcessor">
        /// Processor for user-facing messages
        /// </param>
        /// <returns></returns>
        public bool Validate(Action<string> messageProcessor)
        {
            if (MetadataTxtFiles.Length == 0)
            {
                Log.Logger.Error("Could not load mod from {Directory}. Could not find metadata file!", _directory);
                messageProcessor($"FAILED TO LOAD: Unable to find metadata.txt for \"{_directory}\"!");
                return false;
            }
            else if (MetadataTxtFiles.Length != 1)
            {
                Log.Logger.Error("Could not load mod from {Directory}. Found too many metadata files! {MetadataFiles}", _directory, MetadataTxtFiles);
                messageProcessor($"FAILED TO LOAD: Unable to find metadata.txt for \"{_directory}\"!");
                return false;
            }

            return true;
        }
    }
}