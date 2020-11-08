using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace GK2823.ModelLib.Admin.API
{
    [Table("menu")]
    public class Menu
    {
        public const string TK = "menu";
        [Key]
        public int id { get; set; }
        public int sort { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string code { get; set; }
        public string parent_code { get; set; }
        public string icon { get; set; }
        public string part { get; set; }
        public string html_class { get; set; }
        [Computed]
        public List<Menu> children { get; set; }
    }

  
}
