using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SQLUnitTest.Utilities
{
    /// <summary>
    /// Helper to install SQL Server LocalDB if not already present.
    /// </summary>
    public static class LocalDbInstaller
    {
        private const string DownloadUrl = "https://go.microsoft.com/fwlink/?linkid=2239263";

        public static async Task InstallAsync()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            if (IsInstalled())
            {
                return;
            }

            var tempPath = Path.Combine(Path.GetTempPath(), "SqlLocalDB.msi");
            if (!File.Exists(tempPath))
            {
                using var http = new HttpClient();
                var data = await http.GetByteArrayAsync(DownloadUrl);
                File.WriteAllBytes(tempPath, data);
            }

            var process = Process.Start(new ProcessStartInfo("msiexec.exe", $"/i \"{tempPath}\" /quiet /norestart") { UseShellExecute = false });
            process?.WaitForExit();
        }

        private static bool IsInstalled()
        {
            try
            {
                var proc = Process.Start(new ProcessStartInfo("sqllocaldb", "info") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false });
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}

