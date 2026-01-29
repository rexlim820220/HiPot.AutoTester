using System;
using System.Linq;
using System.Collections.Generic;
using HiPot.AutoTester.Desktop.Interfaces;
using HiPot.AutoTester.Desktop.Models;

namespace HiPot.AutoTester.Desktop.Helpers
{
    public class DataParser
    {
        public static TestResult ParseRawData(IInstrumentCommunication instrument, string isn, string model)
        {
            var finalResult = new TestResult { ISN = isn, Model = model, TestTime = DateTime.Now };
            List<string> stepDetails = new List<string>();

            string snumRaw = instrument.Query(ScpiCommands.GetSnum).Replace("+", "").Trim();
            if (!int.TryParse(snumRaw, out int stepCount))
            {
                finalResult.Result = "FAIL";
                finalResult.Test_Value = "Invalid SNUM response";
                return finalResult;
            }

            if (stepCount > 0)
            {
                string modeRaw = instrument.Query(ScpiCommands.GetModeSummary);
                string resultRaw = instrument.Query(ScpiCommands.GetAllResults);

                string[] modeArray = modeRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] resultArray = resultRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < stepCount; i++)
                {
                    string mode = modeArray.Length > i ? modeArray[i].Trim() : "UNKNOWN";
                    double val = ConvertScientificToDouble(resultArray.Length > i ? resultArray[i].Trim() : "UNKNOWN");
                    string unit; switch (mode)
                    {
                        case "GB": unit = "mΩ"; break;
                        case "IR": unit = "MΩ"; break;
                        case "AC": case "DC": unit = "mA"; break;
                        default:
                            unit = string.Empty;
                            break;
                    }
                    double displayVal = (mode == "IR") ? val : val * 1000;
                    string valStr = (val > 1E+30) ? "N/A" : displayVal.ToString("F2");
                    stepDetails.Add(val.ToString() == "N/A"? $"{mode}:{valStr}":$"{mode}:{valStr} {unit}");
                }
            }

            finalResult.Test_Value = string.Join(", ", stepDetails);
            string overallResult = instrument.Query(ScpiCommands.GetTestSummary);
            string[] results = overallResult.Split(',');
            bool allPass = results.Length == stepCount && results.All(r => r.Trim() == ScpiCommands.PassCode);
            finalResult.Result = allPass ? "PASS" : "FAIL";
            return finalResult;
        }

        public static double ConvertScientificToDouble(string input)
        {
            return double.Parse(input, System.Globalization.NumberStyles.Float);
        }
    }
}
