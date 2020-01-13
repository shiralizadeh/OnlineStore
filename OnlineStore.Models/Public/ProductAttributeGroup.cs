using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ProductAttributeGroup
    {
        public string Title { get; set; }
        public List<ProductAttribute> Attributes { get; set; }
    }
}
