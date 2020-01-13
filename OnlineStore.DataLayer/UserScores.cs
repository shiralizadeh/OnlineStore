using OnlineStore.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class UserScore : EntityBase
    {
        [Display(Name = "عنوان")]
        public int Value { get; set; }

        [Display(Name = "نوع")]
        public UserScoreType UserScoreType { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(250)]
        public string Description { get; set; }

        [Display(Name = "لینک")]
        [MaxLength(100)]
        public string Url { get; set; }
    }

    public static class UserScores
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserScores
                            select new
                            {
                                item.ID,
                                LastUpdate = item.LastUpdate,
                            };

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserScores
                            select item;

                return query.Count();
            }
        }

        public static UserScore GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userScore = db.UserScores.Where(item => item.ID == id).Single();

                return userScore;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userScore = (from item in db.UserScores
                                 where item.ID == id
                                 select item).Single();

                db.UserScores.Remove(userScore);

                db.SaveChanges();
            }
        }

        public static void Insert(UserScore userScore)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.UserScores.Add(userScore);

                db.SaveChanges();
            }
        }

        public static void Update(UserScore userScore)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgUserScore = db.UserScores.Where(item => item.ID == userScore.ID).Single();

                orgUserScore.Value = userScore.Value;
                orgUserScore.UserScoreType = userScore.UserScoreType;
                orgUserScore.Description = userScore.Description;
                orgUserScore.Url = userScore.Url;
                orgUserScore.LastUpdate = userScore.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
