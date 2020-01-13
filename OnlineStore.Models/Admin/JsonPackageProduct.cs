using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class JsonPackageProduct
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public bool HasVarients { get; set; }
    }
}
