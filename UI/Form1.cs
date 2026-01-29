using System;
using System.IO;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using HiPot.AutoTester.Desktop.Models;
using HiPot.AutoTester.Desktop.Services;
using HiPot.AutoTester.Desktop.Interfaces;
using HiPot.AutoTester.Desktop.BusinessLogic;

namespace HiPot.AutoTester.Desktop.UI
{
    public partial class FormMain : Form
    {
        private TestWorkflowManager _manager;
        private IInstrumentCommunication serialService;

        public FormMain()
        {
            InitializeComponent();
#if DEBUG
            serialService = new MockHiPotService();
#else
            serialService = new HiPotSerialService();
#endif
            var sfisService = new SfisService();

            _manager = new TestWorkflowManager(serialService, sfisService);
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            string isn = txtISN.Text;
            string model = lst_TestModel.Text;
            if (lst_TestModel.SelectedItem is DeviceConfig selectedConfig)
            {
                try
                {
                    for (int i = 0; i < selectedConfig.PsuCount; i++)
                    {
                        await RunTestAsync(isn, model);
                    }
                }
                catch
                {
                    lbl_Result.Text = "READY";
                    lbl_Result.ForeColor = Color.Black;
                    lbl_Result.BackColor = SystemColors.Control;
                    MessageBox.Show("Please check HiPot Serial Port settings and cable connection.\n",
                                    "Serial Port Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private async Task RunTestAsync(string isn, string model)
        {
            btn_start.Enabled = false;
            var dr = MessageBox.Show(
                "High voltage testing is about to begin.\n\n" +
                "Please stay away from the output terminals and the device under test (DUT).\n\n" +
                "Ensure the area is clear and press OK to proceed.",
                "High Voltage Safety Warning",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );
            if (dr != DialogResult.OK)
            {
                lbl_Result.Text = "READY";
                lbl_Result.ForeColor = Color.Black;
                lbl_Result.BackColor = SystemColors.Control;
                btn_start.Enabled = !string.IsNullOrWhiteSpace(txtISN.Text);
                return;
            }
            try
            {
                lbl_Result.BackColor = Color.Gray;
                lbl_Result.ForeColor = Color.White;
                lbl_Result.Text = "TESTING";

                var result = await _manager.ExecuteTestAsync(isn, model);
                dgvResults.Rows.Insert(0, isn, "TEST - " + model, result.Test_Value, result.Result, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                dgvResults.ClearSelection();
                dgvResults.Rows[0].Selected = true;

                if (result.Result.ToUpper() == "FAIL")
                {
                    lbl_Result.BackColor = Color.Red;
                    lbl_Result.ForeColor = Color.White;
                    SystemSounds.Hand.Play();
                    lbl_Result.Text = "FAIL";

                    DialogResult ra = MessageBox.Show("Restart again?", "Test Fail", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (ra == DialogResult.No)
                    {
                        lbl_Result.Text = "READY";
                        lbl_Result.ForeColor = Color.Black;
                        lbl_Result.BackColor = SystemColors.Control;
                    }
                    else
                    {
                        await RunTestAsync(isn, model);
                    }
                }
                else
                {
                    lbl_Result.BackColor = Color.Green;
                    lbl_Result.ForeColor = Color.White;
                    SystemSounds.Asterisk.Play();
                    lbl_Result.Text = "PASS";
                }
            }
            finally
            {
                txtISN.Clear();
                txtISN.Focus();
                btn_start.Enabled = !string.IsNullOrWhiteSpace(txtISN.Text);
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
            btn_EditConfig.FlatStyle = FlatStyle.Flat;
            btn_EditConfig.FlatAppearance.BorderSize = 0;
            col_ISN.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col_TestType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col_Result.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col_Time.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Test_Value.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            try
            {
                LoadModelSettings();
            }
            catch
            {
                MessageBox.Show("Failed to load model configuration.", "System Error");
            }

            Task.Run(() => {
                try
                {
                    serialService.Connect(null, 9600);
                }
                catch
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show("Fail to detect any device!\nPlease check HiPot Serial Port settings or cable connection.\n",
                                "Connection failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Application.Exit();
                    }));
                }
            });
        }

        private void LoadModelSettings()
        {
            string configfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models.txt");
            if (File.Exists(configfilePath))
            {
                lst_TestModel.Items.Clear();
                foreach (var line in File.ReadAllLines(configfilePath))
                {
                    if (string.IsNullOrWhiteSpace(line.Trim())) continue;
                    var parts = line.Split(',');
                    string modelName = parts[0].Trim();
                    if (parts.Length != 2 || string.IsNullOrWhiteSpace(modelName) || !int.TryParse(parts[1], out int psuCount))
                    {
                        throw new Exception("Invalid model configuration format.");
                    }
                    var config = new DeviceConfig
                    {
                        Name = modelName,
                        PsuCount = psuCount
                    };
                    lst_TestModel.Items.Add(config);
                }
                if (lst_TestModel.Items.Count > 0) lst_TestModel.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Configuration file 'models.txt' missing. Using default settings.", "System Hint");
            }
        }

        private async void btn_EditConfig_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models.txt");

            if (File.Exists(filePath))
            {
                btn_EditConfig.Enabled = false;

                await Task.Run(() =>
                {
                    using (var process = System.Diagnostics.Process.Start("notepad.exe", filePath))
                    {
                        process.WaitForExit();
                    }
                });

                LoadModelSettings();
                btn_EditConfig.Enabled = true;

                MessageBox.Show("Model list updated successfully!", "System Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
