using System;
using System.Threading.Tasks;
using HiPot.AutoTester.Desktop.Helpers;
using HiPot.AutoTester.Desktop.Services;

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

        public async Task<bool> ExecuteTestAsync(string isn)
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

                    // 根據您的儀器手冊，當狀態回傳包含 STOPPED 代表測試結束
                    if (status.Contains("STOPPED"))
                    {
                        isRunning = false;
                    }
                }

                // 3. 抓取測量數值
                var rawData = _instrument.Query(ScpiCommands.GetAllResults);

                // 4. 解析數據 (將 Raw Data 轉為強型別物件)
                var result = DataParser.ParseRawData(rawData, isn);

                // 5. 執行 SFIS 上傳 (假設您的黑盒子會回傳 bool 或我們可以觀察 result)
                // 這裡建議檢查：測試是否 Pass 且 SFIS 是否上傳成功
                bool sfisResult = _sfis.UploadToSfis(result);

                // 回傳最終結果：只有當測試是 PASS 且 SFIS 上傳成功時才回傳 true
                return result.IsPass && sfisResult;
            }
            catch (Exception ex)
            {
                // 這裡建議記錄 Log，例如：Logger.Write(ex.Message);
                return false; // 發生任何異常（如通訊中斷）皆視為失敗
            }
        }
    }
}
