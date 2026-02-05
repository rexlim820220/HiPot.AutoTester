using HiPot.AutoTester.Desktop.BusinessLogic;
using HiPot.AutoTester.Desktop.Helpers;
using HiPot.AutoTester.Desktop.Interfaces;
using HiPot.AutoTester.Desktop.Models;
using HiPot.AutoTester.Desktop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiPot.AutoTester.Desktop.UI
{
    public partial class FormMain : Form
    {
        private string _lastIsn = "";
        private TestWorkflowManager _manager;
        private readonly IFtpService _ftpService;
        private readonly SfisService sfisService;
        private IInstrumentCommunication serialService;
        private Color _currentColor = Color.LightBlue;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public FormMain()
        {
            InitializeComponent();
#if DEBUG
            serialService = new MockHiPotService();
#else
            serialService = new HiPotSerialService();
#endif
            sfisService = new SfisService();
            _ftpService = new SftpService();
            _manager = new TestWorkflowManager(serialService);
            lst_TestModel.SelectionChangeCommitted += (s, e) =>
            {
                txtISN.Focus();
            };
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            bool needRetry = true;
            string isn = txtISN.Text;
            string model = lst_TestModel.Text;
            
            if (lst_TestModel.SelectedItem is DeviceConfig selectedConfig)
            {
                try
                {
                    while (needRetry)
                    {
                        bool userCancelled = false;
                        List<TestResult> batchResults = new List<TestResult>();

                        for (int psu = 0; psu < selectedConfig.PsuCount; psu++)
                        {
                            if (psu > 0)
                            {
                                MessageBox.Show(
                                    "Please switch PSU cable connection to next test item.\n",
                                    "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information
                                );
                            }
                            var res = await RunTestAsync(psu, selectedConfig.PsuCount, isn, model);
                            if (res == null)
                            {
                                btn_start.Enabled = true;
                                userCancelled = true;
                                break;
                            }
                            res.PSU = $"{psu + 1}";
                            Logger.Log($"Test Result - ISN: {res.ISN}, Model: {res.Model}, Item: PSU{res.PSU}, Status: {res.Result}, Value: {res.Test_Value}", "INFO");
                            batchResults.Add(res);
                        }

                        if (userCancelled) return;

                        if (batchResults.Any(r => r.Result.ToUpper() == "FAIL"))
                        {
                            DialogResult ra = MessageBox.Show("Restart again?", "Test Fail", MessageBoxButtons.YesNo);
                            if (ra == DialogResult.Yes)
                            {
                                needRetry = true;
                                batchResults.Clear();
                            }
                            else
                            {
                                needRetry = false;
                            }
                        }
                        else
                        {
                            var uploadRes = await FormatAndUploadToSfisAsync(batchResults);
                            if (!uploadRes.IsSuccess)
                            {
                                MessageBox.Show($"{isn}:{uploadRes.ErrorMessage}",
                                                "SFIS Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show($"SFIS Upload Success", "Upload Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            if (selectedConfig.PsuCount > 1)
                            {
                                string logContent = GenerateLogContent(batchResults);
                                string ftpfileName = $"{batchResults.Last().ISN}.log";

                                try
                                {
                                    bool ftpSuccess = await _ftpService.UploadLogAsync(logContent, ftpfileName);
                                    if (!ftpSuccess)
                                    {
                                        string backupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{isn}_FTP_Backups");

                                        MessageBox.Show(
                                            $"Failed to upload log to FTP server.\n\n" +
                                            $"The log file has been saved to the local directory for backup:\n" +
                                            $"{backupPath}",
                                            "Upload Error (Network Blocked)",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        MessageBox.Show(
                                            $"File: {ftpfileName}\nStatus: Successfully uploaded to RS700 directory.",
                                            "FTP Upload Success",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError("FTP upload fail", ex);
                                }
                            }
                            needRetry = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lbl_Result.Text = "READY";
                    lbl_Result.ForeColor = Color.Black;
                    lbl_Result.BackColor = SystemColors.Control;
                    MessageBox.Show($"Please check HiPot Serial Port settings and cable connection.\n\n{ex}",
                                    "Serial Port Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private async Task<TestResult> RunTestAsync(int currentIndex, int totalPsu, string isn, string model)
        {
            btn_start.Enabled = false;
            var dr = CustomMessageBox.Show(
                this,
                "High voltage testing is about to begin.\n\n" +
                "Please stay away from the output terminals and the device under test (DUT).\n\n",
                "High Voltage Safety Warning"
            );
            if (dr != DialogResult.OK)
            {
                return null;
            }
            try
            {
                lbl_Result.BackColor = Color.Gray;
                lbl_Result.ForeColor = Color.White;
                lbl_Result.Text = "TESTING";

                var result = await _manager.ExecuteTestAsync(isn, model);
                result.Model = "TEST-" + result.Model;
                if (totalPsu > 1)
                {
                    result.Test_Value = $"PSU{currentIndex + 1}: {result.Test_Value}";
                }
                AddResultToDgv(result);

                if (result.Result.ToUpper() == "PASS")
                {
                    lbl_Result.BackColor = Color.Green;
                    lbl_Result.Text = "PASS";
                    SystemSounds.Asterisk.Play();
                }
                else
                {
                    lbl_Result.BackColor = Color.Red;
                    lbl_Result.Text = "FAIL";
                    SystemSounds.Hand.Play();
                }

                return result;
            }
            finally
            {
                txtISN.Clear();
                txtISN.Focus();
                btn_start.Enabled = !string.IsNullOrWhiteSpace(txtISN.Text);
            }
        }

        private void AddResultToDgv(TestResult result)
        {
            if (result.ISN != _lastIsn)
            {
                _currentColor = (_currentColor == Color.LightBlue) ? Color.White : Color.LightBlue;
            }
            dgvResults.Rows.Insert(0, result.ISN, result.Model, result.Test_Value, result.Result, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            dgvResults.Rows[0].DefaultCellStyle.BackColor = _currentColor;
            dgvResults.Rows[0].Selected = true;
            dgvResults.ClearSelection();
            _lastIsn = result.ISN;
            if (result.Result == "FAIL")
            {
                dgvResults.Rows[0].Cells["col_Result"].Style.ForeColor = Color.Red;
                dgvResults.Rows[0].Cells["col_Result"].Style.SelectionForeColor = Color.Red;
            }
        }

        private string GenerateLogContent(List<TestResult> results)
        {
            var sb = new System.Text.StringBuilder();
            string divider = "---------------------------------------------------------------------";

            sb.AppendLine($"ISN: {results[0].ISN}");

            string testmode = serialService.Query(ScpiCommands.GetModeSummary);
            string[] modeList = testmode.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .ToArray();

            var sourceMap = new Dictionary<string, string>();

            for (int j = 0; j < modeList.Length; j++)
            {
                string mode = modeList[j];
                string value = serialService.Query(
                    $"SOURCE:SAFE:STEP{j + 1}:{mode}:LEV?");
                sourceMap[mode] = value;
            }

            for (int i = 0; i < results.Count; i++)
            {
                sb.AppendLine(divider);
                sb.AppendLine($"PSU{i + 1}");

                string rawData = results[i].Test_Value;
                if (rawData.Contains(": "))
                {
                    rawData = rawData.Split(new[] { ": " }, 2, StringSplitOptions.None).Last();
                }

                string formattedLine =
                Regex.Replace(
                    rawData,
                    @"(?<label>\w+):(?<value>[\d\.]+)",
                    m =>
                    {
                        string label = m.Groups["label"].Value;
                        double val = double.Parse(m.Groups["value"].Value);

                        string source = sourceMap.TryGetValue(label, out var s)
                            ? s
                            : "";

                        return $",{source},+{val.ToString("0.000000E+00")},116";
                    });
                sb.AppendLine(formattedLine);
            }
            sb.AppendLine(divider);
            sb.AppendLine("");
            sb.AppendLine($"Hi-Pot Test PASSED! {DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff}");
            return sb.ToString();
        }

        private async Task<SfisResult> FormatAndUploadToSfisAsync(List<TestResult> results)
        {
            if (results == null || !results.Any()) return SfisResult.Failure("", "No data to upload");

            string isn = results.Last().ISN;
            string model = results.Last().Model;

            var chkResult = sfisService.CheckRouteAsync(isn).Result;
            if (!chkResult.IsSuccess)
            {
                return SfisResult.Failure("", "Check Route Failed");
            }

            StringBuilder pDataBuilder = new StringBuilder();
            pDataBuilder.Append("\"TEST\", \"STATUS\", \"VALUE\", \"UCL\", \"LCL\"\r\n");

            string combinedValues = string.Join(", ", results.Select(r => r.Test_Value));
            pDataBuilder.AppendFormat(
                "\"{0}\", \"{1}\", \"{2}\", \"\", \"\"\r\n",
                model,          // 對應 "TEST" 欄位
                "PASS",         // 對應 "STATUS" 欄位
                combinedValues  // 對應 "VALUE" 欄位
            );
            string pData = pDataBuilder.ToString();
            return await sfisService.UploadResultAsync(isn, pData);
        }

        private void UpdateStartButtonState(object sender, EventArgs e)
        {
            bool isIsnValid = !string.IsNullOrWhiteSpace(txtISN.Text);
            bool isModelSelected = lst_TestModel.SelectedIndex != -1;
            btn_start.Enabled = isIsnValid && isModelSelected;
        }

        private async Task InitializeSfisService(CancellationToken token)
        {
            var loginResult = await sfisService.LoginAsync(2); // Logout

            while (!loginResult.IsSuccess)
            {
                token.ThrowIfCancellationRequested();

                loginResult = await sfisService.LoginAsync(1); // Login

                await Task.Delay(1500, token);
            }
            if (!loginResult.IsSuccess)
                throw new InvalidOperationException("SFIS Login failed after retries.");
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            txtISN.Focus();
            btn_EditConfig.FlatStyle = FlatStyle.Flat;
            btn_EditConfig.FlatAppearance.BorderSize = 0;

            // 設定 DataGridView 樣式
            col_ISN.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col_TestType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col_Result.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col_Time.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Test_Value.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            btn_start.Enabled = false;

            try
            {
                LoadModelSettings();
            }
            catch
            {
                MessageBox.Show("Failed to load model configuration.", "System Error");
            }

            await InitializeSfisService(_cts.Token);

            try
            {
                await Task.Run(() => {
                    serialService.Connect(null, 9600);
                });
                btn_start.Enabled = true;
                Logger.Log("HiPot Device Connected Successfully.", "INFO");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fail to detect any device!\nError: {ex.Message}\n\nPlease check HiPot Serial Port settings or cable connection.",
                    "Connection Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);

                Application.Exit();
            }
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

        private void FormMainClosing(object sender, FormClosingEventArgs e)
        {
            Task.Run(async () => {
                await sfisService.LoginAsync(2);

                if (_ftpService is SftpService sftp)
                {
                    sftp.Dispose();
                }
            });
        }
    }
}
