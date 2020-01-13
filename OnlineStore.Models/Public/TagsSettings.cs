using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class TagsSettings
    {
        public List<int> Paging { get; set; }
        public List<ProductItem> Products { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPageIndex { get; set; }
        public string PageTitle { get; set; }
        public string OriginalUrl { get; set; }
        public string CanonicalUrl { get; set; }
    }
}
