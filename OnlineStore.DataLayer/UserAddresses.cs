using OnlineStore.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Public;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.DataLayer
{
    public class UserAddress : EntityBase
    {
        public string UserID { get; set; }

        [Display(Name = "نام")]
        [MaxLength(50)]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(50)]
        public string Lastname { get; set; }

        [ForeignKey("State")]
        [Display(Name = "استان")]
        public int? StateID { get; set; }
        public City State { get; set; }

        [ForeignKey("City")]
        [Display(Name = "شهر")]
        public int? CityID { get; set; }
        public City City { get; set; }

        [Display(Name = "آدرس محل سکونت")]
        public string Address { get; set; }

        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "شماره ثابت")]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Display(Name = "شماره همراه")]
        [MaxLength(50)]
        public string Mobile { get; set; }

        public bool IsActive { get; set; }
    }

    public static class UserAddresses
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserAddresses
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
                var query = from item in db.UserAddresses
                            select item;

                return query.Count();
            }
        }

        public static UserAddress GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userAddress = db.UserAddresses.Where(item => item.ID == id).Single();

                return userAddress;
            }
        }

        public static List<UserAddress> GetByUserID(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.UserAddresses
                            where item.UserID == userID
                            select item;

                query = query.OrderByDescending(item => item.LastUpdate);

                return query.ToList();
            }
        }

        public static int CountByUserID(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.UserAddresses
                            where item.UserID == userID
                            select item;

                return query.Count();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userAddress = (from item in db.UserAddresses
                                 where item.ID == id
                                 select item).Single();

                db.UserAddresses.Remove(userAddress);

                db.SaveChanges();
            }
        }

        public static void Insert(UserAddress userAddress)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.UserAddresses.Add(userAddress);

                db.SaveChanges();
            }
        }

        public static void Update(UserAddress userAddress)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgUserAddress = db.UserAddresses.Where(item => item.ID == userAddress.ID).Single();

                orgUserAddress.UserID = userAddress.UserID;
                orgUserAddress.Firstname = userAddress.Firstname;
                orgUserAddress.Lastname = userAddress.Lastname;
                orgUserAddress.StateID = userAddress.StateID;
                orgUserAddress.CityID = userAddress.CityID;
                orgUserAddress.Address = userAddress.Address;
                orgUserAddress.PostalCode = userAddress.PostalCode;
                orgUserAddress.Phone = userAddress.Phone;
                orgUserAddress.Mobile = userAddress.Mobile;
                orgUserAddress.IsActive = userAddress.IsActive;

                orgUserAddress.LastUpdate = userAddress.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
