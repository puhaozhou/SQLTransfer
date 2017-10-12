using System.Windows.Forms;

namespace ErowSqlTransfer
{
    partial class Form1
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
            this.SuspendLayout();
            // 
            // BtnSyncData
            // 
            this.BtnSyncData.Location = new System.Drawing.Point(708, 522);
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
            this.BtnSyncSequence.Location = new System.Drawing.Point(847, 522);
            this.BtnSyncSequence.Name = "BtnSyncSequence";
            this.BtnSyncSequence.Size = new System.Drawing.Size(117, 65);
            this.BtnSyncSequence.TabIndex = 3;
            this.BtnSyncSequence.Text = "同步队列";
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
            this.label3.Location = new System.Drawing.Point(35, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 18);
            this.label3.TabIndex = 7;
            this.label3.Text = "/";
            // 
            // TotalNum
            // 
            this.TotalNum.AutoSize = true;
            this.TotalNum.Location = new System.Drawing.Point(58, 126);
            this.TotalNum.Name = "TotalNum";
            this.TotalNum.Size = new System.Drawing.Size(17, 18);
            this.TotalNum.TabIndex = 8;
            this.TotalNum.Text = "0";
            // 
            // SyncDjNo
            // 
            this.SyncDjNo.Location = new System.Drawing.Point(985, 522);
            this.SyncDjNo.Name = "SyncDjNo";
            this.SyncDjNo.Size = new System.Drawing.Size(142, 65);
            this.SyncDjNo.TabIndex = 9;
            this.SyncDjNo.Text = "同步单据编号";
            this.SyncDjNo.UseVisualStyleBackColor = true;
            this.SyncDjNo.Click += new System.EventHandler(this.SyncDjNo_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 670);
            this.Controls.Add(this.SyncDjNo);
            this.Controls.Add(this.TotalNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NumTransfered);
            this.Controls.Add(this.BtnSyncSequence);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.BtnSyncData);
            this.Name = "Form1";
            this.Text = "Form1";
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
    }
}

