using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProductPoint
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public ProductPointType ProductPointType { get; set; }
    }
}
