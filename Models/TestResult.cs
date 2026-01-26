using System;

namespace HiPot.AutoTester.Desktop.Models
{
    public class TestResult
    {
        public string ISN { get; set; } = string.Empty;
        public double Voltage { get; set; }
        public double Current { get; set; }
        public string Judgment { get; set; } = "FAIL"; // PASS or FAIL
        public DateTime TestTime { get; set; }

        public bool IsPass => Judgment.ToUpper().Contains("PASS");
    }
}
