using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class AdvancedSearch
    {
        public List<ProductItem> Products { get; set; }
        public List<JsonBlogSearch> Blogs { get; set; }
        public List<ViewProducer> Producers { get; set; }
        public List<JsonProductGroup> Groups { get; set; }
    }
}
