using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProductFile
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Filename { get; set; }

        public ProductImagePlace ProductImagePlace { get; set; }
    }
}
