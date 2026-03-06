using System;
using System.IO;

namespace HiPot.AutoTester.Desktop.Helpers
{
    public static class Logger
    {
        private static readonly string ResPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "HiPot_Result.log"
        );
        private static readonly string SystemLogPath = Path.Combine(
            Path.GetTempPath(),
            "HiPot_AutoTester_Log.txt"
        );
        private static readonly object _lock = new object();

        private static void WriteToFile(string filePath, string message, string level)
        {
            try
            {
                lock (_lock)
                {
                    using (var sw = File.AppendText(filePath))
                    {
                        sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}");
                    }
                }
            }
            catch
            {
            }
        }

        public static void Log(string message, string level = "INFO")
        {
            WriteToFile(SystemLogPath, message, level);
            WriteToFile(ResPath, message, level);
        }

        public static void Debug(string message, string level = "DEBUG")
        {
            WriteToFile(SystemLogPath, message, level);
        }

        public static void LogError(string message, Exception ex)
        {
            var fullMessage = $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}";
            WriteToFile(SystemLogPath, fullMessage, "ERROR");
        }
    }
}
