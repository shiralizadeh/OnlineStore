using OnlineStore.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class UserNotification : EntityBase
    {
        public string UserID { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "لینک")]
        [MaxLength(250)]
        public string Url { get; set; }

        public NotificationType NotificationType { get; set; }
    }
    public static class UserNotifications
    {
        public static void Send(string userID, string title, string url, NotificationType notificationType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                UserNotification userNotification = new UserNotification();

                userNotification.UserID = userID;
                userNotification.Title = title;
                userNotification.Url = url;
                userNotification.NotificationType = notificationType;

                db.UserNotifications.Add(userNotification);

                db.SaveChanges();
            }
        }

        public static List<UserNotification> Get(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserNotifications
                            where item.UserID == userID
                            orderby item.LastUpdate descending
                            select item;

                return query.ToList();
            }
        }

        public static List<UserNotification> GetLatest(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var sd = DateTime.Now.AddDays(-7);

                var query = from item in db.UserNotifications
                            where item.UserID == userID && item.LastUpdate >= sd
                            orderby item.LastUpdate descending
                            select item;

                return query.Take(10).ToList();
            }
        }

        public static int Count(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserNotifications
                            where item.UserID == userID
                            select item;

                return query.Count();
            }
        }

        public static int CountLatest(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var sd = DateTime.Now.AddDays(-7);

                var query = from item in db.UserNotifications
                            where item.UserID == userID && item.LastUpdate >= sd
                            select item;

                return query.Count();
            }
        }

        public static void Insert(UserNotification userNotification)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.UserNotifications.Add(userNotification);

                db.SaveChanges();
            }
        }
    }
}
