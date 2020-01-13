using OnlineStore.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Identity;

namespace OnlineStore.DataLayer
{
    public class ProductNote : EntityBase
    {
        [Display(Name = "کاربر")]
        [MaxLength(128)]
        public string UserID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name = "توضیح")]
        [MaxLength(1000)]
        public string Note { get; set; }
    }

    public static class ProductNotes
    {
        public static List<EditProductNote> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductNotes
                            where item.ProductID == productID
                            select new EditProductNote
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

        public static void Insert(ProductNote productNote)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductNotes.Add(productNote);

                db.SaveChanges();
            }
        }
    }
}
