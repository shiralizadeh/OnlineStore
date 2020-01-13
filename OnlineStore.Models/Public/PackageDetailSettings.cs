using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Admin;

namespace OnlineStore.Models.Public
{
    public class PackageDetailSettings
    {
        public PackageDetail PackageDetails { get; set; }
        public List<EditProductImage> PackageImages { get; set; }
        public EditProductImage DefaultImage { get; set; }
        public List<ViewPackageProduct> PackageProducts { get; set; }
}
}
