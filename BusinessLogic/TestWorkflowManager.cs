using System;
using System.Threading.Tasks;
using HiPot.AutoTester.Desktop.Models;
using HiPot.AutoTester.Desktop.Helpers;
using HiPot.AutoTester.Desktop.Services;
using HiPot.AutoTester.Desktop.Interfaces;

namespace HiPot.AutoTester.Desktop.BusinessLogic
{
    class TestWorkflowManager
    {
        private readonly IInstrumentCommunication _instrument;
        private readonly SfisService _sfis; // 您的黑盒子

        public TestWorkflowManager(IInstrumentCommunication instrument, SfisService sfis)
        {
            _instrument = instrument;
            _sfis = sfis;
        }

        public async Task<TestResult> ExecuteTestAsync(string isn, string model)
        {
            try
            {
                _instrument.SendCommand(ScpiCommands.StartTest);

                bool isRunning = true;
                while (isRunning)
                {
                    await Task.Delay(500);
                    var status = _instrument.Query(ScpiCommands.GetStatus);
                    if (status.Contains("STOPPED"))
                    {
                        isRunning = false;
                    }
                }

                var judgeCode = _instrument.Query(ScpiCommands.GetTestSummary);
                bool isPass = (judgeCode.Trim() == "116");

                var finalResult = DataParser.ParseRawData(_instrument, isn, model);
                return finalResult;
            }
            catch (Exception ex)
            {
                return new TestResult
                {
                    ISN = isn,
                    Model = model,
                    Result = "FAIL",
                    Test_Value = $"Error: {ex.Message}",
                    TestTime = DateTime.Now
                };
            }
            finally
            {
                _instrument.SendCommand(ScpiCommands.StopTest);
            }
        }
    }
}
