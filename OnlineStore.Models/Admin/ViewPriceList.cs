using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class ViewPriceList
    {
        public int SectionID { get; set; }
        public string SectionTitle { get; set; }
        public string SectionImage { get; set; }
        public int ColumnID { get; set; }
        public List<ViewPriceListProduct> PriceListProducts { get; set; }
    }
}
