using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.ModelLib.Shared
{
    public class SSOUser
    {
        public int id { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public Dictionary<string, object> others { get; set; }
    }
}
