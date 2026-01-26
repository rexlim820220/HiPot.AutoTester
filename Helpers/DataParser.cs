using System;
using System.Globalization;
using HiPot.AutoTester.Desktop.Models;

namespace HiPot.AutoTester.Desktop.Helpers
{
    public class DataParser
    {
        public static TestResult ParseRawData(string rawData, string isn)
        {
            var parts = rawData.Split(',');

            return new TestResult
            {
                ISN = isn,
                // 使用 Float 樣式解析科學記號 (如 1.500E+03)
                Voltage = parts.Length > 0 ? double.Parse(parts[0], NumberStyles.Float) : 0,
                Current = parts.Length > 1 ? double.Parse(parts[1], NumberStyles.Float) : 0,
                Judgment = parts.Length > 2 ? parts[2].Trim() : "FAIL",
                TestTime = DateTime.Now
            };
        }
    }
}
