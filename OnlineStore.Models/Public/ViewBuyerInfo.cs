using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewBuyerInfo
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string PostalCode { get; set; }

        public string StateName { get; set; }

        public string CityName { get; set; }

        public string HomeAddress { get; set; }
    }
}
