using GK2823.BizLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Buffering;
using System;
using System.Collections.Generic;
using System.Text;
using GK2823.BizLib.Finance.Services;
using Finance.API.Middlewares;
using Microsoft.AspNetCore.Http;
using GK2823.ModelLib.Shared;
using Microsoft.Extensions.Options;


namespace Finance.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var connectionStr = Configuration.GetSection("appConfig")["connstr"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<MapperService>();
            services.AddTransient<Service_xuangubao>();
            services.AddTransient<DBService>();
            services.AddHttpClient();
            services.AddSingleton<IRedisService, RedisService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCors(options =>
            {
                options.AddPolicy("any123", builder =>
                {
#if DEBUG
                    builder.WithOrigins(new string[] { "http://localhost:9000", })
#elif RELEASE
                    builder.WithOrigins(new string[] { "http://fweb.gk2823.com" })
#endif
                     //.AllowAnyMethod()
                     //.AllowAnyHeader()
                     //.AllowCredentials()
                     .SetPreflightMaxAge(TimeSpan.FromSeconds(3600))
                     .WithHeaders("Authorization", "Content-Type", "Access-Control-Allow-Origin")
                      //.WithMethods("GET", "POST", "OPTIONS");
                      .WithMethods("POST");
                });
            });
            AutofacContainer.Build(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseStaticFiles();
            //app.UseHttpsRedirection();
            app.UseCors("any123");
            app.UseRouting();
            app.UseResponseBuffering();
            // app.UseAuthorization();

            //app.UseMiddleware(typeof(AuthExceptionMiddleware));//自定义骚操作
            app.UseMiddleware(typeof(GlobalExceptionMiddleware));//自定义全局异常

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
