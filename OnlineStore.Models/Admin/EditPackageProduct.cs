using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditPackageProduct
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int? ProductVarientID { get; set; }
        public int OldPrice { get; set; }
        public int NewPrice { get; set; }
    }
}
