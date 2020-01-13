using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineStore.Models
{
    public class Column
    {
        public string DataField { get; set; }

        public string Title { get; set; }
    }

    public class GridViewSettings
    {
        public GridViewSettings()
        {
            this.Editable = true;
            this.ShowAdd = true;
            this.CustomAjax = false;
            this.Deletable = true;
            this.Selectable = false;
        }

        public string ControllerName { get; set; }

        public string AddUrl { get; set; }

        public bool ShowAdd { get; set; }

        public bool CustomAjax { get; set; }

        public List<Column> Columns { get; set; }

        public bool Editable { get; set; }

        public bool Deletable { get; set; }

        public bool Selectable { get; set; }

    }
}