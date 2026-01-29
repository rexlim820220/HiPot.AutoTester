using System;
using System.Threading.Tasks;
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

        public async Task<bool> ExecuteTestAsync(string isn, string model)
        {
            try
            {
                // 1. 發送啟動指令
                _instrument.SendCommand(ScpiCommands.StartTest);

                // 2. Polling 狀態 (輪詢直到測試停止)
                bool isRunning = true;
                while (isRunning)
                {
                    await Task.Delay(500); // 避免 CPU 負載過高
                    var status = _instrument.Query(ScpiCommands.GetStatus);

                    // 當狀態回傳包含 STOPPED 代表測試結束
                    if (status.Contains("STOPPED"))
                    {
                        isRunning = false;
                    }
                }

                var judgeCode = _instrument.Query(ScpiCommands.GetTestSummary);
                bool isPass = (judgeCode.Trim() == "116");

                // 3. 解析數據 (將 Raw Data 轉為強型別物件)
                var result = DataParser.ParseRawData(_instrument, isn, model);

                // 4. 測試是否 Pass 且 SFIS 是否上傳成功
                bool sfisResult = isPass ? _sfis.UploadToSfis(result): false;

                // 回傳最終結果：只有當測試是 PASS 且 SFIS 上傳成功時才回傳 true
                return sfisResult;
            }
            catch (Exception ex)
            {
                // 這裡建議記錄 Log，例如：Logger.Write(ex.Message);
                return false; // 發生任何異常（如通訊中斷）皆視為失敗
            }
            finally
            {
                _instrument.SendCommand(ScpiCommands.StopTest);
            }
        }
    }
}
