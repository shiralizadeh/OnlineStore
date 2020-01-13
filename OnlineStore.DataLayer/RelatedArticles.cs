using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;

namespace OnlineStore.DataLayer
{
    public class RelatedArticle : EntityBase
    {
        [Display(Name = "مطلب مرتبط")]
        [ForeignKey("Relation")]
        public int RelationID { get; set; }
        public Article Relation { get; set; }

        [Display(Name = "مطلب")]
        [ForeignKey("Article")]
        public int? ArticleID { get; set; }
        public Article Article { get; set; }

    }

    public static class RelatedArticles
    {
        public static List<JsonRelatedArticle> Get(int articleID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.RelatedArticles
                            where item.ArticleID == articleID
                            select new JsonRelatedArticle
                            {
                                ID = item.ID,
                                Title = item.Relation.Title,
                                RelationID = item.RelationID
                            };

                return query.ToList();
            }
        }

        public static void Insert(List<RelatedArticle> relatedArticles)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.RelatedArticles.AddRange(relatedArticles);

                db.SaveChanges();
            }
        }

        public static void DeleteRelatedArticles(int articleID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var relatedArticles = (from item in db.RelatedArticles
                                       where item.ArticleID == articleID
                                       select item);

                db.RelatedArticles.RemoveRange(relatedArticles);

                db.SaveChanges();
            }
        }

        public static List<RecentPost> GetRelatedArticles(int articleID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var relatedArticles = (from item in db.RelatedArticles
                                       where item.ArticleID == articleID
                                       && item.Relation.ArticleStatus == ArticleStatus.Approved
                                       && now >= item.Relation.PublishDate
                                       && item.Relation.IsVisible
                                       select new RecentPost
                                       {
                                           ID = item.Relation.ID,
                                           Title = item.Relation.Title,
                                           GroupID = item.Relation.GroupID.HasValue ? item.Relation.GroupID.Value : 0,
                                           Image = item.Relation.Image,
                                           LastUpdate = item.LastUpdate
                                       });

                return relatedArticles.ToList();
            }
        }

    }
}
