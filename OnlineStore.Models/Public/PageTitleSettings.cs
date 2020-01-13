using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class PageTitleSettings
    {
        public PageTitleSettings()
        {
            ShowTitle = true;
            ShowRSS = false;
        }

        public string Title { get; set; }
        public List<BreadCrumbLink> BreadCrumbs { get; set; }
        public bool ShowTitle { get; set; }
        public bool ShowRSS { get; set; }
        public string RSSUrl { get; set; }
    }
}
