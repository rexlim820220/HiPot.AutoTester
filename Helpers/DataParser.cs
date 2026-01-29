using System;
using System.Collections.Generic;
using HiPot.AutoTester.Desktop.Models;
using HiPot.AutoTester.Desktop.Interfaces;

namespace HiPot.AutoTester.Desktop.Helpers
{
    public class DataParser
    {
        public static TestResult ParseRawData(IInstrumentCommunication instrument, string isn, string model)
        {
            var finalResult = new TestResult { ISN = isn, Model = model, TestTime = DateTime.Now };
            List<string> stepDetails = new List<string>();

            int totalSteps = int.Parse(instrument.Query("SAFE:SNUM?"));

            for (int i = 1; i <= totalSteps; i++)
            {
                var rawData = instrument.Query($"SAFE:FETH? STEP,MODE,OMET");
                var parts = rawData.Split(',');

                string mode = parts[1].Trim();
                double val = ConvertScientificToDouble(parts[2]);

                string unit = mode == "GB" ? "mΩ" : (mode == "IR" ? "MΩ" : "mA");
                double displayVal = (mode == "IR") ? val : val * 1000;

                stepDetails.Add($"{mode}:{displayVal:F2}{unit}");
            }

            finalResult.Test_Value = string.Join(", ", stepDetails);

            var code = instrument.Query(ScpiCommands.GetTestSummary);
            finalResult.Result = (code.Trim() == "116") ? "PASS" : "FAIL";

            return finalResult;
        }

        public static double ConvertScientificToDouble(string input)
        {
            return double.Parse(input, System.Globalization.NumberStyles.Float);
        }
    }
}
