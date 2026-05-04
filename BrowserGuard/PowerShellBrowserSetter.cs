using System;
using System.Diagnostics;

namespace BrowserGuard
{
    /// <summary>
    /// Helper class for setting default browser using PowerShell script
    /// </summary>
    public static class PowerShellBrowserSetter
    {
        /// <summary>
        /// Sets default browser using PowerShell script method
        /// </summary>
        public static void SetDefaultBrowser(string preferredBrowser, Action<string> logCallback)
        {
            string scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "set-default-browser.ps1");
            string powershellPath = GetPowerShellPath();
            
            if (string.IsNullOrEmpty(powershellPath))
            {
                logCallback?.Invoke("PowerShell not found");
                return;
            }
            
            string arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"";

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = powershellPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                });

                logCallback?.Invoke("Script started. Wait for completion");
            }
            catch (Exception ex)
            {
                logCallback?.Invoke($"Error starting script: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the path to PowerShell executable
        /// </summary>
        private static string GetPowerShellPath()
        {
            // Try PowerShell 7+ first (pwsh)
            string pwshPath = @"C:\Program Files\PowerShell\7\pwsh.exe";
            if (System.IO.File.Exists(pwshPath))
                return pwshPath;

            // Try Windows PowerShell 5.1
            string ps5Path = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            if (System.IO.File.Exists(ps5Path))
                return ps5Path;

            // Try using where command to find powershell in PATH
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "where",
                    Arguments = "powershell",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                using (var process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadLine();
                    if (!string.IsNullOrEmpty(output) && System.IO.File.Exists(output))
                        return output;
                }
            }
            catch
            {
                // Ignore errors, fall back to default
            }

            return ps5Path; // Return default as fallback
        }
    }
}