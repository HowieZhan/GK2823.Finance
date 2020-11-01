using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace GK2823.ModelLib.Admin.API
{
    [Table("role_menu")]
   public class RoleMenu
    {
        [Key]
        public int id { get; set; }
        public string role_code { get; set; }
        public string menu_code { get; set; }
    }
}
