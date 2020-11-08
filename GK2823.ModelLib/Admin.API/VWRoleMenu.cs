using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.ModelLib.Admin.API
{
    [Table("view_role_menu")]
    public class VWRoleMenu
    {
        public int id { get; set; }
        public int sort { get; set; }
        public string role_code { get; set; }
        public string role_name { get; set; }
        public string menu_code { get; set; }
        public string menu_name { get; set; }
        public string icon { get; set; }
        public string part { get; set; }
        public string html_class { get; set; }
        public string parent_code { get; set; }
        public bool open { get; set; }
        [Computed]
        public List<VWRoleMenu> children { get; set; }
    }
}
