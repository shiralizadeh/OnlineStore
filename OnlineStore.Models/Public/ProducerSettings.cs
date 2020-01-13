using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ProducerSettings
    {
        public ViewProducer ProducerDetails { get; set; }
        public List<ViewProductGroup> ProductGroups { get; set; }
    }
}
