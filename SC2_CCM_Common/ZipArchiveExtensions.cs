using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SC2_CCM_Common
{
    /// <summary>
    /// Extensions for Zip Archives
    /// </summary>
    public static class ZipArchiveExtensions
    {
        /// <summary>
        /// Extracts a zip archive to a directory
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="destinationDirectory"></param>
        public static void ExtractToDirectory(
            this ZipArchive archive,
            string destinationDirectory
        )
        {
            string fullName = Directory.CreateDirectory(destinationDirectory).FullName;
            foreach (var zipArchiveEntry in archive.Entries)
            {
                ExtractEntry(fullName, zipArchiveEntry);
            }
        }

        /// <summary>
        /// Extracts a zip entry to a specific location
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="zipArchiveEntry"></param>
        /// <exception cref="IOException"></exception>
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