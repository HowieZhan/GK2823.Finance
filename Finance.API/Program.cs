using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using GK2823.ModelLib.Shared;

namespace Finance.API
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
   WebHost.CreateDefaultBuilder(args)
       .UseStartup<Startup>()
           .ConfigureAppConfiguration((context, build) => {


            }).ConfigureServices((hostContext, services) => {
                services.AddOptions();

#if DEBUG
                var AppSetting = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.debug.json")
                                .Build();

#elif RELEASE
                var AppSetting = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json")
                                    .Build();
#endif
                services.Configure<AppSettings>(AppSetting);

            })
       .UseKestrel(options =>
       {
           options.Listen(IPAddress.Loopback, 9001);
           //options.Listen(IPAddress.Loopback, 5001, listenOptions =>
           //{
           //    listenOptions.UseHttps("testCert.pfx", "testPassword");
           //});
       })
       .Build();
    }
}
