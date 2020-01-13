using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineStore.Models
{
    public enum TreeViewSelectMode
    {
        Single,
        Multiple
    }

    public class TreeViewSettings
    {
        public TreeViewSettings()
        {
            this.CurrentID = -1;
            this.TreeViewSelectMode = TreeViewSelectMode.Single;
        }

        public TreeViewSelectMode TreeViewSelectMode { get; set; }

        public string FieldName { get; set; }

        public string ControllerName { get; set; }

        public string JsonMothod { get; set; }

        public int? CurrentID { get; set; }

        public int? _SelectedID;
        public int? SelectedID
        {
            get
            {
                return _SelectedID;
            }
            set
            {
                if (value.HasValue)
                    _SelectedID = value;
                else
                    _SelectedID = -1;
            }
        }

        public List<int> SelectedItems { get; set; }
    }

    public class TreeItem
    {
        public TreeItem()
        {
            this.open = false;
        }

        public List<TreeItem> branch { get; set; }

        public int id { get; set; }

        public string label { get; set; }

        public bool checkbox { get; set; }

        public bool inode { get; set; }

        public bool open { get; set; }

        public bool radio { get; set; }
    }
}