using System;
using System.Windows.Forms;
using HiPot.AutoTester.Desktop.Services;
using HiPot.AutoTester.Desktop.BusinessLogic;

namespace HiPot.AutoTester.Desktop.UI
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
            string model = lst_TestModel.Text;
            try
            {
                bool isSuccess = await _manager.ExecuteTestAsync(isn, model);
                if (!isSuccess)
                {
                    MessageBox.Show("Test Fail", "Restart again?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                dgvResults.Rows.Add(isn, model, "", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                // 專門捕捉物件未設定參考的錯誤
                MessageBox.Show($"通訊物件異常：請確認 Serial Port 是否正確初始化。\n錯誤訊息：{ex.Message}",
                                "系統異常", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void UpdateStartButtonState(object sender, EventArgs e)
        {
            bool isIsnValid = !string.IsNullOrWhiteSpace(txtISN.Text);
            bool isModelSelected = lst_TestModel.SelectedIndex != -1;
            btn_start.Enabled = isIsnValid && isModelSelected;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            txtISN.Focus();
        }
    }
}
