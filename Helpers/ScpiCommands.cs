namespace HiPot.AutoTester.Desktop.Helpers
{
    public static class ScpiCommands
    {
        // 1. 開始測試指令
        public const string StartTest = "SAFE:STAR";
        // 2. 停止測試指令 (安全備用)
        public const string StopTest = "SAFE:STOP";
        // 3. 查詢目前狀態 (如 "STOPPED")
        public const string GetStatus = "SAFE:STAT?";
        // 4. 查詢工作記憶體中已設定多少個step (如 +2)
        public const string GetSnum = "SAFE:SNUM?";
        // 5. 取得測試總結結果 (如 116, 116)
        public const string GetTestSummary = "SAFE:RES:ALL?";
        // 6. 取得測試模式 (如 GB, DC)
        public const string GetModeSummary = "SAFE:RES:ALL:MODE?";
        // 7. 讀取所有步驟的量測數值 (如 +6.540001E-02,+9.900000E+37)
        public const string GetAllResults = "SAFE:RES:ALL:MMET?";
        // 8. 系統重置與清除
        public const string Reset = "*RST";
        public const string ClearStatus = "*CLS";
        // 9. 查詢設備身份 (測試通訊用)
        public const string Identification = "*IDN?";
        // 10. 判定測試是否通過的代碼
        public const string PassCode = "116";
    }
}
