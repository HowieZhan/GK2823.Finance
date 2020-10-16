﻿using GK2823.BizLib.Finance.Services;
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
                    _redisService.SetKeyExpire(redisKey, TimeSpan.FromMinutes(5));
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
                    _redisService.SetKeyExpire(redisKey, TimeSpan.FromMinutes(5));
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
