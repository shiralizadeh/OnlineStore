using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Public;

namespace OnlineStore.Models.User
{
    public class JsonUserScore
    {
        public string Text { get; set; }
        public List<ScoreValue> ScoreValues { get; set; }

    }
}
