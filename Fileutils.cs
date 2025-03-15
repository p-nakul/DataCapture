using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCapture
{
    internal class Fileutils
    {
        public static string ExtractZipToTemp(string zipFilePath)
        {
            try
            {
                string tempFolder = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(zipFilePath));
                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
                Directory.CreateDirectory(tempFolder);

                ZipFile.ExtractToDirectory(zipFilePath, tempFolder);
                return tempFolder;
            }
            catch(Exception ex) {
                return "";
            }
        }

        public static bool ExtractNestedZips(string directory)
        {
            try
            {
                foreach (string zipFile in Directory.GetFiles(directory, "*.zip", SearchOption.AllDirectories))
                {
                    string extractPath = Path.Combine(Path.GetDirectoryName(zipFile), Path.GetFileNameWithoutExtension(zipFile));
                    if (!Directory.Exists(extractPath))
                    {
                        Directory.CreateDirectory(extractPath);
                        ZipFile.ExtractToDirectory(zipFile, extractPath);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<string> GetAllFiles(string directory)
        {
            return new List<string>(Directory.GetFiles(directory, "*.html*", SearchOption.AllDirectories));
        }

    }
}
