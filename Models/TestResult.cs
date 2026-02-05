using System;

namespace HiPot.AutoTester.Desktop.Models
{
    public class TestResult
    {
        public string ISN { get; set; }
        public string Model { get; set; }
        public string PSU { get; set; }
        public string Test_Value { get; set; }
        public string Result { get; set; }    // PASS / FAIL
        public DateTime TestTime { get; set; }
    }
}
