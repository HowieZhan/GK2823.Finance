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

namespace Finance.TaskRunner
{
    class Program
    {
        static async Task Main(string[] args)
        {
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
                    services.AddSingleton<IHostedService, ScheduleService>();
                    //}
                    services.Configure<AppSettings>(AppSetting);
                    services.AddHttpClient();
                    services.AddSingleton<DBService>();
                    services.AddSingleton<Service_xuangubao>();
                    services.AddSingleton<MapperService>();
                    services.AddSingleton<IRedisService,RedisService>();
                    AutofacContainer.Build(services);
                });

            await builder.RunConsoleAsync();
        }
    }
}
