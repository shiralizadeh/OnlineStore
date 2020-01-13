using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class PackageListSettings
    {
        public List<int> Paging { get; set; }
        public List<PackageItem> Packages { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPageIndex { get; set; }
        public string PageTitle { get; set; }
    }
}
