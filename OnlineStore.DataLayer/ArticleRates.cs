using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ArticleRate : EntityBase
    {
        [MaxLength(15)]
        public string IP { get; set; }

        [MaxLength(128)]
        public string UserID { get; set; }
        public float Rate { get; set; }

        [ForeignKey("Article")]
        public int ArticleID { get; set; }
        public Article Article { get; set; }

    }

    public static class ArticleRates
    {
        public static void Insert(ArticleRate articleRate)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ArticleRates.Add(articleRate);
                db.SaveChanges();
            }
        }

        public static int RepeatPreferentials(int articleID, string ip, string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                if (!String.IsNullOrWhiteSpace(userID))
                {
                    return db.ArticleRates.Where(item => item.UserID == userID && item.ArticleID == articleID).Count();
                }

                return db.ArticleRates.Where(item => item.IP == ip && item.ArticleID == articleID).Count();
            }
        }
    }
}
