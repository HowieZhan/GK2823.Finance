using GK2823.BizLib.Finance.Services;
using GK2823.ModelLib.Finance.API;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IRedisService _redisService;
        public ScheduleService()
        {
            _appSettings = AutofacContainer.Resolve<IOptions<AppSettings>>();
            // _clientFactory= AutofacContainer.Resolve<IHttpClientFactory>();
            _xuangubaoService = AutofacContainer.Resolve<Service_xuangubao>();
            _redisService= AutofacContainer.Resolve<IRedisService>();
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
                    //case "get_from_xuangubao":
                    //    _xuangubaoService.GetFromXuangubaoAsync(state.ToString());
                    //    ; break;
                    //case "limitUpBroken":
                    //    _xuangubaoService.GetLimitUpBroken(state.ToString());
                    //    ; break;
                    case "test":
                        Test();
                        break;
                  
                }
            }
            catch (Exception ex)
            {
#if RELEASE
                _xuangubaoService.SendEmail(_appSettings.Value.MainEmail.UserAddress,"爬虫异常",ex.Message+ex.StackTrace);
#endif
            }
        }

        public void Test()
        {
            string fileUrl = Path.Combine("E:\\Main\\gk2823_files\\json", "table0.json");
            JsonFileHelper jsonFileHelper = new JsonFileHelper(fileUrl);
            var data= jsonFileHelper.ReadList<Table0>("RECORDS");
            List<JosnTab0> jsonTabs = new List<JosnTab0>();
            foreach(var a in data.GroupBy(p=>p.REP_OFFICE).ToList())
            {
                JosnTab0 jsonTab = new JosnTab0();
                jsonTab.officeName = a.Key.ToString();
                jsonTab.details = new List<JosnTab0.Details>();
                foreach(var b in a.ToList().GroupBy(p=>p.AGENTLEVEL).ToList())
                {
                    var detail = new JosnTab0.Details();
                    detail.agentName = b.Key.ToString();
                    detail.bindThridAgenNum = b.Sum(p => p.BINDTHIRDCOUNT);
                    detail.cloudGoodNum = b.Sum(p => p.BINDSECONDCOUNT);
                    detail.goodsDetails = new List<JosnTab0.Details.GoodsDetails>();
                    foreach(var c in b.ToList().GroupBy(p=>p.CATALOGNAME).ToList())
                    {
                        var goodsDetail = new JosnTab0.Details.GoodsDetails();
                        goodsDetail.allLightCount = c.FirstOrDefault().ALLLIGHTUPNUM;
                        goodsDetail.goodNum = $"{c.Key.ToString()}："+c.FirstOrDefault().LOCALLIGHTUPNUM.ToString();
                        var present = c.FirstOrDefault().LOCALLIGHTUPNUM/ c.FirstOrDefault().ALLLIGHTUPNUM;
                        goodsDetail.goodPresent = $"{c.Key.ToString()}：" + (present > 0 ? present.ToString("0%") : "0%");
                        goodsDetail.provinceMoney = c.FirstOrDefault().MONEY;
                        goodsDetail.provinceNum = c.FirstOrDefault().SELLNUM;
                        detail.goodsDetails.Add(goodsDetail);
                    }
                    jsonTab.details.Add(detail);
                }
                Console.WriteLine(JsonConvert.SerializeObject(jsonTab));
                Console.WriteLine("-----------------------------------");
                jsonTabs.Add(jsonTab);
            }
        }

        public class JosnTab0
        {
            public string officeName { get; set; }
            public List<Details> details { get; set; }
            public class Details { 
                public string agentName { get; set; }
                public double bindThridAgenNum { get; set; }
                public double cloudGoodNum { get; set; }

                public List<GoodsDetails> goodsDetails { get; set; }
                public class GoodsDetails
                { 
                    public double allLightCount { get; set; }
                    public string goodNum { get; set; }
                    public string goodPresent { get; set; }
                    public double provinceMoney { get; set; }
                    public double provinceNum { get; set; }
                }







            }
        }

        public class Table0
        {
            public string AGENTLEVEL { get; set; }
            public string REP_OFFICE { get; set; }
            public string CATALOGNAME { get; set; }
            public double LOCALLIGHTUPNUM { get; set; }
            public double ALLLIGHTUPNUM { get; set; }
            public double SELLNUM { get; set; }
            public double MONEY { get; set; }
            public double BINDTHIRDCOUNT { get; set; }
            public double BINDSECONDCOUNT { get; set; }
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
