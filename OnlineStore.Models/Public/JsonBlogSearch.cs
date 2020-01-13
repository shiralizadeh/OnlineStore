using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Providers;
using Newtonsoft.Json;

namespace OnlineStore.Models.Public
{
    public class JsonBlogSearch
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }

        [JsonIgnore]
        public string Summary { get; set; }
        public string GroupTitle { get; set; }
    }
}
