using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Public
{
    public class ViewHomeBox
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public HomeBoxType HomeBoxType { get; set; }
        public List<ProductItem> Products { get; set; }
        public List<ViewHomeBoxItem> Items { get; set; }
        public int OrderID { get; set; }
    }
}
