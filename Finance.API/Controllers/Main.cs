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

namespace Finance.API.Controllers
{
    public class Main : Base
    {
        private readonly Service_xuangubao _xuangubaoService;
        private readonly MapperService _mapperService;
        public Main()
        {
            _xuangubaoService = AutofacContainer.Resolve<Service_xuangubao>();
            _mapperService = AutofacContainer.Resolve<MapperService>();
        }
        [HttpPost("api/GetPoolDetail")]
        public IActionResult GetPoolDetail()
        {
            var result = new MsgResult();
            var list = _xuangubaoService.GetAllTopPoolDetail(true);
            var _list = _mapperService.MapCheck<List<APIPoolDetail>>(list);
            result.data = _list.OrderBy(p=>p.thatDate);
            return Ok(result);
        }

        //[HttpPost("api/GetHistoryFromXuangubaoAsync")]
        //public IActionResult GetHistoryFromXuangubao()
        //{
        //    var result = new MsgResult();
        //    var list = _xuangubaoService.GetHistoryFromXuangubaoAsync("sd");
        //    //var _list = _mapperService.MapCheck<List<APIPoolDetail>>(list);
        //    //result.data = _list;
        //    return Ok(result);
        //}
    }
}
