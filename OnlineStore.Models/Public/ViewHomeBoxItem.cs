using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Public
{
    public class ViewHomeBoxItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Filename { get; set; }
        public string Link { get; set; }
    }
}
