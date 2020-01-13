namespace OnlineStore.UserWorks
{
    partial class frmReport
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
            this.grdUserWorks = new System.Windows.Forms.DataGridView();
            this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Diff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdUserWorks)).BeginInit();
            this.SuspendLayout();
            // 
            // grdUserWorks
            // 
            this.grdUserWorks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdUserWorks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title,
            this.StartTime,
            this.EndTime,
            this.Diff});
            this.grdUserWorks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdUserWorks.Location = new System.Drawing.Point(0, 0);
            this.grdUserWorks.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.grdUserWorks.Name = "grdUserWorks";
            this.grdUserWorks.Size = new System.Drawing.Size(774, 362);
            this.grdUserWorks.TabIndex = 0;
            // 
            // Title
            // 
            this.Title.DataPropertyName = "Title";
            this.Title.HeaderText = "عنوان کار";
            this.Title.Name = "Title";
            this.Title.Width = 250;
            // 
            // StartTime
            // 
            this.StartTime.DataPropertyName = "StartTime";
            this.StartTime.HeaderText = "تاریخ شروع";
            this.StartTime.Name = "StartTime";
            this.StartTime.Width = 175;
            // 
            // EndTime
            // 
            this.EndTime.DataPropertyName = "EndTime";
            this.EndTime.HeaderText = "تاریخ پایان";
            this.EndTime.Name = "EndTime";
            this.EndTime.Width = 175;
            // 
            // Diff
            // 
            this.Diff.DataPropertyName = "Diff";
            this.Diff.HeaderText = "زمان";
            this.Diff.Name = "Diff";
            // 
            // frmReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 362);
            this.Controls.Add(this.grdUserWorks);
            this.Font = new System.Drawing.Font("B Yekan", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "frmReport";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "گزارش کارها";
            this.Load += new System.EventHandler(this.frmReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdUserWorks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdUserWorks;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title;
        private System.Windows.Forms.DataGridViewTextBoxColumn StartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Diff;
    }
}