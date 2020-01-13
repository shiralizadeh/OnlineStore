using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class PackageDetail
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public float PackageScore { get; set; }
        public int OldPrice { get; set; }
        public int NewPrice { get; set; }
    }
}
