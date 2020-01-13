using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models
{
    public class JsonSuccessResult
    {
        public string[] Errors { get; set; }
        public object Data { get; set; }
        public bool Success { get; set; }
    }
}
