using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        public string GetJWT()
        {
            var tokenHeader = Request.Headers["Authorization"].ToString();
            string token = null;
            if (tokenHeader != null && tokenHeader.StartsWith("Bearer"))
            {
                token = tokenHeader.Substring("Bearer ".Length).Trim();
            }
            return token;
        }

        protected async Task<T> GetRequest<T>()
        {
            string errors = string.Empty;
            var request = AutofacContainer.Resolve<IHttpContextAccessor>().HttpContext.Request;
            
                using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8))
                {
                    var content = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<T>(content); ;
                }
            
        }
    }
}
