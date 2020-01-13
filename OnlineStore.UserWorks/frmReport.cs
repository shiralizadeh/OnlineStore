using OnlineStore.UserWorks.UserWorksService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineStore.UserWorks
{
    public partial class frmReport : Form
    {
        public string Username;
        UserWorksServiceSoapClient UserWorks = new UserWorksServiceSoapClient();

        public frmReport()
        {
            InitializeComponent();
        }

        private void frmReport_Load(object sender, EventArgs e)
        {
            var userWorks = this.UserWorks.GetLatestByUsername(Username);

            grdUserWorks.DataSource = (from item in userWorks
                                       select new
                                       {
                                           Title = item.Title,
                                           StartTime = (item.StartTime.HasValue ? Utilities.ToPersianDate(item.StartTime.Value) + " " + item.StartTime.Value.ToString("HH:mm:ss") : "ندارد"),
                                           EndTime = (item.EndTime.HasValue ? Utilities.ToPersianDate(item.EndTime.Value) + " " + item.EndTime.Value.ToString("HH:mm:ss") : "ندارد"),
                                           Diff = (item.StartTime.HasValue && item.EndTime.HasValue ? (item.EndTime.Value.Subtract(item.StartTime.Value).ToString("hh':'mm':'ss")) : "ندارد"),
                                       }).ToList();
        }
    }
}
