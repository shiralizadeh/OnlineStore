using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class JsonSimpleSearch
    {
        public List<JsonProductSearch> Products { get; set; }
        public List<JsonBlogSearch> Blogs { get; set; }
        public List<JsonProductGroup> Groups { get; set; }
    }
}
