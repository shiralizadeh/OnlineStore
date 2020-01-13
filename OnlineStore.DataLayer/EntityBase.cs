using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public abstract class EntityBase
    {
        public EntityBase()
        {
            ID = -1;
            LastUpdate = DateTime.Now;
        }

        [Key]
        public int ID { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
