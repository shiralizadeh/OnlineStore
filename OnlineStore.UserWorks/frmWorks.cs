using OnlineStore.UserWorks.UserWorksService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OnlineStore.UserWorks
{
    public partial class frmWorks : Form
    {
        public frmWorks()
        {
            InitializeComponent();
        }

        UserWork userWork;
        UserWorksServiceSoapClient UserWorks = new UserWorksServiceSoapClient();

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("لطفا نام خود را وارد نمایید.", "شروع", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (String.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("لطفا عنوان کار را وارد نمایید.", "شروع", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            bgUserWorks.RunWorkerAsync(true);

            DisableFrom();
            pUserWorks.Visible = true;
        }

        private void bgUserWorks_DoWork(object sender, DoWorkEventArgs e)
        {
            var isStart = (bool)e.Argument;
            var result = new UserWorksResult();

            result.IsStart = isStart;
            result.Now = DateTime.Now;

            try
            {
                if (isStart)
                {
                    userWork = new UserWork()
                    {
                        Username = txtUsername.Text,
                        Title = txtTitle.Text,
                    };

                    userWork.ID = UserWorks.StartTime(userWork.Username, userWork.Title);
                }
                else
                {
                    UserWorks.EndTime(userWork.ID);
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Error = ex.Message;
            }

            e.Result = result;
        }

        private void bgUserWorks_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (UserWorksResult)e.Result;

            if (result.IsError)
            {
                MessageBox.Show("رخداد خطا در ارسال اطلاعات. دوباره امتحان کنید.\n" + result.Error, "رخداد خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);

                RefreshFrom(!result.IsStart);
            }
            else
            {
                RefreshFrom(result.IsStart);

                if (tUserWorks.Enabled)
                    tUserWorks.Stop();
                else
                {
                    startTime = result.Now;
                    tUserWorks.Start();
                }
            }

            pUserWorks.Visible = false;
        }

        private void DisableFrom()
        {
            txtTitle.Enabled = txtUsername.Enabled = btnStart.Enabled = btnStop.Enabled = btnReport.Enabled = false;
        }

        private void RefreshFrom(bool isStarted)
        {
            if (isStarted)
            {
                txtTitle.Enabled = txtUsername.Enabled = btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
            else
            {
                txtTitle.Enabled = btnStart.Enabled = true;
                btnStop.Enabled = false;
            }

            btnReport.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bgUserWorks.RunWorkerAsync(false);

            DisableFrom();
            pUserWorks.Visible = true;
        }

        DateTime startTime;
        private void tUserWorks_Tick(object sender, EventArgs e)
        {
            var diff = DateTime.Now.Subtract(startTime);
            lblTimes.Text = "شروع: " + startTime.ToString("HH:mm:ss") + " - زمان: " + (diff.Hours.ToString("00:") + diff.Minutes.ToString("00:") + diff.Seconds.ToString("00"));
            this.Text = "نرم افزار اعلان وضعیت کار - " + lblTimes.Text;
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                var frmReport = new frmReport();

                frmReport.Username = txtUsername.Text;

                frmReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("رخداد خطا در دریافت اطلاعات. دوباره امتحان کنید.\n" + ex.Message, "رخداد خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmWorks_Load(object sender, EventArgs e)
        {

        }
    }
}
