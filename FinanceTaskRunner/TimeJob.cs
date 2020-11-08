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

    public class SendFinanceEamils : IJob
    {
        private readonly Service_xuangubao _xuangubaoService;
        public SendFinanceEamils()
        {
            _xuangubaoService = AutofacContainer.Resolve<Service_xuangubao>();
        }
        public Task Execute(IJobExecutionContext context)
        {
            _xuangubaoService.SendFinanceEamils();
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
            IJobDetail job3 = JobBuilder.Create<SendFinanceEamils>()
    .WithIdentity("runJoib3", "group2")
    .Build();



            ITrigger trigger = TriggerBuilder.Create()
    .WithIdentity("financeTrigger", "group0")
   .WithCronSchedule("5 55 15 * * ? ")
  //.WithCronSchedule("20 59 17 * * ? ")
    .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
  .WithIdentity("financeTrigger", "group1")
  .WithCronSchedule("20 55 15 * * ? ")
  //.WithCronSchedule("30 59 17 * * ? ")
  .Build();

            ITrigger trigger3 = TriggerBuilder.Create()
.WithIdentity("financeTrigger", "group2")
.WithCronSchedule("50 55 15 * * ? ")
//.WithCronSchedule("30 59 17 * * ? ")
.Build();

            await scheduler.ScheduleJob(job, trigger);
            await scheduler.ScheduleJob(job2, trigger2);
            await scheduler.ScheduleJob(job3, trigger3);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
