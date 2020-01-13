using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class ViewProducer
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string GroupsTitle { get; set; }

        public string TitleEn { get; set; }

        public string Filename { get; set; }

        public int OrderID { get; set; }

        public bool IsVisible { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
