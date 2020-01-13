using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class JsonKeywordProduct
    {
        public int ID { get; set; }
        public int KeywordID { get; set; }
        public int ProductID { get; set; }
        public string Title { get; set; }
    }
}
