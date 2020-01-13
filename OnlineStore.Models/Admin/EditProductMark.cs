using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProductMark
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Color { get; set; }

        public DateTime StartDate { get; set; }

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

        public DateTime EndDate { get; set; }

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
    }
}
