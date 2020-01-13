using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Public
{
    public class ProductDetailSettings
    {
        public ProductDetail ProductDetail { get; set; }
        public List<EditProductImage> ProductImages { get; set; }
        public EditProductImage DefaultImage { get; set; }
        public int CommentsCount { get; set; }
        public List<ViewProductQuestion> ProductQuestions { get; set; }
        public List<ViewScoreParameter> ScoreParameters { get; set; }
        public List<ViewScoreComment> ScoreComments { get; set; }
        public RelatedProductSettings RelatedProducts { get; set; }
        public List<ProductItem> SimilarProducts { get; set; }
        public List<ProductItem> ProductAccessories { get; set; }
        public List<ScoresAverage> ScoresAverages { get; set; }
        public List<ViewAttribute> ProductAttributes { get; set; }
        public ViewProducer Producer { get; set; }
        public List<BreadCrumbLink> BreadCrumbLinks { get; set; }
        public List<ProductItem> Gifts { get; set; }
    }
}
