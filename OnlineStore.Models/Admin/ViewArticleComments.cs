using System;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Admin
{
    public class ViewArticleComments
    {
        public int ID { get; set; }

        public int ArticleID { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string UserFullName { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public ArticleCommentStatus CommentStatus { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
