using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Middlewares
{
    public class AuthExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private IHostEnvironment _hostEnvironment;
        static string[] _allowIPs = { "localhost", "::1" };

        public AuthExceptionMiddleware(RequestDelegate requestDelegate, IHostEnvironment hostEnvironment)
        {
            this._requestDelegate = requestDelegate;
            this._hostEnvironment = hostEnvironment;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                var ip = httpContext.Connection.RemoteIpAddress.ToString();
                if (!_allowIPs.Contains(ip)) throw new Exception("当前IP不允许访问");

                this.CheckAuth(httpContext);

                await _requestDelegate.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = 500;
                httpContext.Response.ContentType = "text/json;charset=utf-8";
                string error = string.Empty;
                if (_hostEnvironment.IsDevelopment())
                {
                    //await _requestDelegate.Invoke(httpContext);
                    error = ex.Message;
                }
                else
                {
                    var json = new { message = ex.Message };
                    error = JsonConvert.SerializeObject(json); ;
                }
                await httpContext.Response.WriteAsync(error);
            }
        }

        private void CheckAuth(HttpContext httpContext)
        {
            var controller = httpContext.Request.RouteValues["controller"].ToString().ToLower();
            var action = httpContext.Request.RouteValues["action"].ToString().ToLower();
            var role = new List<string>();
            var cacheRole = new List<string>();
            throw new Exception("当前用户不允许访问");

        }



    }
}
