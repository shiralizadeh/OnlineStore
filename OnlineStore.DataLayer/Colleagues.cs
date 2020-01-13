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
    public class Colleague : EntityBase
    {
        [Display(Name = "نام")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Display(Name = "نام شرکت")]
        [MaxLength(300)]
        public string CompanyName { get; set; }

        [Display(Name = "پست الکترونیک")]
        [MaxLength(300)]
        public string Email { get; set; }

        [Display(Name = "شماره تماس")]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Display(Name = "شماره همراه")]
        [MaxLength(50)]
        public string Mobile { get; set; }

        [Display(Name = "آدرس شرکت")]
        [MaxLength(500)]
        public string CompanyAddress { get; set; }

        [Display(Name = "زمینه همکاری")]
        public string CooperationDescription { get; set; }

        [Display(Name = "توضیحات")]
        public string Text { get; set; }

        [Display(Name = "وضعیت")]
        public ColleagueStatus ColleagueStatus { get; set; }
    }

    public static class Colleagues
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string name)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.Colleagues
                            select new
                            {
                                item.ID,
                                item.FirstName,
                                item.LastName,
                                item.ColleagueStatus,
                                item.LastUpdate,
                            };

                if (!String.IsNullOrWhiteSpace(name))
                    query = query.Where(item => item.FirstName.Contains(name) ||
                                                item.LastName.Contains(name) ||
                                                (item.FirstName + " " + item.LastName).Contains(name));

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string name)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Colleagues
                            select item;

                if (!String.IsNullOrWhiteSpace(name))
                    query = query.Where(item => item.FirstName.Contains(name) ||
                                                item.LastName.Contains(name) ||
                                                (item.FirstName + " " + item.LastName).Contains(name));

                return query.Count();
            }
        }

        public static Colleague GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var colleague = db.Colleagues.Where(item => item.ID == id).Single();

                return colleague;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var colleague = (from item in db.Colleagues
                                  where item.ID == id
                                  select item).Single();

                db.Colleagues.Remove(colleague);

                db.SaveChanges();
            }
        }

        public static void Insert(Colleague colleague)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Colleagues.Add(colleague);

                db.SaveChanges();
            }
        }

        public static void Update(Colleague colleague)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgColleague = db.Colleagues.Where(item => item.ID == colleague.ID).Single();

                orgColleague.ColleagueStatus = colleague.ColleagueStatus;
                orgColleague.LastUpdate = colleague.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
