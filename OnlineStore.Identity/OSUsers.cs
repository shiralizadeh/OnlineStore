using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Identity
{
    public class OSUser : IdentityUser
    {
        [Display(Name = "نام")]
        [MaxLength(50)]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(50)]
        public string Lastname { get; set; }

        [Display(Name = "کد ملی")]
        [MaxLength(10)]
        public string NationalCode { get; set; }

        [Display(Name = "شماره ثابت")]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Display(Name = "شماره همراه")]
        [MaxLength(50)]
        public string Mobile { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "جنسیت")]
        public bool? Gender { get; set; }

        [Display(Name = "استان")]
        public int? StateID { get; set; }

        [Display(Name = "شهر")]
        public int? CityID { get; set; }

        [Display(Name = "آدرس محل سکونت")]
        [MaxLength(500)]
        public string HomeAddress { get; set; }

        [Display(Name = "کد پستی")]
        [MaxLength(10)]
        public string PostalCode { get; set; }

        [Display(Name = "شماره کارت")]
        [MaxLength(50)]
        public string CardNumber { get; set; }

        [Display(Name = "تصویر کاربر")]
        [MaxLength(50)]
        public string ImageFile { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastUpdate { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<OSUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            return userIdentity;
        }
    }

    public static class OSUsers
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string userName, string fullName, string email, bool? isActive)
        {
            using (var db = IdentityDbContext.Entity)
            {
                var query = from item in db.Users
                            select new
                            {
                                ID = item.Id,
                                item.UserName,
                                item.Firstname,
                                item.Lastname,
                                item.Email,
                                item.IsActive,
                                item.LastUpdate,
                            };

                if (!String.IsNullOrWhiteSpace(userName))
                    query = query.Where(item => item.UserName.Contains(userName));

                if (!String.IsNullOrWhiteSpace(fullName))
                    query = query.Where(item => item.Firstname.Contains(fullName)
                                             || item.Lastname.Contains(fullName)
                                             || (item.Firstname + " " + item.Lastname).Contains(fullName));

                if (!String.IsNullOrWhiteSpace(email))
                    query = query.Where(item => item.Email.Contains(email));

                if (isActive.HasValue)
                    query = query.Where(item => item.IsActive == isActive);

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string userName, string fullName, string email, bool? isActive)
        {
            using (var db = IdentityDbContext.Entity)
            {
                var query = from item in db.Users
                            select item;

                if (!String.IsNullOrWhiteSpace(userName))
                    query = query.Where(item => item.UserName.Contains(userName));

                if (!String.IsNullOrWhiteSpace(fullName))
                    query = query.Where(item => item.Firstname.Contains(fullName)
                                             || item.Lastname.Contains(fullName)
                                             || (item.Firstname + " " + item.Lastname).Contains(fullName));

                if (!String.IsNullOrWhiteSpace(email))
                    query = query.Where(item => item.Email.Contains(email));

                if (isActive.HasValue)
                    query = query.Where(item => item.IsActive == isActive);

                return query.Count();
            }
        }

        public static OSUser GetByID(string id)
        {
            using (var db = IdentityDbContext.Entity)
            {
                var osUser = db.Users.Where(item => item.Id == id).Single();

                return osUser;
            }
        }

        public static void Delete(string id)
        {
            using (var db = IdentityDbContext.Entity)
            {
                var osUser = (from item in db.Users
                              where item.Id == id
                              select item).Single();

                db.Users.Remove(osUser);

                db.SaveChanges();
            }
        }

        public static void Insert(OSUser osUser)
        {
            using (var db = IdentityDbContext.Entity)
            {
                db.Users.Add(osUser);

                db.SaveChanges();
            }
        }

        public static void Update(OSUser osUser)
        {
            using (var db = IdentityDbContext.Entity)
            {
                var orgOSUser = db.Users.Where(item => item.Id == osUser.Id).Single();

                orgOSUser.UserName = osUser.UserName;
                orgOSUser.IsActive = osUser.IsActive;
                orgOSUser.LastUpdate = osUser.LastUpdate;

                db.SaveChanges();
            }
        }
        
    }
}
