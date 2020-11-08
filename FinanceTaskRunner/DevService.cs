using Dapper;
using Dapper.Contrib.Extensions;
using GK2823.BizLib.Shared;
using GK2823.ModelLib.Admin.API;
using GK2823.UtilLib.Helpers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.TaskRunner
{
    public class DevService : IHostedService
    {
        private DBService _dBService;
        public void Init()
        {
            _dBService = AutofacContainer.Resolve<DBService>();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //Console.WriteLine("开始!");
            //Init();

            //for (var i = 0; i < 101; i++)
            //{
            //    List<Menu> menus = new List<Menu>();
            //    List<Role> roles = new List<Role>();
            //    List<Task> tasks = new List<Task>();
            //    tasks.Add(
            //        Task.Factory.StartNew(() =>
            //        {
            //            menus = _dBService.AdminDB.GetAllAsync<Menu>().Result.ToList();
            //        })
            //    );
            //    tasks.Add(
            //      Task.Factory.StartNew(() =>
            //      {
            //          roles = _dBService.AdminDB.GetAllAsync<Role>().Result.ToList();
            //      })
            //  );

            //    Task.WaitAll(tasks.ToArray());

            //    menus.ForEach(p =>
            //    {
            //        Console.WriteLine(p.name);
            //    });
            //    roles.ForEach(p =>
            //    {
            //        Console.WriteLine(p.name);
            //    });
            //    Console.WriteLine($"-----------{i}-----------");
            //    _dBService.AdminDB.Close();
            //}

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("结束!");
            return Task.CompletedTask;
        }
    }
}
