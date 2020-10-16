using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using System.Text;

namespace GK2823.ModelLib.Finance.API
{
    [Table("view_everyday_lbs")]
   public  class EverydayLBS
    {
        public int lb_count { get; set; }
        [Key]
        public string stime { get; set; }
        public string stock_chi_name { get; set; }
    }
}
