using System;
using System.Windows.Forms;
using HiPot.AutoTester.Desktop.UI;
using HiPot.AutoTester.Desktop.Helpers;

namespace HiPot.AutoTester.Desktop
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += (s, e) => Logger.LogError("Unhandled UI Exception", e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Logger.LogError("Unhandled App Exception", e.ExceptionObject as Exception);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
