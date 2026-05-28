using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrowserGuard
{
    public partial class MainForm : Form
    {
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem startupMenuItem;
        private ToolStripMenuItem usePowerShellMenuItem;
        private TextBox logTextBox;
        private System.Windows.Forms.Timer workTimer;
        private string preferredBrowser = "Chrome";
        private bool usePowerShellMethod = false;
        private int checkInterval = 5000; // Default: 5 seconds

        private const string AppName = "BrowserGuard";

        public MainForm()
        {
            // InitializeComponent() removed - all UI is created programmatically
            this.Icon = IconHelper.CreateColoredIcon();
            LoadConfig();
            SetupTrayIcon();
            SetupLogTextBox();
            StartWorker();
        }

        private void LoadConfig()
        {
            ConfigHelper.LoadConfig(
                browser => preferredBrowser = browser,
                method => usePowerShellMethod = method,
                interval => checkInterval = interval,
                Log,
                () =>
                {
                    if (string.IsNullOrEmpty(preferredBrowser))
                    {
                        preferredBrowser = ConfigHelper.AskForPreferredBrowser();
                        ConfigHelper.SaveConfig(preferredBrowser, usePowerShellMethod, checkInterval, Log);
                        Log($"Selected browser: {preferredBrowser}");
                    }
                    return string.IsNullOrEmpty(preferredBrowser);
                });
        }

        private void SaveConfig()
        {
            ConfigHelper.SaveConfig(preferredBrowser, usePowerShellMethod, checkInterval, Log);
        }

        private void ToggleStartup(object sender, EventArgs e)
        {
            StartupHelper.ToggleStartup(
                AppName,
                Application.ExecutablePath,
                Log,
                UpdateStartupMenuItem);
        }

        private void UpdateStartupMenuItem()
        {
            if (startupMenuItem != null)
            {
                bool inStartup = StartupHelper.IsInStartup(AppName);
                startupMenuItem.Text = inStartup ? "Remove from startup" : "Add to startup";
            }
        }

        private void SetupLogTextBox()
        {
            logTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(logTextBox);
        }

        private void Log(string message)
        {
            var text = $"{DateTime.Now:HH:mm:ss} {message}{Environment.NewLine}";
            if (logTextBox.InvokeRequired)
            {
                logTextBox.BeginInvoke(new Action(() => logTextBox.AppendText(text)));
            }
            else
            {
                logTextBox.AppendText(text);
            }
        }

        private void SetupTrayIcon()
        {
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Restore", null, ShowForm);
            
            startupMenuItem = new ToolStripMenuItem();
            startupMenuItem.Click += ToggleStartup;
            UpdateStartupMenuItem();
            contextMenu.Items.Add(startupMenuItem);

            usePowerShellMenuItem = new ToolStripMenuItem("Use PowerShell method");
            usePowerShellMenuItem.Click += TogglePowerShellMethod;
            usePowerShellMenuItem.Checked = usePowerShellMethod;
            contextMenu.Items.Add(usePowerShellMenuItem);
            
            contextMenu.Items.Add("Exit", null, ExitApplication);

            notifyIcon = new NotifyIcon
            {
                Icon = IconHelper.CreateColoredIcon(),
                Visible = true,
                Text = "BrowserGuard",
                ContextMenuStrip = contextMenu
            };

            notifyIcon.DoubleClick += ShowForm;
        }

        private void TogglePowerShellMethod(object sender, EventArgs e)
        {
            usePowerShellMethod = !usePowerShellMethod;
            usePowerShellMenuItem.Checked = usePowerShellMethod;
            SaveConfig();
            Log(usePowerShellMethod ? "PowerShell method enabled" : "Native method enabled");
        }

        private void ShowForm(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            workTimer.Stop();
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void StartWorker()
        {
            workTimer = new System.Windows.Forms.Timer();
            workTimer.Interval = checkInterval;
            workTimer.Tick += WorkerMethod;
            
            // First check immediately, then start timer
            WorkerMethod(null, null);
            
            workTimer.Start();
            Log($"Monitoring started (interval: {checkInterval}ms)");
        }

        private void WorkerMethod(object sender, EventArgs e)
        {
            if (BrowserMonitorHelper.IsBrowserChanged(preferredBrowser))
            {
                string currentBrowser = BrowserMonitorHelper.GetDefaultBrowserProgid();
                ShowWarningPopup(5);
                
                Log($"Detected {currentBrowser}. Expected {preferredBrowser}. Starting script");

                SetChrome();
            }
            
            UpdateTitleWithTime();
        }

        private void UpdateTitleWithTime()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateTitleWithTime));
                return;
            }
            this.Text = $"BrowserGuard - last check {DateTime.Now:HH:mm:ss}";
        }

        private void SetChrome()
        {
            if (usePowerShellMethod)
            {
                PowerShellBrowserSetter.SetDefaultBrowser(preferredBrowser, Log);
            }
            else
            {
                NativeBrowserSetter.SetDefaultBrowser(preferredBrowser, Log);
            }
        }

        private void ShowWarningPopup(int seconds = 5)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowWarningPopup(seconds)));
                return;
            }

            int formWidth = 800;
            int formHeight = 500;

            foreach (var screen in Screen.AllScreens)
            {
                var form = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.Manual,
                    Size = new System.Drawing.Size(formWidth, formHeight),
                    TopMost = true,
                    Location = new System.Drawing.Point(
                        screen.WorkingArea.Left + (screen.WorkingArea.Width - formWidth) / 2,
                        screen.WorkingArea.Top + (screen.WorkingArea.Height - formHeight) / 2),
                    ShowInTaskbar = false
                };

                var label = new System.Windows.Forms.Label
                {
                    Text = $"Please wait...\n\nBrowser setup starts in {seconds} seconds...",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGray,
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    AutoSize = false
                };

                form.Controls.Add(label);

                var timer = new System.Windows.Forms.Timer { Interval = seconds * 1000 };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    form.Close();
                };

                timer.Start();
                form.Show();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                if (workTimer != null)
                {
                    workTimer.Stop();
                }
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
            base.OnFormClosing(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }
    }
}