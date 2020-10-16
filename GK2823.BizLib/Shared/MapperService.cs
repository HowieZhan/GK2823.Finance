using AutoMapper;
using GK2823.ModelLib.Finance.API;
using GK2823.UtilLib.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public class MapperService
    {
        public MapperConfiguration _configuration;
        public MapperService()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PoolDetail, _PoolDetail>()
                .ForMember(tag => tag.limit_timeline, sour => sour.MapFrom(p =>JsonConvert.SerializeObject(p.limit_timeline)))
                .ForMember(tag => tag.surge_reason, sour => sour.MapFrom(p => JsonConvert.SerializeObject(p.surge_reason)))
                .ForMember(tag => tag.id, sour => sour.Ignore());

                cfg.CreateMap<_PoolDetail, PoolDetail>()
              .ForMember(tag => tag.limit_timeline, sour => sour.MapFrom(p =>JsonConvert.DeserializeObject<PoolDetail.LimitTimeline>(p.limit_timeline)))
              .ForMember(tag => tag.surge_reason, sour => sour.MapFrom(p =>JsonConvert.DeserializeObject<PoolDetail.SurgeReason>(p.surge_reason)))
               .ForMember(tag => tag.id, sour => sour.Ignore());

                cfg.CreateMap<PoolDetail, APIPoolDetail>().
                ForMember(tag => tag.thatDate, sour => sour.MapFrom(p => 
                      TimeHelper.ConvertToDateTime((long)p.last_limit_up).Date.ToString("yyyyMMdd")
                   
                )).ForMember(tag => tag.strWeek, sour => sour.MapFrom(p =>    
                this.chineseStrWeek((long)p.last_limit_up)
                ));


                cfg.CreateMap<PoolDetail, APIPoolDetailWithoutST>().
               ForMember(tag => tag.thatDate, sour => sour.MapFrom(p =>
                     p.last_limit_up

               )).ForMember(tag => tag.strWeek, sour => sour.MapFrom(p =>
               this.chineseStrWeek((p.last_limit_up.ToString())
               ))).ForMember(tag => tag.symbol, sour => sour.MapFrom(p =>
                string.Empty
               ));


                //var tp = TimeHelper.ConvertToDateTime((long)item.last_limit_up).Date;
                //var dt = TimeHelper.ConvertToTimeStamp(tp);
                //item.last_limit_up = dt;

                //.ForMember(tag => tag.CHANNELNAME, sour => sour.MapFrom(p => p.details.dId));
            });
            configuration.AssertConfigurationIsValid();
            _configuration = configuration;
            // var fooDto = mapper.Map<FooDto>(foo);
        }

        private string chineseStrWeek(long tT)
        {
            var k = TimeHelper.ConvertToDateTime(tT).Date.DayOfWeek.ToString();
            var str = string.Empty;
            switch(k)
            {
                case "Monday":str = "周一"; break;
                case "Tuesday": str = "周二"; break;
                case "Wednesday": str = "周三"; break;
                case "Thursday": str = "周四"; break;
                case "Friday": str = "周五"; break;       
                default: str = "未知"; break;
            }
            return str;
        }

        private string chineseStrWeek(string tT)
        {
            tT =TimeHelper.ConvertToyMd(tT);
            var k = Convert.ToDateTime(tT).Date.DayOfWeek.ToString();
            var str = string.Empty;
            switch (k)
            {
                case "Monday": str = "周一"; break;
                case "Tuesday": str = "周二"; break;
                case "Wednesday": str = "周三"; break;
                case "Thursday": str = "周四"; break;
                case "Friday": str = "周五"; break;
                default: str = "未知"; break;
            }
            return str;
        }


        public T MapCheck<T>(object t)
        {
            var mapper = _configuration.CreateMapper();
            var fooDto = mapper.Map<T>(t);
            return fooDto;
        }
    }
}
