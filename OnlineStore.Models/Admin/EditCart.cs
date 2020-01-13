using Newtonsoft.Json;
using OnlineStore.Models.Enums;
using OnlineStore.Models.User;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditCart
    {
        public EditCart()
        {
            ID = -1;
            Notes = new List<EditOrderNote>();
        }

        [Display(Name = "کد سفارش")]
        public int ID { get; set; }

        [Display(Name = "روش ارسال")]
        public SendMethodType SendMethodType { get; set; }

        [Display(Name = "روش پرداخت")]
        public PaymentMethodType PaymentMethodType { get; set; }

        [Display(Name = "مبلغ کل")]
        public int? Total { get; set; }

        [Display(Name = "مبلغ قابل پرداخت")]
        public int? ToPay { get; set; }

        [Display(Name = "وضعیت سفارش")]
        public CartStatus CartStatus { get; set; }

        [Display(Name = "وضعیت ارسال")]
        public SendStatus SendStatus { get; set; }

        [Display(Name = "وضعیت مالی")]
        public ConfirmationStatus ConfirmationStatus { get; set; }

        [Display(Name = "تاریخ تاییدیه مالی")]
        public DateTime? ConfirmationDate { get; set; }

        [Display(Name = "تاریخ تاییدیه مالی")]
        public string PersianConfirmationDate
        {
            get
            {
                if (ConfirmationDate == new DateTime() || !ConfirmationDate.HasValue)
                    return "";

                return Utilities.ToPersianDate(ConfirmationDate.Value);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    ConfirmationDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "تاریخ ارسال")]
        public DateTime? SendDate { get; set; }

        [Display(Name = "تاریخ ارسال")]
        public string PersianSendDate
        {
            get
            {
                if (SendDate == new DateTime() || !SendDate.HasValue)
                    return "";

                return Utilities.ToPersianDate(SendDate.Value);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    SendDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "تاریخ دریافت")]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "تاریخ دریافت")]
        public string PersianDeliveryDate
        {
            get
            {
                if (DeliveryDate == new DateTime() || !DeliveryDate.HasValue)
                    return "";

                return Utilities.ToPersianDate(DeliveryDate.Value);

            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    DeliveryDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "میزان مالیات")]
        public int Tax { get; set; }

        [Display(Name = "هزینه پست")]
        public int DelivaryPrice { get; set; }

        [Display(Name = "کد پیگیری")]
        public string SaleReferenceID { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "توضیحات خریدار")]
        public string UserDescription { get; set; }

        [Display(Name = "تاریخ سفارش")]
        public DateTime? DateTime { get; set; }

        [Display(Name = "شماره قبض")]
        public string BillNumber { get; set; }

        [Display(Name = "کامنت ها")]
        public List<EditOrderNote> Notes { get; set; }

        public string JSONNotes
        {
            get
            {
                return JsonConvert.SerializeObject(Notes);
            }
            set
            {
                Notes = JsonConvert.DeserializeObject<List<EditOrderNote>>(value);
            }
        }

        public List<ViewOrderItem> OrderItems { get; set; }

    }
}
