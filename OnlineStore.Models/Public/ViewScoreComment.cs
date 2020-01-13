using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewScoreComment
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int ProductID { get; set; }
        public string Text { get; set; }
        public List<ScoreValue> ScoreValues { get; set; }
        public DateTime LastUpdate { get; set; }
        public int LikesCount { get; set; }
        public int DisLikesCount { get; set; }
    }
}
