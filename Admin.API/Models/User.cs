using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.API.Models
{
    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
        public string mobile { get; set; }
        public string captcha { get; set; }
        public string type { get; set; }
    }
}
