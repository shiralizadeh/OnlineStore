using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewCustomerComment
    {
        public string UserName { get; set; }

        public string Text { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public DateTime DateTime { get; set; }

        public string PersianDateTime
        {
            get
            {
                if (DateTime == new DateTime())
                    return "";

                return Utilities.ToPersianDate(DateTime);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    DateTime = DateTime.Now;

                DateTime = Utilities.ToEnglishDate(value);
            }
        }

    }
}
