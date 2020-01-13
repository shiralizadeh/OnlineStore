using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Providers;
using OnlineStore.Models.Enums;
using Newtonsoft.Json;

namespace OnlineStore.Models.Public
{
    public class JsonProductSearch
    {
        public int ID { get; set; }

        [JsonIgnore]
        public string Title_En { get; set; }

        [JsonIgnore]
        public string Title_Fa { get; set; }
        public string Url { get; set; }
        public string DisplayTitle
        {
            get
            {
                if (DisplayTitleType == DisplayTitleType.Title_Fa && !String.IsNullOrWhiteSpace(Title_Fa))
                    return Title_Fa;
                else if (DisplayTitleType == DisplayTitleType.Title_En && !String.IsNullOrWhiteSpace(Title_En))
                    return Title_En;
                else
                {
                    if (!String.IsNullOrWhiteSpace(Title_Fa))
                        return Title_Fa;
                    else if (!String.IsNullOrWhiteSpace(Title_En))
                        return Title_En;
                    else
                        return "نا مشخص";
                }
            }
        }

        [JsonIgnore]
        public DisplayTitleType DisplayTitleType { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public string Image { get; set; }
        public int? GroupID { get; set; }

        [JsonIgnore]
        public string UrlPerfix
        {
            get
            {
                return Title_En + " " + Title_Fa;
            }
        }
    }
}
