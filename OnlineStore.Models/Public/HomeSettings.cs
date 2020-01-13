using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class HomeSettings
    {
        public List<ViewCustomerComment> CustomerComments { get; set; }

        public List<ViewSliderImage> HomeSlider { get; set; }

        public List<ViewSliderImage> OfferSlider { get; set; }

        public List<ViewBanner> Banners { get; set; }

        public List<ViewProducer> Producers { get; set; }

        public List<ViewHomeBox> HomeBoxes { get; set; }

        public List<ProductItem> LatestProducts { get; set; }

        public List<ProductItem> RandomProducts { get; set; }

        public List<ViewHomeBox> SpecialSale { get; set; }

        public List<RecentPost> LatestNews { get; set; }
    }
}
