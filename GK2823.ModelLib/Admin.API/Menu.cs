using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace GK2823.ModelLib.Admin.API
{
    [Table("menu")]
    public class Menu
    {
        [Key]
        public int id { get; set; }
        public int sort { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string code { get; set; }
        public string parent_code { get; set; }
        [Computed]
        public List<Menu> children { get; set; }
    }

  
}
