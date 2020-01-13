using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class BlogDetail
    {
        public BlogPost BlogDetails { get; set; }
        public List<RecentPost> RelatedPosts { get; set; }
        public List<RecentComment> LatestComments { get; set; }
        public List<RecentPost> LatestPosts { get; set; }
        public List<ViewArticleComment> Comments { get; set; }
        public RelatedProductSettings Products { get; set; }
    }
}
