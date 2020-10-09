using Dapper;
using Dapper.Contrib.Extensions;
using GK2823.BizLib.Shared;
using GK2823.ModelLib.Finance.API;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
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
        private readonly DBService _dBService;
        private readonly MapperService _mapperService;
        public Service_xuangubao()
        {
            _clientFactory = AutofacContainer.Resolve<IHttpClientFactory>();
            _dBService = AutofacContainer.Resolve<DBService>();
            _mapperService = AutofacContainer.Resolve<MapperService>();
        }

        public async Task<MsgResult> GetHistoryFromXuangubaoAsync(string taskName)
        {
            var result = new MsgResult();

            var tTime = Convert.ToDateTime("2020/04/13");
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
                            var totayDBData = _dBService.FinanceDB.Query<_PoolDetail>("select * from pool_detail where last_limit_up>=@todayTime and last_limit_up<@tomorrowTime", dynamicParams).ToList();

                            var listData = new List<int>();
                            foreach (var item in data)
                            {
                                var hasItem = totayDBData.Where(p => p.symbol == item.symbol).FirstOrDefault();
                                var k = _mapperService.MapCheck<_PoolDetail>(item);
                                if (hasItem != null)
                                {
                                    k.id = hasItem.id;
                                    var b = _dBService.FinanceDB.Update<_PoolDetail>(k);

                                    listData.Add(k.id);
                                }
                                else
                                {

                                    var a = _dBService.FinanceDB.Insert<_PoolDetail>(k);
                                    listData.Add(k.id);
                                }
                            }
                            result.code = 200;
                            result.data = listData;
                            this.SetTaskLog(taskName, listData);
                            _dBService.FinanceDB.Close();
                            Console.WriteLine(res.code);
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
                    continue;
                }
                finally
                {
                    tT = tT.AddDays(1);
                }
            }


            return result;
        }

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
                        var totayDBData = _dBService.FinanceDB.Query<_PoolDetail>("select * from pool_detail where last_limit_up>=@todayTime and last_limit_up<@tomorrowTime", dynamicParams).ToList();

                        var listData = new List<int>();
                        foreach (var item in data)
                        {
                            var hasItem = totayDBData.Where(p => p.symbol == item.symbol).FirstOrDefault();
                            var k = _mapperService.MapCheck<_PoolDetail>(item);
                            if (hasItem != null)
                            {
                                k.id = hasItem.id;
                                var b = _dBService.FinanceDB.Update<_PoolDetail>(k);

                                listData.Add(k.id);
                            }
                            else
                            {

                                var a = _dBService.FinanceDB.Insert<_PoolDetail>(k);
                                listData.Add(k.id);
                            }
                        }
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
                var totayDBData = _dBService.FinanceDB.Query<_PoolDetail>(sql, dynamicParams).ToList();
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

                var totayDBData = _dBService.FinanceDB.Query<_PoolDetail>(sql).ToList();

                var resPoolDetail = new List<_PoolDetail>();
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
    }
}
