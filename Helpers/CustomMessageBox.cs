using System.Drawing;
using System.Windows.Forms;

namespace HiPot.AutoTester.Desktop.Helpers
{
    public class CustomMessageBox
    {
        public static DialogResult Show(IWin32Window owner, string message, string title)
        {
            using (Form form = new Form())
            {
                form.Text = title;
                form.Width = 600;  // 設定寬度
                form.Height = 400; // 設定高度
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.TopMost = true; // 警告視窗建議置頂

                string warningMessage = "\u26A0\n" + message;

                // 建立顯示訊息的 Label
                Label lblMessage = new Label();
                lblMessage.Text = warningMessage;
                lblMessage.Font = new Font("Verdana", 18, FontStyle.Bold);
                lblMessage.Dock = DockStyle.Fill;
                lblMessage.TextAlign = ContentAlignment.MiddleCenter;
                lblMessage.ForeColor = Color.DarkRed; // 警告色
                lblMessage.Padding = new Padding(20);

                // 建立 OK 按鈕
                Button btnOk = new Button();
                btnOk.Text = "OK";
                btnOk.Font = new Font("Verdana", 24, FontStyle.Bold);
                btnOk.Size = new Size(180, 100);
                btnOk.Dock = DockStyle.Bottom; // 放在底部
                btnOk.DialogResult = DialogResult.OK; // 點擊回傳 OK
                btnOk.BackColor = Color.Orange;
                btnOk.FlatStyle = FlatStyle.Flat;

                // 將控制項加入 Form
                form.Controls.Add(lblMessage);
                form.Controls.Add(btnOk);
                form.AcceptButton = btnOk; // 支援 Enter 鍵觸發 OK

                return form.ShowDialog(owner);
            }
        }
    }
}
