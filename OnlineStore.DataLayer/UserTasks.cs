using OnlineStore.Providers;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Admin;
using System.Collections.Generic;

namespace OnlineStore.DataLayer
{
    public class UserTask : EntityBase
    {
        [Display(Name = "عنوان")]
        [MaxLength(300)]
        public string Title { get; set; }

        [Display(Name = "متن وظیفه")]
        public string Text { get; set; }

        [Display(Name = "تاریخ یادآوری")]
        public DateTime UserTaskDate { get; set; }

        [Display(Name = "تاریخ یادآوری")]
        [NotMapped]
        public string PersianUserTaskDate
        {
            get
            {
                if (UserTaskDate == new DateTime())
                    return "";

                return Utilities.ToPersianDate(UserTaskDate);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    UserTaskDate = DateTime.Now;

                UserTaskDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "کاربر")]
        [MaxLength(128)]
        public string UserID { get; set; }

        [Display(Name = "وضعیت")]
        public UserTaskStatus UserTaskStatus { get; set; }

        [Display(Name = "لینک")]
        [MaxLength(500)]
        public string Link { get; set; }

        [Display(Name = "کلید")]
        [MaxLength(128)]
        public string Key { get; set; }

    }

    public static class UserTasks
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title, string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserTasks
                            select new
                            {
                                item.ID,
                                item.Title,
                                item.Text,
                                item.LastUpdate,
                                item.UserTaskDate,
                                item.UserID
                            };

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title) || item.Text.Contains(title));

                if (!String.IsNullOrWhiteSpace(userID))
                    query = query.Where(item => item.UserID == userID);

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string title, string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserTasks
                            select item;

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title) || item.Text.Contains(title));

                if (!String.IsNullOrWhiteSpace(userID))
                    query = query.Where(item => item.UserID == userID);

                return query.Count();
            }
        }

        public static UserTask GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userTask = db.UserTasks.Where(item => item.ID == id).Single();

                return userTask;
            }
        }

        public static UserTask GetByKey(string key)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userTask = db.UserTasks.Where(item => item.Key == key).FirstOrDefault();

                return userTask;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userTask = (from item in db.UserTasks
                                where item.ID == id
                                select item).Single();

                db.UserTasks.Remove(userTask);

                db.SaveChanges();
            }
        }

        public static void Insert(UserTask userTask)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.UserTasks.Add(userTask);

                db.SaveChanges();
            }
        }

        public static void Update(UserTask userTask)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgUserTask = db.UserTasks.Where(item => item.ID == userTask.ID).Single();

                orgUserTask.Title = userTask.Title;
                orgUserTask.Text = userTask.Text;
                orgUserTask.UserTaskDate = userTask.UserTaskDate;
                orgUserTask.UserTaskStatus = userTask.UserTaskStatus;
                orgUserTask.UserID = userTask.UserID;
                orgUserTask.Link = userTask.Link;
                orgUserTask.LastUpdate = userTask.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdateDateTask(UserTask userTask)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                UserTask orgUserTask = db.UserTasks.Where(item => item.ID == userTask.ID).Single();

                orgUserTask.UserTaskDate = userTask.UserTaskDate;
                orgUserTask.UserTaskStatus = userTask.UserTaskStatus;
                orgUserTask.LastUpdate = userTask.LastUpdate;

                db.SaveChanges();
            }
        }

        public static List<ViewUserTask> GetUserTask(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.UserTasks
                            where item.UserID == userID
                            && item.UserTaskDate <= now
                            && item.UserTaskStatus == UserTaskStatus.NotDone
                            select new ViewUserTask
                            {
                                ID = item.ID,
                                Title = item.Title,
                                Text = item.Text,
                                UserTaskDate = item.UserTaskDate,
                                Link = item.Link
                            };

                return query.ToList();
            }
        }

        public static void DoneTask(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgUserTask = db.UserTasks.Where(item => item.ID == id).Single();

                orgUserTask.UserTaskStatus = UserTaskStatus.Done;
                orgUserTask.LastUpdate = DateTime.Now;

                db.SaveChanges();
            }
        }

        public static void SetTask(string title, string text, string userID, string key, string link, DateTime taskDate)
        {
            var rpt = UserTasks.GetByKey(key);

            if (rpt == null)
            {
                var userTask = new UserTask
                {
                    Title = title,
                    Text = text,
                    UserID = userID,
                    UserTaskDate = taskDate,
                    Link = link,
                    UserTaskStatus = UserTaskStatus.NotDone,
                    Key = key
                };

                Insert(userTask);
            }
            else
            {
                var userTask = new UserTask
                {
                    ID = rpt.ID,
                    UserTaskDate = taskDate,
                    UserTaskStatus = UserTaskStatus.NotDone
                };

                UpdateDateTask(userTask);
            }
        }

    }

}
