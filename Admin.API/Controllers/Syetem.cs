using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.API.Controllers
{
    public class Syetem:Base
    {
        [HttpPost("api/system/get_server_info")]
        public IActionResult GetServerInfo()
        {
            var msg = new MsgResult();
            msg.data = ComputerHelper.GetComputerInfo();
            return ReJson(msg);
        }

        
    }
}
