using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.DataLayer
{
    public class PaymentLog : EntityBase
    {
        [ForeignKey("Cart")]
        public int CartID { get; set; }
        public Cart Cart { get; set; }

        [MaxLength(15)]
        public string IP { get; set; }
        public BankType BankType { get; set; }
        public DateTime ConnectDate { get; set; }
        public DateTime? SettleDate { get; set; }
        public int Price { get; set; }
        public CartStatus PaymentStatus { get; set; }

        [MaxLength(200)]
        public string KeyID { get; set; }
        public string Data { get; set; }

    }

    public static class PaymentLogs
    {
        public static void Insert(PaymentLog paymentLog)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.PaymentLogs.Add(paymentLog);

                db.SaveChanges();
            }
        }

        public static void UpdateByOrderID(PaymentLog paymentLog)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgPaymentLog = db.PaymentLogs.Where(item => item.KeyID == paymentLog.KeyID).Single();

                orgPaymentLog.SettleDate = paymentLog.SettleDate;
                orgPaymentLog.PaymentStatus = paymentLog.PaymentStatus;
                orgPaymentLog.Data = paymentLog.Data;
                orgPaymentLog.LastUpdate = paymentLog.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
