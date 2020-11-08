using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using System.Text;

namespace GK2823.ModelLib.Admin.API
{
    [Table("user_roles")]
    public class UserRole
    {
        [Key]
        public int id { get; set; }
        public string user_id { get; set; }
        public string role_code { get; set; }
    }
}
