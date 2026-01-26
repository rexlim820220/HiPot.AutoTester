using System;
using System.Windows.Forms;
using HiPot.AutoTester.Desktop.Services;
using HiPot.AutoTester.Desktop.BusinessLogic;

namespace HiPot.AutoTester.Desktop
{
    public partial class FormMain : Form
    {
        private TestWorkflowManager _manager;

        public FormMain()
        {
            InitializeComponent();

            var serialService = new HiPotSerialService();
            var sfisService = new SfisService();
            _manager = new TestWorkflowManager(serialService, sfisService);
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            string isn = txtISN.Text;
            bool isSuccess = await _manager.ExecuteTestAsync(isn);

            lblStatus.Text = isSuccess ? "測試並過站成功" : "失敗";
        }
    }
}
