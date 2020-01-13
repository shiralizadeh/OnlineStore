using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class BankData
    {
        public int ResCode { get; set; }
        public int VerifyCode { get; set; }
        public int SettleCode { get; set; }
        public string SaleReferenceID { get; set; }

    }
}
