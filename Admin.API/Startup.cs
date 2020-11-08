using GK2823.BizLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using GK2823.BizLib.Finance.Services;
using Admin.API.Middlewares;
using Microsoft.AspNetCore.Http;
using GK2823.ModelLib.Shared;
using Microsoft.Extensions.Options;
using GK2823.BizLib.Admin.Services;

namespace Admin.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var connectionStr = Configuration.GetSection("appConfig")["connstr"];
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<MapperService>();
            services.AddSingleton<AccountService>();
            services.AddSingleton<H3CService>();
            services.AddSingleton<DBService>();
            services.AddHttpClient();
            services.AddSingleton<IRedisService, RedisService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
#if DEBUG
            //services.AddHostedService<DevService>();
#endif
            services.AddCors(options =>
            {
                options.AddPolicy("fweb", builder =>
                {
#if DEBUG
                    builder.WithOrigins(new string[] { "http://localhost:9004", })
#elif RELEASE
                    builder.WithOrigins(new string[] { "http://admin.gk2823.com" })
#endif      
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials()
                     .SetPreflightMaxAge(TimeSpan.FromSeconds(3600))
                     .WithHeaders("Authorization", "Content-Type", "Access-Control-Allow-Origin", "refresh_token")
                     .WithMethods("GET", "POST", "OPTIONS");                  
                });
            });
            AutofacContainer.Build(services);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("fweb");
            app.UseRouting();
            app.UseMiddleware(typeof(GlobalMiddleware));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
