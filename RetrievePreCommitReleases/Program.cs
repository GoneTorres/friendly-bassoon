using System;
using System.IO;
using System.Net;

namespace RetrievePreCommitReleases
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Pre-Commit Downloader");

            var baseURL = "https://github.com/pre-commit/pre-commit";
            Console.WriteLine("Github Repository: {0}", baseURL);

            Console.WriteLine("Enter Major Version");
            var majorVersion = Console.ReadLine();

            Console.WriteLine("Enter Minor Version");
            var minorVersion = Console.ReadLine();

            Console.WriteLine($"Searching for release version {majorVersion}.{minorVersion}.X");

            var patchVersion = 0;

            var archiveURL = $"{baseURL}/archive/refs/tags/";

            HttpWebResponse response = null;

            try
            {
                do
                {
                    var request = (HttpWebRequest)WebRequest.Create(archiveURL + $"v{majorVersion}.{minorVersion}.{patchVersion}.zip");
                    response = (HttpWebResponse)request.GetResponse();
                    patchVersion++;
                }
                while (response != null);
            }
            catch
            {
                patchVersion--;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            if (patchVersion == -1)
            {
                Console.WriteLine("Requested version not found.");
            }
            else
            {
                var folderName = $"v{majorVersion}.{minorVersion}";

                var fileName = $"v{majorVersion}.{minorVersion}.{patchVersion}.zip";

                var fullDownloadPath = Path.Combine(folderName, fileName);

                if (File.Exists(fullDownloadPath))
                {
                    File.Delete(fullDownloadPath);
                }

                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }

                Console.WriteLine($"Downloading release version {fileName}");

                using var webClient = new WebClient();

                try
                {
                    webClient.DownloadFile(archiveURL + fileName, fileName);

                    Console.WriteLine("Download complete! Moving file to subdirectory!");

                    File.Move(fileName, Path.Combine(folderName, fileName));
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}

