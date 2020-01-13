namespace OnlineStore.UserWorks
{
    partial class frmWorks
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWorks));
            this.btnStart = new System.Windows.Forms.Button();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTimes = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.pUserWorks = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.bgUserWorks = new System.ComponentModel.BackgroundWorker();
            this.tUserWorks = new System.Windows.Forms.Timer(this.components);
            this.btnReport = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btnStart.Location = new System.Drawing.Point(299, 286);
            this.btnStart.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(270, 53);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "شروع";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(15, 189);
            this.txtTitle.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(459, 38);
            this.txtTitle.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(486, 192);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 30);
            this.label1.TabIndex = 2;
            this.label1.Text = "عنوان کار";
            // 
            // lblTimes
            // 
            this.lblTimes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblTimes.ForeColor = System.Drawing.Color.White;
            this.lblTimes.Location = new System.Drawing.Point(15, 234);
            this.lblTimes.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblTimes.Name = "lblTimes";
            this.lblTimes.Size = new System.Drawing.Size(549, 45);
            this.lblTimes.TabIndex = 3;
            this.lblTimes.Text = "خوش آمدید";
            this.lblTimes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStop.Location = new System.Drawing.Point(157, 286);
            this.btnStop.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(130, 53);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "پایان";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // pUserWorks
            // 
            this.pUserWorks.Location = new System.Drawing.Point(15, 349);
            this.pUserWorks.MarqueeAnimationSpeed = 10;
            this.pUserWorks.Name = "pUserWorks";
            this.pUserWorks.Size = new System.Drawing.Size(554, 17);
            this.pUserWorks.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pUserWorks.TabIndex = 5;
            this.pUserWorks.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(508, 140);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 30);
            this.label2.TabIndex = 7;
            this.label2.Text = "نام من";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(15, 137);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(459, 38);
            this.txtUsername.TabIndex = 1;
            // 
            // bgUserWorks
            // 
            this.bgUserWorks.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgUserWorks_DoWork);
            this.bgUserWorks.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgUserWorks_RunWorkerCompleted);
            // 
            // tUserWorks
            // 
            this.tUserWorks.Interval = 1000;
            this.tUserWorks.Tick += new System.EventHandler(this.tUserWorks_Tick);
            // 
            // btnReport
            // 
            this.btnReport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btnReport.Location = new System.Drawing.Point(15, 286);
            this.btnReport.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(130, 53);
            this.btnReport.TabIndex = 5;
            this.btnReport.Text = "گزارش کارها";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnList_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::OnlineStore.UserWorks.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(472, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("B Yekan", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label3.Location = new System.Drawing.Point(27, 37);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(422, 51);
            this.label3.TabIndex = 9;
            this.label3.Text = "فروشگاه اینترنتی آنلاین استور";
            // 
            // frmWorks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 375);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.pUserWorks);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblTimes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.btnStart);
            this.Font = new System.Drawing.Font("B Yekan", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "frmWorks";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "نرم افزار اعلان وضعیت کار";
            this.Load += new System.EventHandler(this.frmWorks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTimes;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ProgressBar pUserWorks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.ComponentModel.BackgroundWorker bgUserWorks;
        private System.Windows.Forms.Timer tUserWorks;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
    }
}