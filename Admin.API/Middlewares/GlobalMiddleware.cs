using GK2823.BizLib.Finance.Services;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Admin.API.Middlewares
{
    public class GlobalMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private IHostEnvironment _hostEnvironment;

        public GlobalMiddleware(RequestDelegate requestDelegate, IHostEnvironment hostEnvironment)
        {
            this._requestDelegate = requestDelegate;
            this._hostEnvironment = hostEnvironment;
        }

        public static IOptions<AppSettings> _appSettings;

        public static List<string> NoAllowedApiUris;
        public List<string> NoAllow()
        {
            if (NoAllowedApiUris == null)
            {
                NoAllowedApiUris = new List<string>();
            }
            return NoAllowedApiUris;
        }

        public static List<string> AuthUris;
        public List<string> GetPubAuthUri()
        {
            if (AuthUris == null)
            {
                AuthUris = new List<string>();
                AuthUris.Add("/api/login");
                AuthUris.Add("/h3c/oasis_table_info");
            }
            return AuthUris;
        }

        
        private int TOpenJwtToken(string token,string apiUri, out string strInfo)
        {
            int info = 0;
            strInfo = string.Empty;
            var k = new JwtSecurityToken(token);
            var end = k.Claims.Where(p => p.Type == "exp").FirstOrDefault().Value;
            var expired = TimeHelper.ConvertToDateTime(Convert.ToInt32(end));
            if (DateTime.Now < expired)
            {
                info = 2;//正常
            }
            else
            {
                
                var start = k.Claims.Where(p => p.Type == "nbf").FirstOrDefault().Value;
                var started = TimeHelper.ConvertToDateTime(Convert.ToInt32(start));
                TimeSpan ts = expired.Subtract(started);
                int sec = (int)ts.TotalSeconds;
                if (DateTime.Now < expired.AddSeconds(sec * 2))
                {
                    if (apiUri == refreshtoken)
                    {
                        info = 2;//通过刷新API
                    }
                    else
                    {
                        info = 1;//需要刷新，但仍然正常返回
                        strInfo = k.Claims.Where(p => p.Type == "gk2823").FirstOrDefault().Value;
                    }
                }
                else
                {
                    info = 0;//超时
                }
            }
            return info;
        }
        public const string refreshtoken = "/api/refreshtoken";
        public async Task Invoke(HttpContext httpContext)
        {
            var apiUri = httpContext.Request.Path.Value.ToLower();

            try
            {
                if (NoAllow().Contains(apiUri))
                {
                    throw new Exception("NotAllowed");
                }
                if (!GetPubAuthUri().Contains(apiUri))
                {
                    var tokenHeader = httpContext.Request.Headers["Authorization"].ToString();
                    string token = null;
                    if (tokenHeader != null && tokenHeader.StartsWith("Bearer"))
                    {
                        token = tokenHeader.Substring("Bearer ".Length).Trim();
                    }
                    else
                    {
                        throw new Exception(((int)System.Net.HttpStatusCode.Unauthorized).ToString());
                    }
                    string strInfo = string.Empty;
                    var checkResult = this.TOpenJwtToken(token, apiUri, out strInfo);
                    if (checkResult == 0)
                    {
                        throw new Exception(((int)System.Net.HttpStatusCode.Unauthorized).ToString());
                    }
                    else if (checkResult == 1 || apiUri == "/api/gettest")
                    {
                        httpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.RequestTimeout;
                        //httpContext.Response.Headers.Add("refresh_token", JwtHelper.CreatJwtToken(strInfo));
                    }

                    // httpContext.Response.Headers.Add("Content-Type", "application/json; charset=utf-8; "+JwtHelper.CreatJwtToken("2121e32"));

                }
                await _requestDelegate.Invoke(httpContext);
                //var features = httpContext.Features;
            }
            catch (Exception ex)
            {
                _appSettings = AutofacContainer.Resolve<IOptions<AppSettings>>();
                await this.HandleException(httpContext, ex);
            }
        }

        private async Task HandleException(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "text/json;charset=utf-8";
            string error = string.Empty;
#if DEBUG
            var json = new { message = ex.Message, trace = ex.StackTrace };
            error = JsonConvert.SerializeObject(json);
#elif RELEASE
            error = "出错了";
            var json = new { message = ex.Message, trace = ex.StackTrace };
#endif
            NLogHelper.GetLogger().Info(error);
            //rabbitmq就绪 json
            var post = new SendEmailOptions
            {
                title = "位置异常",
                content = JsonConvert.SerializeObject(json)
            };
            if (ex.Message == "NotAllowed")
            {
                await httpContext.Response.WriteAsync(ex.Message);
            }
            else if (ex.Message == ((int)System.Net.HttpStatusCode.Unauthorized).ToString())
            {
                var msg = new MsgResult();
                msg.code = (int)System.Net.HttpStatusCode.Unauthorized;
                msg.data = "请登录";
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(msg));
            }
            else
            {
#if RELEASE
                this.SendErrorEmail(post);
#endif
                await httpContext.Response.WriteAsync(error);
            }
        }

        protected void SendErrorEmail(SendEmailOptions sendEmailOptions)
        {

            try
            {
                sendEmailOptions.sendTo = string.IsNullOrEmpty(sendEmailOptions.sendTo) ?
                    _appSettings.Value.MainEmail.UserAddress : sendEmailOptions.sendTo;
                var succ = GK2823.BizLib.Shared.RabbitMQHelper.Send(
                     _appSettings.Value.MQHandle.DefaultQueue.QueueName,
                     System.Text.Json.JsonSerializer.Serialize(sendEmailOptions),
                     _appSettings.Value.MQHandle.DefaultQueue.MQFunction.SendEmail);
                if (!succ)
                {
                    throw (new Exception("自定义消息队列异常-From Web"));
                }
            }
            catch (Exception ex)
            {
                NLogHelper.GetLogger().Info("消息队列异常-From Web:" + ex.Message + ex.StackTrace);
                AutofacContainer.Resolve<Service_xuangubao>().SendEmail(sendEmailOptions.sendTo, sendEmailOptions.title, sendEmailOptions.content);

            }
        }

    }
}
