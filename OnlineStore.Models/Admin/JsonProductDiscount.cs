using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Admin
{
    public class JsonProductDiscount
    {
        public int ID { get; set; }
        public int? ProductID { get; set; }
        public string ProductTitle { get; set; }
        public int? GroupID { get; set; }
        public string GroupTitle { get; set; }
        public string RoleID { get; set; }
        public string RoleTitle { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float Percent { get; set; }
        public ProductDiscountStatus ProductDiscountStatus { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
