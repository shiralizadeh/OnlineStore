using Newtonsoft.Json;
using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditPackage
    {
        public EditPackage()
        {
            ID = -1;

            Images = new List<EditProductImage>();
            Products = new List<EditPackageProduct>();
        }

        public int ID { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(300)]
        public string Title { get; set; }

        [Display(Name = "متن")]
        public string Text { get; set; }

        [Display(Name = "تاریخ شروع")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاریخ شروع")]
        public string PersianStartDate
        {
            get
            {
                if (StartDate == new DateTime())
                    return "";

                return Utilities.ToPersianDate(StartDate);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    StartDate = DateTime.Now;

                StartDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "تاریخ پایان")]
        public DateTime EndDate { get; set; }

        [Display(Name = "تاریخ پایان")]
        public string PersianEndDate
        {
            get
            {
                if (EndDate == new DateTime())
                    return "";

                return Utilities.ToPersianDate(EndDate);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    EndDate = DateTime.Now;

                EndDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "وضعیت")]
        public PackageStatus PackageStatus { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        public DateTime LastUpdate { get; set; }

        [Display(Name = "امتیاز بسته")]
        public float PackageScore { get; set; }

        [Display(Name = "عکس ها")]
        public List<EditProductImage> Images { get; set; }
        public string JSONImages
        {
            get
            {
                return JsonConvert.SerializeObject(Images);
            }
            set
            {
                Images = JsonConvert.DeserializeObject<List<EditProductImage>>(value);
            }
        }

        [Display(Name = "محصولات")]
        public List<EditPackageProduct> Products { get; set; }
        public string JSONProducts
        {
            get
            {
                return JsonConvert.SerializeObject(Products);
            }
            set
            {
                Products = JsonConvert.DeserializeObject<List<EditPackageProduct>>(value);
            }
        }
    }
}
