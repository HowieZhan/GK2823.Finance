using System;
using Microsoft.Extensions.Hosting;//HostBuilder
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.IO;
using GK2823.UtilLib.Helpers;
using System.Net.Http;
using Microsoft.Extensions.Http;
using GK2823.BizLib.Finance.Services;
using GK2823.BizLib.Shared;
using GK2823.ModelLib.Shared;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Finance.TaskRunner
{
    class Program
    {
        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            //Console.WriteLine(e.ExceptionObject.ToString());
            //Console.WriteLine("Press Enter to continue");
            //Console.ReadLine();
            //Environment.Exit(1);
            //位置错误
        }
        static async Task Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());



                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                    
                })
                .ConfigureServices((hostContext, services) =>
                {
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

                   
                    //int scheduleSwitch = int.Parse(AppSetting["AppConfigs:ScheduleSwitch"]);
                    //if (scheduleSwitch.Equals(1))
                    //{
                   
                    
                    //}
                    services.Configure<AppSettings>(AppSetting);
                    services.AddHttpClient();
                    services.AddSingleton<DBService>();
                    services.AddSingleton<Service_xuangubao>();
                    services.AddSingleton<MapperService>();
                    services.AddSingleton<IRedisService,RedisService>();
                    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

#if DEBUG
                    services.AddHostedService<DevService>();
#endif
                    services.AddSingleton<IHostedService, TimeJob>();
                    AutofacContainer.Build(services);
                    
                });
           
            await builder.RunConsoleAsync();
        }
    }
}
