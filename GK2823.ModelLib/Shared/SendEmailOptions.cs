using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.ModelLib.Shared
{
    public class SendEmailOptions
    {
        public string title { get; set; }
        public string content { get; set; }
        public string sendTo { get; set; }

        public SendEmailOptions()
        {
            this.title = string.Empty;
            this.sendTo = string.Empty;
            this.content = string.Empty;
        }
    }
}
