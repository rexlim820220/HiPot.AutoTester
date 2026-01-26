using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiPot.AutoTester.Desktop.Helpers
{
    public static class ScpiCommands
    {
        // 1. 啟動測試指令
        // 依據手冊範例：[:SOURCE]:SAFEty:START[:ONCE]
        public const string StartTest = "SAFE:STAR";

        // 2. 停止測試指令 (安全備用)
        public const string StopTest = "SAFE:STOP";

        // 3. 查詢目前狀態
        // 依據手冊範例：用於 Polling 判斷狀態是否為 "STOPPED"
        public const string GetStatus = "SAFE:STAT?";

        // 4. 讀取所有步驟的量測數值 (主計量值)
        // 依據手冊範例：用於獲取如 1.5kV, 0.02mA 等實測數據
        public const string GetAllResults = "SAFE:RES:ALL:MMET?";

        // 5. 系統重置與清除
        public const string Reset = "*RST";
        public const string ClearStatus = "*CLS";

        // 6. 查詢設備身份 (測試通訊用)
        public const string Identification = "*IDN?";
    }
}
