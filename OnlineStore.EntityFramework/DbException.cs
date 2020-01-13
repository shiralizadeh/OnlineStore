using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.EntityFramework
{
    public class DbException : Exception
    {
        public DbException(List<string> errors)
            : base("رخداد خطا")
        {
            Errors = errors;
        }

        public List<string> Errors { get; set; }
    }
}
