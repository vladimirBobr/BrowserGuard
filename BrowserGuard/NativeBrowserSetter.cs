using System;
using System.Diagnostics;
using System.Threading;

namespace BrowserGuard
{
    /// <summary>
    /// Helper class for setting default browser using native Windows UI automation
    /// </summary>
    public static class NativeBrowserSetter
    {
        /// <summary>
        /// Sets default browser using native Windows UI automation
        /// </summary>
        public static void SetDefaultBrowser(string preferredBrowser, Action<string> logCallback)
        {
            try
            {
                // Kill existing SystemSettings process if running
                var existingProcesses = Process.GetProcessesByName("SystemSettings");
                foreach (var p in existingProcesses)
                {
                    p.Kill();
                    p.WaitForExit(1000);
                }

                logCallback?.Invoke("Starting Settings app...");

                // Start the Settings app with default apps page
                Process.Start(new ProcessStartInfo
                {
                    FileName = "ms-settings:defaultapps",
                    UseShellExecute = true
                });

                // Wait for SystemSettings process to appear
                Process systemSettings = null;
                int waitCount = 0;
                const int maxWait = 50; // 5 seconds max
                while (systemSettings == null && waitCount < maxWait)
                {
                    Thread.Sleep(100);
                    var processes = Process.GetProcessesByName("SystemSettings");
                    systemSettings = processes.Length > 0 ? processes[0] : null;
                    waitCount++;
                }

                if (systemSettings == null)
                {
                    logCallback?.Invoke("Could not find SystemSettings process");
                    return;
                }

                logCallback?.Invoke("Waiting for Settings window...");
                
                // Wait for window to be ready
                while (systemSettings.MainWindowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                    systemSettings.Refresh();
                }

                // Give extra time for UI to fully load
                Thread.Sleep(500);

                logCallback?.Invoke("Sending keyboard commands...");

                // Create WScript.Shell COM object for SendKeys (same as PowerShell script)
                dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));

                // Press TAB 4 times to navigate to browser selection (same as PowerShell)
                for (int i = 0; i < 4; i++)
                {
                    shell.SendKeys("{TAB}");
                    Thread.Sleep(100);
                }

                // Type browser name (same as PowerShell script uses "chrom")
                string browserSearch = GetBrowserSearchString(preferredBrowser);
                shell.SendKeys(browserSearch);
                Thread.Sleep(1000); // Wait for search results

                // Press TAB to move to the first result
                shell.SendKeys("{TAB}");
                Thread.Sleep(100);

                // Press ENTER to select
                shell.SendKeys("{ENTER}");
                Thread.Sleep(100);

                // Press ENTER again to confirm
                shell.SendKeys("{ENTER}");
                Thread.Sleep(100);

                // Close Settings with Alt+F4
                shell.SendKeys("%{F4}");

                logCallback?.Invoke("Default browser set to " + preferredBrowser);
            }
            catch (Exception ex)
            {
                logCallback?.Invoke($"Error setting default browser: {ex.Message}");
            }
        }

        /// <summary>
        /// Maps browser names to search strings for UI automation
        /// </summary>
        private static string GetBrowserSearchString(string browser)
        {
            // Map browser names to search strings (partial match)
            switch (browser.ToLower())
            {
                case "chrome":
                case "google chrome":
                    return "chrom";
                case "firefox":
                case "mozilla firefox":
                    return "firefo";
                case "edge":
                case "microsoft edge":
                    return "edg";
                case "opera":
                    return "opera";
                case "brave":
                    return "brave";
                default:
                    // Use first few characters for partial matching
                    return browser.Length > 4 ? browser.Substring(0, 4) : browser;
            }
        }
    }
}