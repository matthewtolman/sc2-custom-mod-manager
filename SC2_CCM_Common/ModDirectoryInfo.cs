using System;
using System.IO;
using Serilog.Core;

namespace SC2_CCM_Common
{
    public class ModDirectoryInfo
    {
        public string[] metadataTxt;
        public string directory;

        public ModDirectoryInfo(string dir)
        {
            metadataTxt = Directory.GetFiles(dir, "metadata.txt", SearchOption.AllDirectories);
            directory = dir;
        }

        public bool Validate(Action<string> messageProcessor)
        {
            if (metadataTxt.Length == 0)
            {
                Log.Logger.Error("Could not load mod from {Directory}. Could not find metadata file!", directory);
                messageProcessor($"FAILED TO LOAD: Unable to find metadata.txt for \"{directory}\"!");
                return false;
            }
            else if (metadataTxt.Length != 1)
            {
                Log.Logger.Error("Could not load mod from {Directory}. Found too many metadata files! {MetadataFiles}", directory, metadataTxt);
                messageProcessor($"FAILED TO LOAD: Unable to find metadata.txt for \"{directory}\"!");
                return false;
            }

            return true;
        }
    }
}