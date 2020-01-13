using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewMenuItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int? ParentID { get; set; }
        public string Link { get; set; }
        public string IconClass { get; set; }
        public MenuItemType MenuItemType { get; set; }
        public int OrderID { get; set; }

    }
}
