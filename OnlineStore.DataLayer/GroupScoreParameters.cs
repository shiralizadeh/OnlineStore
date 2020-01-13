using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Public;

namespace OnlineStore.DataLayer
{
    public class GroupScoreParameter : EntityBase
    {
        [ForeignKey("Group")]
        public int GroupID { get; set; }
        public Group Group { get; set; }

        [ForeignKey("ScoreParameter")]
        public int ScoreParameterID { get; set; }
        public ScoreParameter ScoreParameter { get; set; }
    }

    public static class GroupScoreParameters
    {
        public static List<GroupScoreParameter> GetByScoreParameterID(int scoreParameterID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                return db.GroupScoreParameters.Where(item => item.ScoreParameterID == scoreParameterID).ToList();
            }
        }

        public static List<ViewScoreParameter> GetByGroupID(List<int> groupItems)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.GroupScoreParameters
                            where groupItems.Contains(item.GroupID)
                            && item.ScoreParameter.IsActive
                            select new ViewScoreParameter
                            {
                                ID = item.ScoreParameter.ID,
                                Title = item.ScoreParameter.Title
                            };

                query = query.OrderBy(item => item.ID);

                return query.ToList();
            }
        }

        public static void Insert(GroupScoreParameter groupScoreParameter)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                if (!db.Groups.Any(item => item.ParentID == groupScoreParameter.GroupID))
                {
                    db.GroupScoreParameters.Add(groupScoreParameter);

                    db.SaveChanges();
                }
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = (from item in db.GroupScoreParameters
                             where item.ID == id
                             select item).Single();

                db.GroupScoreParameters.Remove(query);

                db.SaveChanges();
            }
        }
    }

}
