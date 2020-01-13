using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditMenuItemBanner
    {
        public int ID { get; set; }
        public string Filename { get; set; }
        public MenuItemBannerType MenuItemBannerType { get; set; }
        public string Link { get; set; }
        public Guid Key { get; set; }

    }
}
