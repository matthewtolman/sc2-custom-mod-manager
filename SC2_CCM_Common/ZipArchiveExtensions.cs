using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SC2_CCM_Common
{
    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(
            this ZipArchive archive,
            string destinationDirectory,
            bool overwrite = true
        )
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectory);
            }
            else
            {
                string fullName = Directory.CreateDirectory(destinationDirectory).FullName;
                foreach (var zipArchiveEntry in archive.Entries)
                {
                    ExtractEntry(fullName, zipArchiveEntry);
                }
            }
        }

        private static void ExtractEntry(string fullName, ZipArchiveEntry zipArchiveEntry)
        {
            var fullPath = Path.GetFullPath(Path.Combine(fullName, zipArchiveEntry.FullName));
            if (!fullPath.StartsWith(fullName, StringComparison.OrdinalIgnoreCase))
            {
                Log.Logger.Error("Zip archive tried to extract files outside of destination directory! {Zip}", fullPath);
                throw new IOException(
                    "Zip archive attempted to extract file(s) outside of destination directory! Stopping extraction. See this link for more info: https://snyk.io/research/zip-slip-vulnerability");
            }

            var dirName = Path.GetDirectoryName(fullPath);
            if (dirName == null)
            {
                Log.Logger.Error("Could not determine directory name for zip {Zip}!", fullPath);
                throw new IOException("Could not determine directory name for nested zip directory!");
            }
            else if (zipArchiveEntry.Name.Length == 0)
            {
                Directory.CreateDirectory(dirName);
            }
            else
            {
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                zipArchiveEntry.ExtractToFile(fullPath, true);
            }
        }
    }
}