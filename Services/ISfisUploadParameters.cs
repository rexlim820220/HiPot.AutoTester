using System.Configuration;

namespace HiPot.AutoTester.Desktop.Services
{
    public class Sfis_Upload_Para
    {
        public string ProgramId { get; set; }
        public string ProgramPassword { get; set; }
        public string Device { get; set; }
        public string UserID { get; set; }
        public string UserPassword { get; set; }
        public string TSP { get; set; }
        public int Status { get; set; }
        public string CPKFlag { get; set; }
        public string Error { get; set; }

        public Sfis_Upload_Para()
        {
            ProgramId = ConfigurationManager.AppSettings["Sfis.ProgramId"] ?? "TSP_DTAUTO";
            ProgramPassword = ConfigurationManager.AppSettings["Sfis.ProgramPassword"] ?? ":e5T.?H3?n";
            Device = ConfigurationManager.AppSettings["Sfis.Device"] ?? "980212";
            UserID = ConfigurationManager.AppSettings["Sfis.UserID"] ?? "LA0800494";
            UserPassword = ConfigurationManager.AppSettings["Sfis.UserPassword"] ?? "LA0800494";
            TSP = ConfigurationManager.AppSettings["Sfis.TSP"] ?? "HiPot";

            // 處理 int 類型的轉換
            int.TryParse(ConfigurationManager.AppSettings["Sfis.Status"], out int statusVal);
            Status = statusVal == 0 ? 1 : statusVal;

            CPKFlag = ConfigurationManager.AppSettings["Sfis.CPKFlag"] ?? "N";
            Error = ConfigurationManager.AppSettings["Sfis.Error"] ?? "";
        }
    }
}
