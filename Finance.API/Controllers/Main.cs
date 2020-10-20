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
        private readonly MapperService _mapperService;
        private readonly IRedisService _redisService;
        public Main()
        {
            _xuangubaoService = AutofacContainer.Resolve<Service_xuangubao>();
            _mapperService = AutofacContainer.Resolve<MapperService>();
            _redisService = AutofacContainer.Resolve<IRedisService>();
        }
        [HttpPost("api/GetPoolDetail")]
        public IActionResult GetPoolDetail()
        {
            var result = new MsgResult();
            var _list = new List<APIPoolDetailWithoutST>();
            var redisKey = Constants.Redis.FAPI_GetPoolDetail;
            var redislist = _redisService.GetCache(redisKey) as List<APIPoolDetailWithoutST>;
            if (redislist == null)
            {
                var list = _xuangubaoService.GetAllTopPoolDetailWithoutST();
                _list = _mapperService.MapCheck<List<APIPoolDetailWithoutST>>(list);
                if (_list.Count() > 0)
                {
                    _redisService.SetCache(redisKey, _list);
                    _redisService.SetKeyExpire(redisKey, TimeSpan.FromHours(24));
                }
            }
            else
            {
                _list = redislist;
            }
            result.data = _list;
            return Ok(result);
        }

        [HttpPost("api/GetEverydayLBS")]
        public IActionResult GetEverydayLBS()
        {
            var result = new MsgResult();
            var _list = new List<EverydayLBS>();
            var redisKey = Constants.Redis.FAPI_GetEverydayLBS;
            var redislist = _redisService.GetCache(redisKey) as List<EverydayLBS>;
            if (redislist == null)
            {
                _list = _xuangubaoService.GetEverydayLBSList();
                if (_list.Count() > 0)
                {
                    _redisService.SetCache(redisKey, _list);
                    _redisService.SetKeyExpire(redisKey, TimeSpan.FromHours(24));
                }
            }
            else
            {
                _list = redislist;
            }
            result.data = _list;
            return Ok(result);
        }

        [HttpPost("api/GetEverydayBrokenLBS")]
        public IActionResult GetEverydayBrokenLBS()
        {
            var result = new MsgResult();
            var _list = new List<EverydayBrokenLBS>();
            var redisKey = Constants.Redis.FAPI_GetEverydayBrokenLBS;
            var redislist = _redisService.GetCache(redisKey) as List<EverydayBrokenLBS>;
            if (redislist == null)
            {
                _list = _xuangubaoService.GetEverydayBrokenLBSList();
                if (_list.Count() > 0)
                {
                    _redisService.SetCache(redisKey, _list);
                    _redisService.SetKeyExpire(redisKey, TimeSpan.FromHours(24));
                }
            }
            else
            {
                _list = redislist;
            }
            result.data = _list;
            return Ok(result);
        }

        [HttpPost("api/GetEverydayBrokenPercent")]
        public IActionResult GetEverydayBrokenPercent()
        {
            var result = new MsgResult();
            var _list = new List<BrokenPercent>();
            var redisKey = Constants.Redis.FAPI_BrokenPercent;
            var redislist = _redisService.GetCache(redisKey) as List<BrokenPercent>;
            if (redislist == null)
            {
                _list = _xuangubaoService.GetEverydayBrokenPercent();
                if (_list.Count() > 0)
                {
                    _redisService.SetCache(redisKey, _list);
                    _redisService.SetKeyExpire(redisKey, TimeSpan.FromHours(24));
                }
            }
            else
            {
                _list = redislist;
            }
            result.data = _list;
            return Ok(result);
        }

        [HttpPost("api/GetEverydayUpLBS")]
        public IActionResult GetEverydayUpLBS()
        {
            var result = new MsgResult();
            var _list = new List<EverydayUpLBS>();
            var redisKey = Constants.Redis.FAPI_GetEverydayUpLBS;
            var redislist = _redisService.GetCache(redisKey) as List<EverydayUpLBS>;
            if (redislist == null)
            {
                _list = _xuangubaoService.GetEverydayUpLBSList();
                if (_list.Count() > 0)
                {
                    _redisService.SetCache(redisKey, _list);
                    _redisService.SetKeyExpire(redisKey, TimeSpan.FromHours(24));
                }
            }
            else
            {
                _list = redislist;
            }
            result.data = _list;
            return Ok(result);
        }

    }
}
