using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GK2823.ModelLib.Shared
{
    [Table("task_log")]
    public class TaskLog
    {
        public const string seedName = "TL";
        [Key]
        public int id { get; set; }
        public string task_name { get; set; }
        public string remark { get; set; }
        public DateTime last_time { get; set; }
        public int run_count { get; set; }
    }
}
