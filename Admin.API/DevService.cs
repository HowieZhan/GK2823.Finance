using GK2823.BizLib.Admin.Services;
using GK2823.BizLib.Shared;
using GK2823.ModelLib.Admin.API;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Admin.API
{
    public class DevService : IHostedService
    {
        public  AccountService _accountService;
        public void DevInit()
        {
            _accountService = AutofacContainer.Resolve<AccountService>();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            DevInit();
            _accountService.DeleteRole(new List<string>(){ "R202011010001"}, true);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
