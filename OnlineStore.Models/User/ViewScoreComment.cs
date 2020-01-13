using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.User
{
    public class ViewScoreComment
    {
        public int ID { get; set; }
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
        public DisplayTitleType DisplayTitleType { get; set; }
        public string Title_Fa { get; set; }
        public string Title_En { get; set; }
        public string Text { get; set; }
        public int LikeCount { get; set; }
        public int DisLikeCount { get; set; }
        public ScoreCommentStatus ScoreCommentStatus { get; set; }
    }
}
