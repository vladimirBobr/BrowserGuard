using Microsoft.Win32;
using System;

namespace BrowserGuard
{
    /// <summary>
    /// Helper class for managing Windows startup
    /// </summary>
    public static class StartupHelper
    {
        private const string StartupRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Checks if the application is set to start with Windows
        /// </summary>
        public static bool IsInStartup(string appName)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, false))
            {
                return key.GetValue(appName) != null;
            }
        }

        /// <summary>
        /// Adds the application to Windows startup
        /// </summary>
        public static void AddToStartup(string appName, string exePath, Action<string> logCallback)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true))
                {
                    key.SetValue(appName, $"\"{exePath}\"");
                }
                logCallback?.Invoke("Added to startup");
            }
            catch (Exception ex)
            {
                logCallback?.Invoke($"Error adding to startup: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes the application from Windows startup
        /// </summary>
        public static void RemoveFromStartup(string appName, Action<string> logCallback)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true))
                {
                    key.DeleteValue(appName, false);
                }
                logCallback?.Invoke("Removed from startup");
            }
            catch (Exception ex)
            {
                logCallback?.Invoke($"Error removing from startup: {ex.Message}");
            }
        }

        /// <summary>
        /// Toggles startup registration
        /// </summary>
        public static void ToggleStartup(string appName, string exePath, Action<string> logCallback, Action updateMenuCallback)
        {
            if (IsInStartup(appName))
            {
                RemoveFromStartup(appName, logCallback);
            }
            else
            {
                AddToStartup(appName, exePath, logCallback);
            }
            updateMenuCallback?.Invoke();
        }
    }
}