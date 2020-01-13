using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProductImage
    {
        public int ID { get; set; }

        public string Filename { get; set; }

        public ProductImagePlace ProductImagePlace { get; set; }
    }
}
