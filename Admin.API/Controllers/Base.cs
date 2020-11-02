using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.API.Controllers
{
    public class Base : ControllerBase
    {
        protected readonly IOptions<AppSettings> _appsettings;
        public Base()
        {
            if(_appsettings==null)
            {
                _appsettings = AutofacContainer.Resolve<IOptions<AppSettings>>();
            }
        }

        [HttpGet("api/apiname")]
        public IActionResult ApiName()
        {
            var name = _appsettings.Value.appName;
            return Ok(new { name });
        }

        public ContentResult ReJson(object ob)
        {
            return base.Content(JsonConvert.SerializeObject(ob),"application/json");
        }
    }
}
