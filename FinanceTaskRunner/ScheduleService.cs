using GK2823.BizLib.Finance.Services;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.TaskRunner
{
    internal class ScheduleService : IHostedService, IDisposable
    {
        private List<Timer> _taskJobs;

        private readonly IOptions<AppSettings> _appSettings;
        //private readonly IHttpClientFactory _clientFactory;
        private readonly Service_xuangubao _xuangubaoService;
        public ScheduleService()
        {
            _appSettings = AutofacContainer.Resolve<IOptions<AppSettings>>();
            // _clientFactory= AutofacContainer.Resolve<IHttpClientFactory>();
            _xuangubaoService = AutofacContainer.Resolve<Service_xuangubao>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var taskList = _appSettings.Value.rangeTime;
            foreach (var item in taskList)
            {
                if (item.opened == 1)
                {
                    var itemJob = new Timer(DoWork, item.taskName, TimeSpan.Zero, TimeSpan.FromSeconds(item.second));
                    if (_taskJobs == null) _taskJobs = new List<Timer>();
                    _taskJobs.Add(itemJob);
                }
            }
            return Task.CompletedTask;
        }



        private void DoWork(object state)
        {
            try
            {
                switch (state.ToString())
                {
                    case "get_from_xuangubao":
                        _xuangubaoService.GetFromXuangubaoAsync(state.ToString());
                        ; break;
                    case "get_from_xuangubao_0":
                       
                        ; break;
                    case "test":
                        Test();
                        break;
                }
            }
            catch (Exception ex)
            {
                //log
            }
        }

        public void Test()
        {
            int a = 0;
            string b = string.Empty;
            this.GetItems(out a,out b);
        }

        public void GetItems(out int aa, out string bb)
        {
            aa = 1;
            bb = "Hello";
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _taskJobs.ForEach(p =>
            {
                p?.Change(Timeout.Infinite, 0);
            });
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _taskJobs.ForEach(p =>
            {
                p?.Dispose();
            });
        }
    }
}
