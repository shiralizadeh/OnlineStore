using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class RSSProductsBlog
    {
        public int ID { get; set; }
        public string Title_Fa { get; set; }
        public string Title_En { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public RSSRowType Type { get; set; }
        public string UrlPerfix
        {
            get
            {
                if (Type == RSSRowType.Product)
                {
                    return Title_En + "-" + Title_Fa;
                }
                else
                {
                    return Title_Fa;
                }
            }
        }
        public int? GroupID { get; set; }
        public string GroupTitle { get; set; }
        public DateTime Date { get; set; }
    }
}
