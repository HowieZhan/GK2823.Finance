using GK2823.BizLib.Finance.Services;
using GK2823.UtilLib.Helpers;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.TaskRunner
{
    public class FinanceUpJob : IJob
    {
        private readonly Service_xuangubao _xuangubaoService;
        public FinanceUpJob()
        {
            _xuangubaoService = AutofacContainer.Resolve<Service_xuangubao>();
        }
        public Task Execute(IJobExecutionContext context)
        {
            _xuangubaoService.GetFromXuangubaoAsync("get_from_xuangubao");
            return Task.CompletedTask;
        }
    }

    public class FinanceBrokenJob : IJob
    {
        private readonly Service_xuangubao _xuangubaoService;
        public FinanceBrokenJob()
        {
            _xuangubaoService = AutofacContainer.Resolve<Service_xuangubao>();
        }
        public Task Execute(IJobExecutionContext context)
        {
            _xuangubaoService.GetLimitUpBroken("limitUpBroken");
            return Task.CompletedTask;
        }
    }
    class TimeJob : IHostedService, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<FinanceUpJob>()
     .WithIdentity("runJoib", "group0")
     .Build();
            IJobDetail job2 = JobBuilder.Create<FinanceBrokenJob>()
     .WithIdentity("runJoib2", "group1")
     .Build();


          
            ITrigger trigger = TriggerBuilder.Create()
    .WithIdentity("financeTrigger", "group0")
  // .WithCronSchedule("5 5 15 * * ? ")
  .WithCronSchedule("20 59 17 * * ? ")
    .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
  .WithIdentity("financeTrigger", "group1")
  //.WithCronSchedule("6 5 15 * * ? ")
  .WithCronSchedule("30 59 17 * * ? ")
  .Build();

            await scheduler.ScheduleJob(job, trigger);
            await scheduler.ScheduleJob(job2, trigger2);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
