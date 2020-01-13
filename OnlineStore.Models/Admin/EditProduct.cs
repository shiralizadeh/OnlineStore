using Newtonsoft.Json;
using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProduct
    {
        public EditProduct()
        {
            ID = -1;

            DisplayTitleType = OnlineStore.Models.Enums.DisplayTitleType.Title_Fa;

            Groups = new List<int>();
            Images = new List<EditProductImage>();
            Files = new List<EditProductFile>();
            Marks = new List<EditProductMark>();
            Points = new List<EditProductPoint>();
            Keywords = new List<EditProductKeyword>();
            Notes = new List<EditProductNote>();
            ProductPricesLinks = new List<EditProductPricesLink>();

            Supply = new EditProductSupply();
            Supplies = new List<EditProductSupply>();

            Price = new EditProductPrice();
            Prices = new List<EditProductPrice>();

            Varients = new List<EditProductVarient>();

            Discounts = new List<EditProductDiscount>();
        }

        public int ID { get; set; }

        public string userID { get; set; }

        [Display(Name = "تولید کننده")]
        public int ProducerID { get; set; }

        [Display(Name = "امتیاز محصول")]
        public float ProductScore { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "عنوان انگلیسی")]
        public string Title_En { get; set; }

        public string UrlPerfix
        {
            get
            {
                return Title_En + " " + Title;
            }
        }

        public string GroupUrlPerfix { get; set; }

        [Display(Name = "عنوان نمایشی")]
        public DisplayTitleType DisplayTitleType { get; set; }

        //[Display(Name = "قیمت")]
        //public int Price { get; set; }

        [Display(Name = "کد قیمت")]
        public string PriceCode { get; set; }

        [Display(Name = "تاریخ انتشار")]
        public DateTime PublishDate { get; set; }

        [Display(Name = "تاریخ انتشار")]
        public string PersianPublishDate
        {
            get
            {
                if (PublishDate == new DateTime())
                    return "";

                return Utilities.ToPersianDate(PublishDate);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    PublishDate = DateTime.Now;

                PublishDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "عدم موجودی")]
        public bool IsUnavailable { get; set; }

        [Display(Name = "عدم نمایش")]
        public bool IsInVisible { get; set; }

        [Display(Name = "ارسال رایگان")]
        public bool IsFreeDelivery { get; set; }

        [Display(Name = "دارای انواع کالا")]
        public bool HasVarients { get; set; }

        [Display(Name = "گروه اصلی")]
        public int? GroupID { get; set; }

        [Display(Name = "گروه ها")]
        public List<int> Groups { get; set; }

        public string JSONGroups
        {
            get
            {
                return JsonConvert.SerializeObject(Groups);
            }
            set
            {
                Groups = JsonConvert.DeserializeObject<List<int>>(value);
            }
        }

        [Display(Name = "متن خلاصه")]
        public string Summary { get; set; }

        [Display(Name = "بررسی تخصصی")]
        public string Text { get; set; }

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

        [Display(Name = "فایل ها")]
        public List<EditProductFile> Files { get; set; }

        public string JSONFiles
        {
            get
            {
                return JsonConvert.SerializeObject(Files);
            }
            set
            {
                Files = JsonConvert.DeserializeObject<List<EditProductFile>>(value);
            }
        }

        [Display(Name = "نشانه ها")]
        public List<EditProductMark> Marks { get; set; }

        public string JSONMarks
        {
            get
            {
                return JsonConvert.SerializeObject(Marks);
            }
            set
            {
                Marks = JsonConvert.DeserializeObject<List<EditProductMark>>(value);
            }
        }

        [Display(Name = "انواع کالا")]
        public List<EditProductVarient> Varients { get; set; }

        public string JSONVarients
        {
            get
            {
                return JsonConvert.SerializeObject(Varients);
            }
            set
            {
                Varients = JsonConvert.DeserializeObject<List<EditProductVarient>>(value);
            }
        }

        public List<EditProductSupply> Supplies { get; set; }

        public EditProductSupply Supply { get; set; }

        [Display(Name = "نقاط قوت و ضعف")]
        public List<EditProductPoint> Points { get; set; }

        public string JSONPoints
        {
            get
            {
                return JsonConvert.SerializeObject(Points);
            }
            set
            {
                Points = JsonConvert.DeserializeObject<List<EditProductPoint>>(value);
            }
        }

        [Display(Name = "کلیدواژه ها")]
        public List<EditProductKeyword> Keywords { get; set; }

        public string JSONKeywords
        {
            get
            {
                return JsonConvert.SerializeObject(Keywords);
            }
            set
            {
                Keywords = JsonConvert.DeserializeObject<List<EditProductKeyword>>(value);
            }
        }

        [Display(Name = "کامنت ها")]
        public List<EditProductNote> Notes { get; set; }

        public string JSONNotes
        {
            get
            {
                return JsonConvert.SerializeObject(Notes);
            }
            set
            {
                Notes = JsonConvert.DeserializeObject<List<EditProductNote>>(value);
            }
        }

        [Display(Name = "قیمت در سایر فروشگاه ها")]
        public List<EditProductPricesLink> ProductPricesLinks { get; set; }

        public string JSONProductPricesLinks
        {
            get
            {
                return JsonConvert.SerializeObject(ProductPricesLinks);
            }
            set
            {
                ProductPricesLinks = JsonConvert.DeserializeObject<List<EditProductPricesLink>>(value);
            }
        }

        public List<EditProductDiscount> Discounts { get; set; }

        public List<EditProductPrice> Prices { get; set; }

        public EditProductPrice Price { get; set; }

        [Display(Name = "وضعیت")]
        public ProductStatus ProductStatus { get; set; }

        [Display(Name = "وضعیت قیمت")]
        public PriceStatus PriceStatus { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

    }
}
