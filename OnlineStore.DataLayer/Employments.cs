using OnlineStore.Models.Enum;
using OnlineStore.Models.Public;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Enums;

namespace OnlineStore.DataLayer
{
    public class Employment : EntityBase
    {
        [Display(Name = "نام")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Display(Name = "نام پدر")]
        [MaxLength(100)]
        public string FatherName { get; set; }

        [Display(Name = "جنسیت")]
        public bool Gender { get; set; }

        [Display(Name = "سن")]
        public int Age { get; set; }

        [Display(Name = "شماره شناسنامه")]
        [MaxLength(11)]
        public string IDNumber { get; set; }

        [Display(Name = "کد ملی")]
        [MaxLength(11)]
        public string NationalCode { get; set; }

        [Display(Name = "ملیت")]
        [MaxLength(20)]
        public string Nationality { get; set; }

        [Display(Name = "دین")]
        [MaxLength(20)]
        public string Religion { get; set; }

        [Display(Name = "وضعیت تاهل")]
        public bool MarriedStatus { get; set; }

        [Display(Name = "وضعیت نظام وظیفه")]
        public MartialStatus? MartialStatus { get; set; }

        [Display(Name = "پست الکترونیک")]
        [MaxLength(300)]
        public string Email { get; set; }

        [Display(Name = "شماره تماس")]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Display(Name = "شماره همراه")]
        [MaxLength(50)]
        public string Mobile { get; set; }

        [Display(Name = "آدرس")]
        [MaxLength(500)]
        public string Address { get; set; }

        [Display(Name = "مدرک تحصیلی")]
        public StudyDegree StudyDegree { get; set; }

        [Display(Name = "رشته تحصیلی")]
        [MaxLength(100)]
        public string StudyMajor { get; set; }

        [Display(Name = "توضیحات")]
        public string StudyText { get; set; }

        [Display(Name = "سوابق شغلی")]
        public string ResumeText { get; set; }

        [Display(Name = "دوره های آموزشی")]
        public string EducationCoursesText { get; set; }

        [Display(Name = "زبان مسلط")]
        public Language? DominantLanguages { get; set; }

        [Display(Name = "وضعیت")]
        public EmploymentStatus EmploymentStatus { get; set; }

        [Display(Name = "فایل رزومه")]
        [MaxLength(300)]
        public string ResumeFile { get; set; }

        [Display(Name = "تاریخ درخواست")]
        public DateTime DateTime { get; set; }

    }

    public static class Employments
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string name)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.Employments
                            select new
                            {
                                item.ID,
                                item.FirstName,
                                item.LastName,
                                item.EmploymentStatus,
                                item.DateTime,
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
                var query = from item in db.Employments
                            select item;

                if (!String.IsNullOrWhiteSpace(name))
                    query = query.Where(item => item.FirstName.Contains(name) ||
                                                item.LastName.Contains(name) ||
                                                (item.FirstName + " " + item.LastName).Contains(name));

                return query.Count();
            }
        }

        public static Employment GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var employment = db.Employments.Where(item => item.ID == id).Single();

                return employment;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var employment = (from item in db.Employments
                                  where item.ID == id
                                  select item).Single();

                db.Employments.Remove(employment);

                db.SaveChanges();
            }
        }

        public static void Insert(Employment employment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Employments.Add(employment);

                db.SaveChanges();
            }
        }

        public static void Update(Employment employment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgEmployment = db.Employments.Where(item => item.ID == employment.ID).Single();

                orgEmployment.EmploymentStatus = employment.EmploymentStatus;
                orgEmployment.LastUpdate = employment.LastUpdate;

                db.SaveChanges();
            }
        }

    }

}
