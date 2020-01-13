using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
   public class GroupOption
    {
        public int GroupID { get; set; }
        public List<int> Options { get; set; }
    }
}
