using Microsoft.Win32;
using System;

namespace BrowserGuard
{
    /// <summary>
    /// Helper class for monitoring default browser
    /// </summary>
    public static class BrowserMonitorHelper
    {
        private const string KeyPath = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";

        /// <summary>
        /// Gets the ProgID of the current default browser
        /// </summary>
        public static string GetDefaultBrowserProgid()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(KeyPath))
            {
                if (key != null)
                {
                    object value = key.GetValue("Progid");
                    return value?.ToString() ?? "Unknown";
                }
            }
            return "Unknown";
        }

        /// <summary>
        /// Checks if the current default browser matches the preferred one
        /// </summary>
        public static bool IsBrowserChanged(string preferredBrowser)
        {
            string currentBrowser = GetDefaultBrowserProgid();
            return !currentBrowser.Contains(preferredBrowser);
        }
    }
}