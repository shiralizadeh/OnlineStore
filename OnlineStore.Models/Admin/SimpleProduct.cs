using Newtonsoft.Json;
using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class SimpleProduct
    {
        public int ID { get; set; }
        public bool IsUnavailable { get; set; }
        public bool HasVarients { get; set; }
    }
}
