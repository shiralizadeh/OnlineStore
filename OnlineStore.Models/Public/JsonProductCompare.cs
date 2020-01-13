using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;
using Newtonsoft.Json;

namespace OnlineStore.Models.Public
{
    public class JsonProductCompare
    {
        public List<ViewAttribute> Attributes { get; set; }
        public List<ScoresAverage> ScoresAverages { get; set; }

        [JsonIgnore]
        public DisplayTitleType DisplayTitleType { get; set; }

        [JsonIgnore]
        public string Title_Fa { get; set; }

        [JsonIgnore]
        public string Title_En { get; set; }
        public string ToolTip
        {
            get
            {
                return DisplayTitleType == DisplayTitleType.Title_Fa ? Title_En : Title_Fa;
            }
        }
        public string Url { get; set; }
        public string DisplayTitle
        {
            get
            {
                if (DisplayTitleType == DisplayTitleType.Title_Fa && !String.IsNullOrWhiteSpace(this.Title_Fa))
                    return Title_Fa;
                else if (DisplayTitleType == DisplayTitleType.Title_En && !String.IsNullOrWhiteSpace(this.Title_En))
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
        public float Score { get; set; }
        public List<PriceItem> Prices { get; set; }
        public string Image { get; set; }
        public bool IsUnavailable { get; set; }
        public int ProductID { get; set; }
        public int GroupID { get; set; }
        public int PreferentialsCount { get; set; }
    }
}
