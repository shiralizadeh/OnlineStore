using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ProductDonwload : EntityBase
    {
        [ForeignKey("ProductFile")]
        public int ProductFileID { get; set; }
        public ProductFile ProductFile { get; set; }

        [MaxLength(128)]
        public string UserID { get; set; }

        [MaxLength(128)]
        public string HashKey { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime LastDownload { get; set; }
    }
}
