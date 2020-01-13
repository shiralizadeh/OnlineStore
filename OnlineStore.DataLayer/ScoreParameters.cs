using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Admin;

namespace OnlineStore.DataLayer
{
    public class ScoreParameter : EntityBase
    {
        public string Title { get; set; }
        public bool IsActive { get; set; }
    }

    public static class ScoreParameters
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title, bool? isActive)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ScoreParameters
                            select new ViewScoreParameters
                            {
                                ID = item.ID,
                                Title = item.Title,
                                IsActive = item.IsActive,
                                LastUpdate = item.LastUpdate
                            };

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (isActive.HasValue)
                    query = query.Where(item => item.IsActive == isActive);

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                var result = query.ToList();

                foreach (var item in result)
                {
                    var groupIDs = GroupScoreParameters.GetByScoreParameterID(item.ID).Select(s => s.GroupID).ToList();

                    if (groupIDs.Count > 0)
                    {
                        item.GroupsTitle = Groups.GetByIDs(groupIDs).Select(group => group.Title).Aggregate((a, b) => b + ", " + a);
                    }

                }

                return result;
            }
        }

        public static int Count(string title, bool? isActive)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ScoreParameters
                            select item;

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (isActive.HasValue)
                    query = query.Where(item => item.IsActive == isActive);

                return query.Count();
            }
        }

        public static ScoreParameter GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var scoreParameter = db.ScoreParameters.Where(item => item.ID == id).Single();

                return scoreParameter;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var scoreParameter = (from item in db.ScoreParameters
                                      where item.ID == id
                                      select item).Single();

                db.ScoreParameters.Remove(scoreParameter);

                db.SaveChanges();
            }
        }

        public static void Insert(ScoreParameter scoreParameter)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ScoreParameters.Add(scoreParameter);

                db.SaveChanges();
            }
        }

        public static void Update(ScoreParameter scoreParameter)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgScoreParameter = db.ScoreParameters.Where(item => item.ID == scoreParameter.ID).Single();

                orgScoreParameter.Title = scoreParameter.Title;
                orgScoreParameter.IsActive = scoreParameter.IsActive;
                orgScoreParameter.LastUpdate = scoreParameter.LastUpdate;

                db.SaveChanges();
            }
        }

    }

}
