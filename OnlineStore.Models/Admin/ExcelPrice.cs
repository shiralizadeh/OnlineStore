using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class ExcelPrice
    {
        public int? PriceID { get; set; }

        public int? VarientID { get; set; }

        public string PriceCode { get; set; }

        public int ProductID { get; set; }

        public string Title { get; set; }

        public PriceType PriceType { get; set; }

        public int Price { get; set; }

        public int NewPrice { get; set; }

    }
}
