using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewBanner
    {
        public int ID { get; set; }

        public Guid Key { get; set; }

        public string Title { get; set; }

        public string Filename { get; set; }

        public string Link { get; set; }

        public string BannerType { get; set; }

    }
}
