using Dapper;
using Dapper.Contrib.Extensions;
using GK2823.BizLib.Shared;
using GK2823.ModelLib.Finance.API;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GK2823.BizLib.Finance.Services
{
    public class Service_xuangubao : BaseService
    {
        private readonly IHttpClientFactory _clientFactory;
      
        private static MapperService _mapperService;
        private readonly IRedisService _redisService;
        
        public Service_xuangubao()
        {
            _clientFactory = AutofacContainer.Resolve<IHttpClientFactory>();
           
            _mapperService = AutofacContainer.Resolve<MapperService>();
            _redisService = AutofacContainer.Resolve<IRedisService>();
           
        }

        public async Task<MsgResult> GetHistoryFromXuangubaoAsync(string taskName)
        {
            var result = new MsgResult();

            var tTime = Convert.ToDateTime("2020/10/21");
            for (var tT = tTime; tT < DateTime.Now; tT.AddDays(1))
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get,
                   $"https://flash-api.xuangubao.cn/api/pool/detail?pool_name=limit_up&date={tT.Date.ToString("yyyy-MM-dd")}");


                    var client = _clientFactory.CreateClient();

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<MsgResult>(responseString);
                        if (res.code == 20000)
                        {
                            var data = JsonConvert.DeserializeObject<List<PoolDetail>>(JsonConvert.SerializeObject(res.data));
                            var nowTime = TimeHelper.ConvertToTimeStamp(tT.Date);
                            var tomTime = TimeHelper.ConvertToTimeStamp(tT.AddDays(1).Date);
                            //nowTime = 1601395200;
                            //tomTime = 1601481600;
                            data = data.Where(p => p.last_limit_up >= nowTime && p.last_limit_up < tomTime).ToList();
                            if (data.Count == 0)
                            {
                                throw new Exception("无更新数据");
                            }

                            var dynamicParams = new DynamicParameters();

                            dynamicParams.Add("todayTime", nowTime);
                            dynamicParams.Add("tomorrowTime", tomTime);
                            var totayDBData = _dBService.FinanceDB.Query<LimitUp>("select * from pool_detail where last_limit_up>=@todayTime and last_limit_up<@tomorrowTime", dynamicParams).ToList();

                            var listData = new List<int>();
                            foreach (var item in data)
                            {
                                var hasItem = totayDBData.Where(p => p.symbol == item.symbol).FirstOrDefault();
                                var k = _mapperService.MapCheck<LimitUp>(item);
                                if (hasItem != null)
                                {
                                    k.id = hasItem.id;
                                    var b = _dBService.FinanceDB.Update<LimitUp>(k);

                                    listData.Add(k.id);
                                }
                                else
                                {

                                    var a = _dBService.FinanceDB.Insert<LimitUp>(k);
                                    listData.Add(k.id);
                                }
                            }
                            result.code = 200;
                            result.data = listData;
                            this.SetTaskLog(taskName, listData);
                            _dBService.FinanceDB.Close();
                            Console.WriteLine(tT);
                        }
                        else
                        {
                            result.message = res.message;
                            result.code = 500;
                        }
                    }
                    else
                    {
                        result.message = "HTTP失败";
                        result.code = 400;
                    }


                }
                catch (Exception ex)
                {
                    if(ex.Message!= "Value cannot be null. (Parameter 'source')")
                    {
                        var a = "";
                    }
                    continue;
                }
                finally
                {
                    tT = tT.AddDays(1);
                }
            }


            return result;
        }

        public async Task<MsgResult> GetAllLimitUpBroken(string taskName)
        {
            var result = new MsgResult();

            var tTime = Convert.ToDateTime("2020/10/21");
            for (var tT = tTime; tT < DateTime.Now; tT.AddDays(1))
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get,
                   $"https://flash-api.xuangubao.cn/api/pool/detail?pool_name=limit_up_broken&date={tT.Date.ToString("yyyy-MM-dd")}");


                    var client = _clientFactory.CreateClient();

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<MsgResult>(responseString);
                        if (res.code == 20000)
                        {
                            var data = JsonConvert.DeserializeObject<List<PoolDetail>>(JsonConvert.SerializeObject(res.data));
                            if(data==null)
                            {
                                var aa = "";
                            }
                            var nowTime = TimeHelper.ConvertToTimeStamp(tT.Date);
                            var tomTime = TimeHelper.ConvertToTimeStamp(tT.AddDays(1).Date);
                            //nowTime = 1601395200;
                            //tomTime = 1601481600;
                            data = data.Where(p => p.last_break_limit_up >= nowTime && p.last_break_limit_up < tomTime).ToList();
                            if (data.Count == 0)
                            {
                                throw new Exception("无更新数据");
                            }

                            var dynamicParams = new DynamicParameters();

                            dynamicParams.Add("todayTime", nowTime);
                            dynamicParams.Add("tomorrowTime", tomTime);
                            var totayDBData = _dBService.FinanceDB.Query<limitUpBroken>("select * from limit_up_broken where last_break_limit_up>=@todayTime and last_break_limit_up<@tomorrowTime", dynamicParams).ToList();

                            var listData = new List<int>();
                            var addList = new List<limitUpBroken>();
                            var upList = new List<limitUpBroken>();
                            foreach (var item in data)
                            {
                                var hasItem = totayDBData.Where(p => p.symbol == item.symbol).FirstOrDefault();
                                var k = _mapperService.MapCheck<limitUpBroken>(item);
                                if (hasItem != null)
                                {
                                    k.id = hasItem.id;
                                    var b = _dBService.FinanceDB.Update<limitUpBroken>(k);
                                    upList.Add(k);
                                    listData.Add(k.id);
                                }
                                else
                                {

                                    var a = _dBService.FinanceDB.Insert<limitUpBroken>(k);
                                    addList.Add(k);
                                    listData.Add(k.id);
                                }
                            }
                            //using (var trans = _dBService.FinanceDB.BeginTransaction())
                            //{
                            //    try
                            //    {
                            //        if (addList.Count() > 0) _dBService.FinanceDB.Insert(addList);
                            //        if (upList.Count() > 0) _dBService.FinanceDB.Update(upList);
                            //        trans.Commit();
                            //    }
                            //    catch(Exception ex)
                            //    {
                            //        trans.Rollback();
                            //    }
                            //}
                            result.code = 200;
                            result.data = listData;
                            this.SetTaskLog(taskName, listData);
                            
                            Console.WriteLine(tT.Date.ToString());
                        }
                        else
                        {
                            result.message = res.message;
                            result.code = 500;
                        }
                    }
                    else
                    {
                        result.message = "HTTP失败";
                        result.code = 400;
                    }


                }
                catch (Exception ex)
                {
                    if(ex.Message!= "Value cannot be null. (Parameter 'source')")
                    {
                        var kk = "";
                    }
                    continue;
                }
                finally
                {
                    tT = tT.AddDays(1);
                }
            }


            return result;
        }


        //used
        public async Task<MsgResult> GetFromXuangubaoAsync(string taskName)
        {
            var result = new MsgResult();
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
               "https://flash-api.xuangubao.cn/api/pool/detail?pool_name=limit_up");
                //request.Headers.Add("Accept", "application/vnd.github.v3+json");
                //request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

                var client = _clientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<MsgResult>(responseString);
                    if (res.code == 20000)
                    {
                        var data = JsonConvert.DeserializeObject<List<PoolDetail>>(JsonConvert.SerializeObject(res.data));
                        var nowTime = TimeHelper.ConvertToTimeStamp(DateTime.Now.Date);
                        var tomTime = TimeHelper.ConvertToTimeStamp(DateTime.Now.AddDays(1).Date);
                        //nowTime = 1601395200;
                        //tomTime = 1601481600;
                        data = data.Where(p => p.last_limit_up >= nowTime && p.last_limit_up < tomTime).ToList();
                        if (data.Count == 0)
                        {
                            throw new Exception("无更新数据");
                        }

                        var dynamicParams = new DynamicParameters();

                        dynamicParams.Add("todayTime", nowTime);
                        dynamicParams.Add("tomorrowTime", tomTime);
                        var totayDBData = _dBService.FinanceDB.Query<LimitUp>("select * from pool_detail where last_limit_up>=@todayTime and last_limit_up<@tomorrowTime", dynamicParams).ToList();

                        var listData = new List<int>();
                        foreach (var item in data)
                        {
                            var hasItem = totayDBData.Where(p => p.symbol == item.symbol).FirstOrDefault();
                            var k = _mapperService.MapCheck<LimitUp>(item);
                            if (hasItem != null)
                            {
                                k.id = hasItem.id;
                                var b = _dBService.FinanceDB.Update<LimitUp>(k);

                                listData.Add(k.id);
                            }
                            else
                            {

                                var a = _dBService.FinanceDB.Insert<LimitUp>(k);
                                listData.Add(k.id);
                            }
                        }
                        _redisService.RemoveCache(Constants.Redis.FAPI_BrokenPercent);
                        _redisService.RemoveCache(Constants.Redis.FAPI_GetEverydayBrokenLBS);
                        _redisService.RemoveCache(Constants.Redis.FAPI_GetEverydayLBS);
                        _redisService.RemoveCache(Constants.Redis.FAPI_GetEverydayUpLBS);
                        _redisService.RemoveCache(Constants.Redis.FAPI_GetPoolDetail);
                        result.code = 200;
                        result.data = listData;
                        this.SetTaskLog(taskName, listData);
                    }
                    else
                    {
                        result.message = res.message;
                        result.code = 500;
                    }
                }
                else
                {
                    result.message = "HTTP失败";
                    result.code = 400;
                }
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = ex.Message + ex.StackTrace;
                this.SetTaskLog("error_" + taskName, result.message);
            }
            return result;
        }
        //used
        public async Task<MsgResult> GetLimitUpBroken(string taskName)
        {
            var result = new MsgResult();
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
               "https://flash-api.xuangubao.cn/api/pool/detail?pool_name=limit_up_broken");
                //request.Headers.Add("Accept", "application/vnd.github.v3+json");
                //request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

                var client = _clientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<MsgResult>(responseString);
                    if (res.code == 20000)
                    {
                        var data = JsonConvert.DeserializeObject<List<PoolDetail>>(JsonConvert.SerializeObject(res.data));
                        var nowTime = TimeHelper.ConvertToTimeStamp(DateTime.Now.Date);
                        var tomTime = TimeHelper.ConvertToTimeStamp(DateTime.Now.AddDays(1).Date);
                       
                        data = data.Where(p => p.last_break_limit_up >= nowTime && p.last_break_limit_up < tomTime).ToList();
                        if (data.Count == 0)
                        {
                            throw new Exception("无更新数据");
                        }

                        var dynamicParams = new DynamicParameters();

                        dynamicParams.Add("todayTime", nowTime);
                        dynamicParams.Add("tomorrowTime", tomTime);
                        var totayDBData = _dBService.FinanceDB.Query<limitUpBroken>("select * from limit_up_broken where last_break_limit_up>=@todayTime and last_break_limit_up<@tomorrowTime", dynamicParams).ToList();

                        var listData = new List<int>();
                        foreach (var item in data)
                        {
                            var hasItem = totayDBData.Where(p => p.symbol == item.symbol).FirstOrDefault();
                            var k = _mapperService.MapCheck<limitUpBroken>(item);
                            if (hasItem != null)
                            {
                                k.id = hasItem.id;
                                var b = _dBService.FinanceDB.Update<limitUpBroken>(k);

                                listData.Add(k.id);
                            }
                            else
                            {

                                var a = _dBService.FinanceDB.Insert<limitUpBroken>(k);
                                listData.Add(k.id);
                            }
                        }
                        _redisService.RemoveCache(Constants.Redis.FAPI_BrokenPercent);
                        _redisService.RemoveCache(Constants.Redis.FAPI_GetEverydayBrokenLBS);
                        _redisService.RemoveCache(Constants.Redis.FAPI_GetEverydayLBS);
                        _redisService.RemoveCache(Constants.Redis.FAPI_GetEverydayUpLBS);
                        _redisService.RemoveCache(Constants.Redis.FAPI_GetPoolDetail);
                        result.code = 200;
                        result.data = listData;
                        this.SetTaskLog(taskName, listData);
                    }
                    else
                    {
                        result.message = res.message;
                        result.code = 500;
                    }
                }
                else
                {
                    result.message = "HTTP失败";
                    result.code = 400;
                }
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = ex.Message + ex.StackTrace;
                this.SetTaskLog("error_" + taskName, result.message);
            }
            return result;
        }

        public List<PoolDetail> GetTodayPoolDetail(bool withoutST = false)
        {
            var list = new List<PoolDetail>();
            try
            {
                var nowTime = TimeHelper.ConvertToTimeStamp(DateTime.Now.Date);
                var tomTime = TimeHelper.ConvertToTimeStamp(DateTime.Now.AddDays(1).Date);
                var dynamicParams = new DynamicParameters();
#if DEBUG
                nowTime = 1601395200;
                tomTime = 1601481600;
#endif
                dynamicParams.Add("todayTime", nowTime);
                dynamicParams.Add("tomorrowTime", tomTime);
                var coloum = "*";
                string sql = $"select {coloum} from pool_detail where last_limit_up>=@todayTime and last_limit_up<@tomorrowTime ";
                if (withoutST)
                {
                    sql += " and stock_type!=@stockType";
                    dynamicParams.Add("stockType", 1);
                }
                var totayDBData = _dBService.FinanceDB.Query<LimitUp>(sql, dynamicParams).ToList();
                list = _mapperService.MapCheck<List<PoolDetail>>(totayDBData);
            }
            catch (Exception ex)
            {
                this.SetTaskLog("error_GetTodayPoolDetail", ex.Message + ex.StackTrace);
            }
            return list;
        }

        public List<PoolDetail> GetAllTopPoolDetail(bool withoutST = false)
        {
            var list = new List<PoolDetail>();

            try
            {
                var coloum = "last_limit_up,limit_up_days,stock_chi_name,symbol,stock_type";
                string sql = $"select {coloum} from pool_detail where 1=1 ";
                var dynamicParams = new DynamicParameters();
                if (withoutST)
                {
                    sql += " and position('ST' in stock_chi_name)=0;";
                    // dynamicParams.Add("stock_type", 1);
                }

                var totayDBData = _dBService.FinanceDB.Query<LimitUp>(sql).ToList();

                var resPoolDetail = new List<LimitUp>();
                foreach (var item in totayDBData)
                {

                    var tp = TimeHelper.ConvertToDateTime((long)item.last_limit_up).Date;
                    var dt = TimeHelper.ConvertToTimeStamp(tp);
                    item.last_limit_up = dt;
                }

                var a = totayDBData.GroupBy(p => p.last_limit_up);
                foreach (var item in a)
                {
                    var max = item.ToList().OrderByDescending(p => p.limit_up_days).FirstOrDefault();
                    resPoolDetail.Add(max);
                }





                list = _mapperService.MapCheck<List<PoolDetail>>(resPoolDetail);
            }
            catch (Exception ex)
            {
                this.SetTaskLog("error_GetTodayPoolDetail", ex.Message + ex.StackTrace);
            }
            return list;
        }
        //used
        public List<PoolDetail> GetAllTopPoolDetailWithoutST()
        {
            var list = new List<PoolDetail>();
            try
            {
                var coloum = "last_limit_up,limit_up_days,stock_chi_name";
                string sql = $"select {coloum} from view_pool_detail ";
                var totayDBData = _dBService.FinancePPDB.Query<LimitUp>(sql).ToList();
                list = _mapperService.MapCheck<List<PoolDetail>>(totayDBData);
            }
            catch (Exception ex)
            {
                this.SetTaskLog("error_GetTodayPoolDetailWithoutST", ex.Message + ex.StackTrace);
            }
            return list;
        }
        //used
        public List<EverydayLBS> GetEverydayLBSList()
        {
            var list = _dBService.FinancePPDB.Query<EverydayLBS>("select * from view_everyday_lbs").ToList();
            //var list = _dBService.FinanceDB.GetAll<EverydayLBS>().ToList();
           // _dBService.FinanceDB.Close();
            return list;
        }
        //used
        public List<EverydayBrokenLBS> GetEverydayBrokenLBSList()
        {
            var list = _dBService.FinancePPDB.Query<EverydayBrokenLBS>("select * from view_everyday_broken_lbs").ToList();
            //var list = _dBService.FinanceDB.GetAll<EverydayBrokenLBS>().ToList();
            //_dBService.FinanceDB.Close();
            return list;
        }
        //used
        public List<BrokenPercent> GetEverydayBrokenPercent()
        {
            var list = _dBService.FinancePPDB.Query<BrokenPercent>("select * from view_broken_percent").ToList();
           // var list = _dBService.FinanceDB.GetAll<BrokenPercent>().ToList();
           // _dBService.FinanceDB.Close();
            return list;
        }
        //used
        public List<EverydayUpLBS> GetEverydayUpLBSList()
        {
            var list = _dBService.FinancePPDB.Query<EverydayUpLBS>("select * from view_everyday_up_lbs").ToList();
            //var list = _dBService.FinanceDB.GetAll<EverydayLBS>().ToList();
            // _dBService.FinanceDB.Close();
            return list;
        }

        public void SendFinanceEamils()
        {
            var t0 = this.CacheRefresh_BrokenPercent();
            var t1 = this.CacheRefresh_EverydayBrokenLBS(); 
            var t2 = this.CacheRefresh_EverydayLBS(); 
            var t3 = this.CacheRefresh_EverydayUpLBS();
            var t4 = this.CacheRefresh_APIPoolDetailWithoutST();

            var today = DateTime.Now.Date;
            if (Convert.ToDateTime(TimeHelper.ConvertToyMd(t1.LastOrDefault().stime))!= today || Convert.ToDateTime(TimeHelper.ConvertToyMd(t3.LastOrDefault().stime))!= today)
            {
                return;
            }
           
            StringBuilder sb = new StringBuilder(8);
            sb.Append("<style>span{font-weight:700}</style>");
            sb.Append($"<p><span>炸板率</span>：{Convert.ToInt32(t0.LastOrDefault().percent)}%</p>");
            sb.Append($"<p><span>炸板数</span>：{t1.LastOrDefault().lb_count}：{t1.LastOrDefault().stock_chi_name}</p>");
            sb.Append($"<p><span>二连板</span>：{t2.LastOrDefault().lb_count}：{t2.LastOrDefault().stock_chi_name}</p>");
            sb.Append($"<p><span>涨停数</span>：{t3.LastOrDefault().lb_count}：{t3.LastOrDefault().stock_chi_name}</p>");
            sb.Append($"<p><span>最高板</span>：{Convert.ToInt32(t4.LastOrDefault().limit_up_days)}：{t4.LastOrDefault().stock_chi_name}</p>");
            sb.Append("<a href='http://fweb.gk2823.com/'>更多...</a>");
            this.SendEmail(_appSettings.Value.MainEmail.UserAddress, "每日金融-" + DateTime.Now.Date.ToString("yyyy年MM月dd日"), sb.ToString());
#if DEBUG
            this.SendEmail(_appSettings.Value.BroMainEmail.UserAddress, "每日金融-" + DateTime.Now.Date.ToString("yyyy年MM月dd日"), sb.ToString());
#endif          
        }

        public List<APIPoolDetailWithoutST> CacheRefresh_APIPoolDetailWithoutST()
        {
            var data0 = new List<APIPoolDetailWithoutST>();
            var redisKey = Constants.Redis.FAPI_GetPoolDetail;
            var redislist = _redisService.GetCache(redisKey) as List<APIPoolDetailWithoutST>;
            if (redislist == null)
            {
                var list = this.GetAllTopPoolDetailWithoutST();
                data0 = _mapperService.MapCheck<List<APIPoolDetailWithoutST>>(list);
                if (data0.Count() > 0)
                {
                    _redisService.SetCache(redisKey, data0, TimeSpan.FromHours(24));
                }
            }
            else
            {
                data0 = redislist;
            }
            return data0;
        }

        public List<EverydayLBS> CacheRefresh_EverydayLBS()
        {
            var data1 = new List<EverydayLBS>();
            var redisKey = Constants.Redis.FAPI_GetEverydayLBS;
            var redislist = _redisService.GetCache(redisKey) as List<EverydayLBS>;
            if (redislist == null)
            {
                data1 = this.GetEverydayLBSList();
                if (data1.Count() > 0)
                {
                    _redisService.SetCache(redisKey, data1, TimeSpan.FromHours(24));
                }
            }
            else
            {
                data1 = redislist;
            }
            return data1;
        }

        public List<EverydayBrokenLBS> CacheRefresh_EverydayBrokenLBS()
        {
            var data2 = new List<EverydayBrokenLBS>();
            var redisKey = Constants.Redis.FAPI_GetEverydayBrokenLBS;
            var redislist = _redisService.GetCache(redisKey) as List<EverydayBrokenLBS>;
            if (redislist == null)
            {
                data2 = this.GetEverydayBrokenLBSList();
                if (data2.Count() > 0)
                {
                    _redisService.SetCache(redisKey, data2, TimeSpan.FromHours(24));
                }
            }
            else
            {
                data2 = redislist;
            }
            return data2;
        }

        public List<BrokenPercent> CacheRefresh_BrokenPercent()
        {
            var data3 = new List<BrokenPercent>();
            var redisKey = Constants.Redis.FAPI_BrokenPercent;
            var redislist = _redisService.GetCache(redisKey) as List<BrokenPercent>;
            if (redislist == null)
            {
                data3 = this.GetEverydayBrokenPercent();
                if (data3.Count() > 0)
                {
                    _redisService.SetCache(redisKey, data3, TimeSpan.FromHours(24));
                }
            }
            else
            {
                data3 = redislist;
            }
            return data3;
        }

        public List<EverydayUpLBS> CacheRefresh_EverydayUpLBS()
        {
            var data4 = new List<EverydayUpLBS>();
            var redisKey = Constants.Redis.FAPI_GetEverydayUpLBS;
            var redislist = _redisService.GetCache(redisKey) as List<EverydayUpLBS>;
            if (redislist == null)
            {
                data4 = this.GetEverydayUpLBSList();
                if (data4.Count() > 0)
                {
                    _redisService.SetCache(redisKey, data4, TimeSpan.FromHours(24));
                }
            }
            else
            {
                data4 = redislist;
            }
            return data4;
        }


    }
}
