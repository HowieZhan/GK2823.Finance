using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.ModelLib.Shared
{
    public class MsgResult
    {
        public int code { get; set; }
        public object data { get; set; }
        public string message { get; set; }
        public MsgResult()
        {
            this.code = 200;
            this.message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void SetCustomErr(string msg,int _code=999)
        {
            message = msg;
            code= _code;
        }
    }

    public enum ErrorEnums
    {
        ErrorLogin=402,
        ErrorCustom=999
    }
}
