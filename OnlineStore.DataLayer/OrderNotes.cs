using OnlineStore.Identity;
using OnlineStore.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class OrderNote : EntityBase
    {
        [Display(Name = "کاربر")]
        [MaxLength(128)]
        public string UserID { get; set; }

        [ForeignKey("Cart")]
        public int CartID { get; set; }
        public Cart Cart { get; set; }

        [Display(Name = "توضیح")]
        [MaxLength(1000)]
        public string Note { get; set; }
    }

    public static class OrderNotes
    {
        public static List<EditOrderNote> GetByCartID(int cartID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.OrderNotes
                            where item.CartID == cartID
                            select new EditOrderNote
                            {
                                Note = item.Note,
                                UserID = item.UserID,
                                LastUpdate = item.LastUpdate
                            };

                var result = query.ToList();

                foreach (var item in result)
                {
                    var user = OSUsers.GetByID(item.UserID);

                    if (user != null)
                    {
                        item.Username = user.UserName;
                    }
                }

                return result;
            }
        }

        public static void Insert(OrderNote orderNote)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.OrderNotes.Add(orderNote);

                db.SaveChanges();
            }
        }
    }
}
