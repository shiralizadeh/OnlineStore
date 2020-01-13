using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class ViewAttribute
    {
        public int ID { get; set; }

        public string GroupsTitle { get; set; }

        public int AttrGroupID { get; set; }
        public int? DependentID { get; set; }
        public int? DependentOptionID { get; set; }

        public string AttrGroupTitle { get; set; }

        public string Title { get; set; }

        public AttributeType AttributeType { get; set; }

        public string Posfix { get; set; }

        public string Perfix { get; set; }

        public bool IsSearchable { get; set; }

        public List<ViewOption> Options { get; set; }

        public DateTime LastUpdate { get; set; }

        public object Value { get; set; }

        public int OrderID { get; set; }

        public int GroupOrderID { get; set; }

    }

    public class ViewOption
    {
        public int ID { get; set; }
        public string Title { get; set; }

    }
}
