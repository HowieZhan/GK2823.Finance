using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GK2823.ModelLib.Shared
{
    [Table("seeds")]
    public class Seeds
    {
        [Key]
        public int id { get; set; }
        public string seed_no { get; set; }
        public string seed_name { get; set; }
        public DateTime seed_date { get; set; }
        public int is_use_date { get; set; }
        public string last_item { get; set; }
    }
}
