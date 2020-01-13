using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewPackageProduct
    {
        public int ProductID { get; set; }
        public int GroupID { get; set; }
        public DisplayTitleType DisplayTitleType { get; set; }
        public string Title { get; set; }
        public string Title_En { get; set; }
        public string DisplayTitle
        {
            get
            {
                if (DisplayTitleType == DisplayTitleType.Title_Fa && !String.IsNullOrWhiteSpace(this.Title))
                    return Title;
                else if (DisplayTitleType == DisplayTitleType.Title_En && !String.IsNullOrWhiteSpace(this.Title_En))
                    return Title_En;
                else
                {
                    if (!String.IsNullOrWhiteSpace(Title))
                        return Title;
                    else if (!String.IsNullOrWhiteSpace(Title_En))
                        return Title_En;
                    else
                        return "نا مشخص";
                }
            }
        }
        public int? ProductVarientID { get; set; }
        public string ProductVarientTitle { get; set; }
        public int OldPrice { get; set; }
        public int NewPrice { get; set; }
        public string ImageFile { get; set; }
        public string UrlPerfix
        {
            get
            {
                return Title_En + "-" + Title;
            }
        }
    }
}
