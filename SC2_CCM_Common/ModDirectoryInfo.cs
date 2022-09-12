using System;
using System.IO;

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
                messageProcessor($"FAILED TO LOAD: Unable to find metadata.txt for \"{directory}\"!");
                return false;
            }
            else if (metadataTxt.Length != 1)
            {
                messageProcessor($"FAILED TO LOAD: Unable to find metadata.txt for \"{directory}\"!");
                return false;
            }

            return true;
        }
    }
}