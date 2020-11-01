using GK2823.BizLib.Finance.Services;
using GK2823.BizLib.Shared;
using GK2823.ModelLib.Finance.API;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.API.Controllers
{
    public class Main : Base
    {
        private readonly Service_xuangubao _xuangubaoService;
        
        public Main()
        {        
            _xuangubaoService = AutofacContainer.Resolve<Service_xuangubao>();                          
        }


        [HttpPost("api/GetPoolDetailChartData")]
        public IActionResult GetPoolDetailChartData()
        {
            var result = new MsgResult();
            var data0 = new List<APIPoolDetailWithoutST>();
            var data1 = new List<EverydayLBS>();
            var data2 = new List<EverydayBrokenLBS>();
            var data3 = new List<BrokenPercent>();
            var data4 = new List<EverydayUpLBS>();

            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(() => {
                data0 = _xuangubaoService.CacheRefresh_APIPoolDetailWithoutST();
            }));
            tasks.Add(Task.Run(() => {
                data1 = _xuangubaoService.CacheRefresh_EverydayLBS();
            }));
            tasks.Add(Task.Run(() => {
                data2 = _xuangubaoService.CacheRefresh_EverydayBrokenLBS();
            }));
            tasks.Add(Task.Run(() => {
                data3 = _xuangubaoService.CacheRefresh_BrokenPercent();
            }));
            tasks.Add(Task.Run(() => {
                data4 = _xuangubaoService.CacheRefresh_EverydayUpLBS();
            }));
            Task.WaitAll(tasks.ToArray());

            result.data = new { data0 , data1 , data2 , data3 , data4  };
            return Ok(result);
        }
    }
}
