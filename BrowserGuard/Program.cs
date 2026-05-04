using System;
using System.Threading;
using System.Windows.Forms;

namespace BrowserGuard
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}