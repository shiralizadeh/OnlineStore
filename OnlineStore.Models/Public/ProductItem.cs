using Newtonsoft.Json;
using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ProductItem
    {
        public ProductItem()
        {
            Marks = new List<MarkItem>();
        }

        public int ID { get; set; }

        public int? GroupID { get; set; }

        public string GroupTitle { get; set; }
        public string GroupTitle_En { get; set; }

        public string Title_Fa { get; set; }
        public string Title_En { get; set; }

        public string ToolTip
        {
            get
            {
                return DisplayTitleType == DisplayTitleType.Title_Fa ? Title_En : Title_Fa;
            }
        }

        public string Url { get; set; }
        //private string _Url;
        //public string Url
        //{
        //    get
        //    {
        //        if (String.IsNullOrWhiteSpace(Url))
        //            _Url = UrlProvider.GetProductUrl(this.ID, this.UrlPerfix, this.UrlPerfix);
        //    }
        //}

        public string UrlPerfix
        {
            get
            {
                return Title_En + "-" + Title_Fa;
            }
        }

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
        public DisplayTitleType DisplayTitleType { get; set; }
        public bool HasVarients { get; set; }
        public List<PriceItem> Prices { get; set; }
        public int CommentCount { get; set; }
        public string ImageFile { get; set; }
        public List<MarkItem> Marks { get; set; }

        private float? _sumScore;
        public float? SumScore
        {
            get
            {
                return _sumScore;
            }
            set
            {
                _sumScore = value ?? 0;
            }
        }
        public int ScoreCount { get; set; }
        public bool IsUnavailable { get; set; }
        public PriceStatus PriceStatus { get; set; }
        public float ProductScore { get; set; }
        public DateTime LastUpdate { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public string Summary { get; set; }
    }

    public class MarkItem
    {
        public string Title { get; set; }
        public string Color { get; set; }
    }
}
