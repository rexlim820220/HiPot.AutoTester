using System;
using System.IO;

namespace HiPot.AutoTester.Desktop.Helpers
{
    public static class Logger
    {
        private static readonly string LogFilePath = Path.Combine(
            Path.GetTempPath(),
            "HiPot_AutoTester_Log.txt"
        );
        private static readonly object _lock = new object();

        public static void Log(string message, string level = "INFO")
        {
            try
            {
                lock (_lock)
                {
                    using (var sw = File.AppendText(LogFilePath))
                    {
                        sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}");
                    }
                }
            }
            catch
            {
            }
        }

        public static void LogError(string message, Exception ex)
        {
            var fullMessage = $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}";
            Log(fullMessage, "ERROR");
        }
    }
}
