
using System.Windows.Forms;

namespace HiPot.AutoTester.Desktop.UI
{
    partial class FormMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtISN = new System.Windows.Forms.TextBox();
            this.btn_start = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.btn_EditConfig = new System.Windows.Forms.Button();
            this.lbl_TestNo = new System.Windows.Forms.Label();
            this.lst_TestModel = new System.Windows.Forms.ComboBox();
            this.lbl_ISN = new System.Windows.Forms.Label();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.col_ISN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_TestType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Test_Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbl_Result = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            // 
            // txtISN
            // 
            this.txtISN.Location = new System.Drawing.Point(81, 71);
            this.txtISN.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtISN.Name = "txtISN";
            this.txtISN.Size = new System.Drawing.Size(232, 22);
            this.txtISN.TabIndex = 0;
            this.txtISN.TextChanged += new System.EventHandler(this.UpdateStartButtonState);
            // 
            // btn_start
            // 
            this.btn_start.Enabled = false;
            this.btn_start.Font = new System.Drawing.Font("新細明體", 12F);
            this.btn_start.Location = new System.Drawing.Point(443, 55);
            this.btn_start.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(139, 46);
            this.btn_start.TabIndex = 2;
            this.btn_start.Text = "Start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.btn_EditConfig);
            this.groupBox.Controls.Add(this.lbl_TestNo);
            this.groupBox.Controls.Add(this.lst_TestModel);
            this.groupBox.Controls.Add(this.lbl_ISN);
            this.groupBox.Controls.Add(this.txtISN);
            this.groupBox.Location = new System.Drawing.Point(57, 35);
            this.groupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox.Size = new System.Drawing.Size(354, 134);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Test Info";
            // 
            // btn_EditConfig
            // 
            this.btn_EditConfig.Font = new System.Drawing.Font("Segoe MDL2 Assets", 9F);
            this.btn_EditConfig.Location = new System.Drawing.Point(319, 29);
            this.btn_EditConfig.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_EditConfig.Name = "btn_EditConfig";
            this.btn_EditConfig.Size = new System.Drawing.Size(27, 23);
            this.btn_EditConfig.TabIndex = 6;
            this.btn_EditConfig.Text = "";
            this.btn_EditConfig.UseVisualStyleBackColor = true;
            this.btn_EditConfig.Click += new System.EventHandler(this.btn_EditConfig_Click);
            // 
            // lbl_TestNo
            // 
            this.lbl_TestNo.AutoSize = true;
            this.lbl_TestNo.Location = new System.Drawing.Point(27, 34);
            this.lbl_TestNo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_TestNo.Name = "lbl_TestNo";
            this.lbl_TestNo.Size = new System.Drawing.Size(47, 12);
            this.lbl_TestNo.TabIndex = 5;
            this.lbl_TestNo.Text = "Model：";
            // 
            // lst_TestModel
            // 
            this.lst_TestModel.FormattingEnabled = true;
            this.lst_TestModel.Items.AddRange(new object[] {
                "M001 - HOJI",
                "M002 - CATLOW",
                "M003 - JIMBO",
                "M004 - FRONTIER",
                "M005 - HL",
                "M006 - BU18RAMONE",
                "M007 - ENDEAUOR",
                "M008 - MACK",
                "M009 - JIM",
                "M010 - ASUSRS700"
            });
            this.lst_TestModel.Location = new System.Drawing.Point(81, 31);
            this.lst_TestModel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lst_TestModel.Name = "lst_TestModel";
            this.lst_TestModel.Size = new System.Drawing.Size(232, 20);
            this.lst_TestModel.TabIndex = 4;
            this.lst_TestModel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.lst_TestModel.SelectionChangeCommitted += (s, e) => {
                txtISN.Focus();
            };
            this.lst_TestModel.SelectedIndexChanged += new System.EventHandler(this.UpdateStartButtonState);
            // 
            // lbl_ISN
            // 
            this.lbl_ISN.AutoSize = true;
            this.lbl_ISN.Location = new System.Drawing.Point(39, 74);
            this.lbl_ISN.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_ISN.Name = "lbl_ISN";
            this.lbl_ISN.Size = new System.Drawing.Size(35, 12);
            this.lbl_ISN.TabIndex = 3;
            this.lbl_ISN.Text = "ISN：";
            // 
            // dgvResults
            // 
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_ISN,
            this.col_TestType,
            this.Test_Value,
            this.col_Result,
            this.col_Time});
            this.dgvResults.Location = new System.Drawing.Point(57, 195);
            this.dgvResults.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.RowHeadersWidth = 62;
            this.dgvResults.Size = new System.Drawing.Size(721, 327);
            this.dgvResults.TabIndex = 4;
            // 
            // col_ISN
            // 
            this.col_ISN.HeaderText = "ISN";
            this.col_ISN.MinimumWidth = 8;
            this.col_ISN.Name = "col_ISN";
            this.col_ISN.Width = 150;
            // 
            // col_TestType
            // 
            this.col_TestType.HeaderText = "Model";
            this.col_TestType.MinimumWidth = 8;
            this.col_TestType.Name = "col_TestType";
            this.col_TestType.Width = 150;
            // 
            // Test_Value
            // 
            this.Test_Value.HeaderText = "Test Value ";
            this.Test_Value.MinimumWidth = 8;
            this.Test_Value.Name = "Test_Value";
            this.Test_Value.Width = 150;
            // 
            // col_Result
            // 
            this.col_Result.HeaderText = "Result";
            this.col_Result.MinimumWidth = 8;
            this.col_Result.Name = "col_Result";
            this.col_Result.Width = 150;
            // 
            // col_Time
            // 
            this.col_Time.HeaderText = "Test Time";
            this.col_Time.MinimumWidth = 8;
            this.col_Time.Name = "col_Time";
            this.col_Time.Width = 160;
            // 
            // lbl_Result
            // 
            this.lbl_Result.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold);
            this.lbl_Result.Location = new System.Drawing.Point(595, 51);
            this.lbl_Result.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Result.Name = "lbl_Result";
            this.lbl_Result.Size = new System.Drawing.Size(160, 53);
            this.lbl_Result.TabIndex = 7;
            this.lbl_Result.Text = "READY";
            this.lbl_Result.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 598);
            this.Controls.Add(this.lbl_Result);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.btn_start);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormMain";
            this.Text = "HiPot Control Panel";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TextBox txtISN;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label lbl_ISN;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.ComboBox lst_TestModel;
        private System.Windows.Forms.Label lbl_TestNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ISN;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_TestType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Test_Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Result;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Time;
        private System.Windows.Forms.Label lbl_Result;
        private System.Windows.Forms.Button btn_EditConfig;
    }
}

