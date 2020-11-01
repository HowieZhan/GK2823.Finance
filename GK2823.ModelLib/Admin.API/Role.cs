using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace GK2823.ModelLib.Admin.API
{
    [Table("role")]
    public class Role
    {
        public const string TK = "role";
        [Key]
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }
}
