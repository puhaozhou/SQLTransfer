using System.Windows.Forms;

namespace SQLTransfer
{
    partial class MainPage
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnSyncData = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.BtnSyncSequence = new System.Windows.Forms.Button();
            this.NumTransfered = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TotalNum = new System.Windows.Forms.Label();
            this.SyncDjNo = new System.Windows.Forms.Button();
            this.IsSyncTables = new System.Windows.Forms.CheckBox();
            this.TableNames = new System.Windows.Forms.TextBox();
            this.UseOracleColumn = new System.Windows.Forms.RadioButton();
            this.UseMsSqlColumn = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnSyncData
            // 
            this.BtnSyncData.Location = new System.Drawing.Point(709, 666);
            this.BtnSyncData.Name = "BtnSyncData";
            this.BtnSyncData.Size = new System.Drawing.Size(119, 65);
            this.BtnSyncData.TabIndex = 0;
            this.BtnSyncData.Text = "同步数据";
            this.BtnSyncData.UseVisualStyleBackColor = true;
            this.BtnSyncData.Click += new System.EventHandler(this.BtnSyncData_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 59);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1116, 46);
            this.progressBar1.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 184);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(1116, 211);
            this.textBox1.TabIndex = 2;
            // 
            // BtnSyncSequence
            // 
            this.BtnSyncSequence.Location = new System.Drawing.Point(848, 666);
            this.BtnSyncSequence.Name = "BtnSyncSequence";
            this.BtnSyncSequence.Size = new System.Drawing.Size(117, 65);
            this.BtnSyncSequence.TabIndex = 3;
            this.BtnSyncSequence.Text = "同步序列";
            this.BtnSyncSequence.UseVisualStyleBackColor = true;
            this.BtnSyncSequence.Click += new System.EventHandler(this.BtnSyncSequence_Click);
            // 
            // NumTransfered
            // 
            this.NumTransfered.AutoSize = true;
            this.NumTransfered.Location = new System.Drawing.Point(12, 126);
            this.NumTransfered.Name = "NumTransfered";
            this.NumTransfered.Size = new System.Drawing.Size(17, 18);
            this.NumTransfered.TabIndex = 6;
            this.NumTransfered.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 18);
            this.label3.TabIndex = 7;
            this.label3.Text = "/";
            // 
            // TotalNum
            // 
            this.TotalNum.AutoSize = true;
            this.TotalNum.Location = new System.Drawing.Point(96, 126);
            this.TotalNum.Name = "TotalNum";
            this.TotalNum.Size = new System.Drawing.Size(17, 18);
            this.TotalNum.TabIndex = 8;
            this.TotalNum.Text = "0";
            // 
            // SyncDjNo
            // 
            this.SyncDjNo.Location = new System.Drawing.Point(986, 666);
            this.SyncDjNo.Name = "SyncDjNo";
            this.SyncDjNo.Size = new System.Drawing.Size(142, 65);
            this.SyncDjNo.TabIndex = 9;
            this.SyncDjNo.Text = "同步单据编号";
            this.SyncDjNo.UseVisualStyleBackColor = true;
            this.SyncDjNo.Click += new System.EventHandler(this.SyncDjNo_Click);
            // 
            // IsSyncTables
            // 
            this.IsSyncTables.AutoSize = true;
            this.IsSyncTables.Location = new System.Drawing.Point(12, 449);
            this.IsSyncTables.Name = "IsSyncTables";
            this.IsSyncTables.Size = new System.Drawing.Size(142, 22);
            this.IsSyncTables.TabIndex = 10;
            this.IsSyncTables.Text = "导入指定的表";
            this.IsSyncTables.UseVisualStyleBackColor = true;
            // 
            // TableNames
            // 
            this.TableNames.Location = new System.Drawing.Point(221, 449);
            this.TableNames.Name = "TableNames";
            this.TableNames.Size = new System.Drawing.Size(339, 28);
            this.TableNames.TabIndex = 11;
            // 
            // UseOracleColumn
            // 
            this.UseOracleColumn.AutoSize = true;
            this.UseOracleColumn.Location = new System.Drawing.Point(251, -1);
            this.UseOracleColumn.Name = "UseOracleColumn";
            this.UseOracleColumn.Size = new System.Drawing.Size(177, 22);
            this.UseOracleColumn.TabIndex = 12;
            this.UseOracleColumn.Text = "使用Oracle表结构";
            this.UseOracleColumn.UseVisualStyleBackColor = true;
            // 
            // UseMsSqlColumn
            // 
            this.UseMsSqlColumn.AutoSize = true;
            this.UseMsSqlColumn.Checked = true;
            this.UseMsSqlColumn.Location = new System.Drawing.Point(6, 0);
            this.UseMsSqlColumn.Name = "UseMsSqlColumn";
            this.UseMsSqlColumn.Size = new System.Drawing.Size(213, 22);
            this.UseMsSqlColumn.TabIndex = 13;
            this.UseMsSqlColumn.TabStop = true;
            this.UseMsSqlColumn.Text = "使用sql server表结构";
            this.UseMsSqlColumn.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 505);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(115, 22);
            this.checkBox1.TabIndex = 14;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(221, 505);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(339, 28);
            this.textBox2.TabIndex = 15;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(12, 569);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(115, 22);
            this.checkBox2.TabIndex = 16;
            this.checkBox2.Text = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(221, 567);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(339, 28);
            this.textBox3.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.UseOracleColumn);
            this.groupBox1.Controls.Add(this.UseMsSqlColumn);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(612, 449);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(516, 28);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            this.groupBox1.Paint += new PaintEventHandler(this.groupBox1_Paint);
            // 
            // ErowSqlTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 743);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.TableNames);
            this.Controls.Add(this.IsSyncTables);
            this.Controls.Add(this.SyncDjNo);
            this.Controls.Add(this.TotalNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NumTransfered);
            this.Controls.Add(this.BtnSyncSequence);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.BtnSyncData);
            this.Name = "SQLTransfer";
            this.Text = "SQLTransfer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button BtnSyncData;
        protected System.Windows.Forms.ProgressBar progressBar1;
        protected System.Windows.Forms.TextBox textBox1;
        private Button BtnSyncSequence;
        //private CheckBox ChkSyncSequence;
        private Label NumTransfered;
        private Label label3;
        private Label TotalNum;
        private Button SyncDjNo;
        private CheckBox IsSyncTables;
        private TextBox TableNames;
        private RadioButton UseOracleColumn;
        private RadioButton UseMsSqlColumn;
        private CheckBox checkBox1;
        private TextBox textBox2;
        private CheckBox checkBox2;
        private TextBox textBox3;
        private GroupBox groupBox1;
    }
}

