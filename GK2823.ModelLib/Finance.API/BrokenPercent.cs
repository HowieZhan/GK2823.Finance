using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using System.Text;

namespace GK2823.ModelLib.Finance.API
{
    [Table("view_broken_percent")]
    public class BrokenPercent
    {
        [Key]
        public string stime { get; set; }
        public decimal percent { get; set; }
    }
}
