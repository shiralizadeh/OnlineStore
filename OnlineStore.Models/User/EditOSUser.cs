using OnlineStore.Providers;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.User
{
    public class EditOSUser
    {
        public string Id { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [Display(Name = "نام")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string Lastname { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "شماره ثابت")]
        public string Phone { get; set; }

        [Display(Name = "شماره همراه")]
        public string Mobile { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "تاریخ تولد")]
        public string PersianBirthDate
        {
            get
            {
                if (BirthDate == new DateTime() || !BirthDate.HasValue)
                    return "";

                return Utilities.ToPersianDate(BirthDate.Value);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    BirthDate = DateTime.Now;
                else
                    BirthDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "جنسیت")]
        public bool? Gender { get; set; }

        [Display(Name = "استان")]
        public int? StateID { get; set; }

        [Display(Name = "شهر")]
        public int? CityID { get; set; }

        [Display(Name = "آدرس محل سکونت")]
        public string HomeAddress { get; set; }

        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "شماره کارت")]
        public string CardNumber { get; set; }

        [Display(Name = "عکس")]
        public string ImageFile { get; set; }

        [Display(Name = "آخرین ویرایش")]
        public DateTime LastUpdate { get; set; }
    }
}
