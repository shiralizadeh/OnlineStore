using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.DataLayer;
using OnlineStore.Models.User;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;

namespace OnlineStore.DataLayer
{
    public class ScoreParameterValue : EntityBase
    {
        [ForeignKey("ScoreComment")]
        public int ScoreCommentID { get; set; }
        public ScoreComment ScoreComment { get; set; }

        [ForeignKey("ScoreParameter")]
        public int ScoreParameterID { get; set; }
        public ScoreParameter ScoreParameter { get; set; }

        public int Rate { get; set; }
    }

    public static class ScoreParameterValues
    {
        public static void Insert(List<ScoreParameterValue> scoreParameterValues)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ScoreParameterValues.AddRange(scoreParameterValues);

                db.SaveChanges();
            }
        }

        public static List<ScoresAverage> GetAverages(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ScoreParameterValues
                            where item.ScoreComment.ProductID == productID
                            && item.ScoreComment.ScoreCommentStatus == ScoreCommentStatus.Approved
                            group item by new { item.ScoreParameterID, item.ScoreParameter.Title } into result
                            select new ScoresAverage
                            {
                                ScoreParameterID = result.Key.ScoreParameterID,
                                Title = result.Key.Title,
                                Sum = result.Sum(item => item.Rate),
                                Count = result.Count(),
                            };

                return query.ToList();

            }
        }

        public static List<ScoresAverage> CalculateAverage(int productID)
        {
            var scoresAverages = ScoreParameterValues.GetAverages(productID);

            foreach (var item in scoresAverages)
            {
                if (item.Count > 0)
                {
                    item.Average = (float)item.Sum / (float)item.Count;
                    if (item.Average > 5)
                    {
                        item.Average = 5;
                    }
                }
                else
                {
                    item.Average = 0;
                }
            }

            return scoresAverages;
        }

        public static void DeleteByScoreCommentID(int scoreCommentID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var values = db.ScoreParameterValues.Where(item => item.ScoreCommentID == scoreCommentID).ToList();

                db.ScoreParameterValues.RemoveRange(values);
                db.SaveChanges();
            }
        }

    }
}
