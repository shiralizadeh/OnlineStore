using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ProductDetail
    {
        public int ID { get; set; }
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

        public string OtherDisplayTitle
        {
            get
            {
                if (DisplayTitleType == DisplayTitleType.Title_Fa && !String.IsNullOrWhiteSpace(this.Title))
                    return Title_En;
                else if (DisplayTitleType == DisplayTitleType.Title_En && !String.IsNullOrWhiteSpace(this.Title_En))
                    return Title;
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

        public string UrlPerfix
        {
            get
            {
                return Title_En + " " + Title;
            }
        }
        public List<PriceItem> Prices { get; set; }
        public PriceItem MinPrice
        {
            get
            {
                if (Prices != null && Prices.Count > 0)
                    return Prices.OrderBy(item => item.Price).First();
                else
                    return new PriceItem();
            }
        }
        public bool IsUnavailable { get; set; }
        public bool IsFreeDelivery { get; set; }
        public PriceStatus PriceStatus { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }

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
        public float ProductScore { get; set; }
        public bool HasVarients { get; set; }
        public int? GroupID { get; set; }
        public int OrderID { get; set; }
        public int ProducerID { get; set; }
        public DateTime LastUpdate { get; set; }
        public object ProducerTitle { get; set; }
    }
}
