using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProductNote
    {
        public int ID { get; set; }
        public string Note { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
