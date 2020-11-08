using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.API.Models
{
   public class ReqOasisTableInfo
    {
        public string[] year { get; set; }
        public string[] quarter { get; set; }
        public string[] productLine { get; set; }
        public string[] office { get; set; }
        public string isDownload { get; set; }
    }
}
