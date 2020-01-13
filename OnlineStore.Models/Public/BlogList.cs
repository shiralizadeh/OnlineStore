using System.Collections.Generic;

namespace OnlineStore.Models.Public
{
    public class BlogList
    {
        public List<int> Paging { get; set; }
        public List<BlogPost> DataList { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPageIndex { get; set; }
        public List<RecentComment> LatestComments { get; set; }
        public List<RecentPost> LatestPosts { get; set; }
        public int? GroupID { get; set; }
    }
}
