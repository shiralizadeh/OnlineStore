using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class City
    {
        public int ID { get; set; }
        public string Title { get; set; }

        [ForeignKey("Parent")]
        public int? ParentID { get; set; }
        public City Parent { get; set; }
    }

    public static class Cities
    {
        public static List<City> GetRoot()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                return db.Cities.Where(item => item.ParentID == null).OrderBy(item => item.Title).ToList();
            }
        }

        public static IEnumerable GetChilds(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Cities
                            where item.ParentID == id
                            select new
                            {
                                Title = item.Title,
                                ID = item.ID
                            };

                query = query.OrderBy(item => item.Title);

                return query.ToList();

            }
        }

        public static string GetCityName(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                return db.Cities.SingleOrDefault(item => item.ID == id).Title;
            }
        }

    }
}
