using System;
using System.Windows.Forms;

namespace BrowserGuard
{
    /// <summary>
    /// Helper class for managing application configuration
    /// </summary>
    public static class ConfigHelper
    {
        private const string ConfigFileName = "config.ini";

        /// <summary>
        /// Loads configuration from config.ini file
        /// </summary>
        public static void LoadConfig(Action<string> preferredBrowserSetter, Action<bool> usePowerShellMethodSetter, Action<int> checkIntervalSetter, Action<string> logCallback, Func<bool> askForPreferredBrowser)
        {
            try
            {
                string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
                if (System.IO.File.Exists(configPath))
                {
                    var lines = System.IO.File.ReadAllLines(configPath);
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("preferredBrowser="))
                        {
                            string browser = line.Substring("preferredBrowser=".Length).Trim();
                            if (!string.IsNullOrEmpty(browser))
                            {
                                preferredBrowserSetter?.Invoke(browser);
                            }
                        }
                        else if (line.StartsWith("usePowerShellMethod="))
                        {
                            string value = line.Substring("usePowerShellMethod=".Length).Trim();
                            bool usePowerShell = value.Equals("1") || value.Equals("true", StringComparison.OrdinalIgnoreCase);
                            usePowerShellMethodSetter?.Invoke(usePowerShell);
                        }
                        else if (line.StartsWith("checkInterval="))
                        {
                            string value = line.Substring("checkInterval=".Length).Trim();
                            if (int.TryParse(value, out int interval) && interval > 0)
                            {
                                checkIntervalSetter?.Invoke(interval);
                            }
                        }
                    }
                }
                
                // If config is missing or empty - ask user
                if (askForPreferredBrowser?.Invoke() == true)
                {
                    askForPreferredBrowser?.Invoke();
                }
            }
            catch (Exception ex)
            {
                logCallback?.Invoke($"Config load error: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves configuration to config.ini file
        /// </summary>
        public static void SaveConfig(string preferredBrowser, bool usePowerShellMethod, int checkInterval, Action<string> logCallback)
        {
            try
            {
                string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
                string content = $"preferredBrowser={preferredBrowser}\nusePowerShellMethod={(usePowerShellMethod ? "1" : "0")}\ncheckInterval={checkInterval}";
                System.IO.File.WriteAllText(configPath, content);
            }
            catch (Exception ex)
            {
                logCallback?.Invoke($"Config save error: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows dialog to ask user for preferred browser
        /// </summary>
        public static string AskForPreferredBrowser()
        {
            string preferredBrowser = "Chrome";
            
            var dialog = new Form
            {
                Width = 450,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Select preferred browser",
                StartPosition = FormStartPosition.CenterScreen,
                TopMost = true
            };

            var label = new Label { Left = 20, Top = 20, Width = 400, Text = "Browser name to check:" };
            var textBox = new TextBox { Left = 20, Top = 45, Width = 400, Text = "Chrome" };
            var button = new Button { Text = "Save", Left = 320, Width = 100, Top = 80, DialogResult = DialogResult.OK };
            
            button.Click += (s, e) =>
            {
                preferredBrowser = textBox.Text;
                dialog.Close();
            };

            dialog.Controls.Add(label);
            dialog.Controls.Add(textBox);
            dialog.Controls.Add(button);
            dialog.AcceptButton = button;

            dialog.ShowDialog();
            return preferredBrowser;
        }
    }
}