using GK2823.BizLib.Finance.Services;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Finance.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private IHostEnvironment _hostEnvironment;

        public GlobalExceptionMiddleware(RequestDelegate requestDelegate, IHostEnvironment hostEnvironment)
        {
            this._requestDelegate = requestDelegate;
            this._hostEnvironment = hostEnvironment;
        }

        public static IOptions<AppSettings> _appSettings;

        public static List<string> NoAllowedApiUris;
        public List<string> NoAllow()
        {
            if(NoAllowedApiUris==null)
            {
                NoAllowedApiUris = new List<string>();
                NoAllowedApiUris.Add("/api/getpooldetail");
                NoAllowedApiUris.Add("/api/geteverydaylbs");
                NoAllowedApiUris.Add("/api/geteverydaybrokenlbs");
                NoAllowedApiUris.Add("/api/geteverydaybrokenpercent");
                NoAllowedApiUris.Add("/api/geteverydayuplbs");
            }
            return NoAllowedApiUris;
        }
       
        public async Task Invoke(HttpContext httpContext)
        {
            var apiUri = httpContext.Request.Path.Value.ToLower();

            try
            {
                if (NoAllow().Contains(apiUri))
                {
                    throw new Exception("NotAllowed");
                }
                await _requestDelegate.Invoke(httpContext);
                var features = httpContext.Features;
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
            else
            {
                this.SendErrorEmail(post);
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
