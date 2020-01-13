using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class UserWork : EntityBase
    {
        public string Username { get; set; }
        public string Title { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class UserWorks
    {
        public static List<UserWork> Get(int pageIndex, int pageSize, string pageOrder, string username)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserWorks
                            select item;

                if (!String.IsNullOrWhiteSpace(username))
                    query = query.Where(item => item.Username.Contains(username));

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string username)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserWorks
                            select item;

                if (!String.IsNullOrWhiteSpace(username))
                    query = query.Where(item => item.Username.Contains(username));

                return query.Count();
            }
        }

        public static UserWork GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userWork = db.UserWorks.Where(item => item.ID == id).SingleOrDefault();

                return userWork;
            }
        }

        public static List<UserWork> GetByUsername(string username)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userWorks = db.UserWorks.Where(item => item.Username == username).ToList();

                return userWorks;
            }
        }

        public static void Insert(UserWork userWork)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.UserWorks.Add(userWork);

                db.SaveChanges();
            }
        }

        public static void Update(UserWork userWork)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgUserWishe = db.UserWorks.Where(item => item.ID == userWork.ID).Single();

                orgUserWishe.Username = userWork.Username;
                orgUserWishe.Title = userWork.Title;
                orgUserWishe.StartTime = userWork.StartTime;
                orgUserWishe.EndTime = userWork.EndTime;
                orgUserWishe.LastUpdate = userWork.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var user = (from item in db.UserWorks
                            where item.ID == id
                            select item).Single();

                db.UserWorks.Remove(user);

                db.SaveChanges();
            }
        }

    }
}
