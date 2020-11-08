using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GK2823.UtilLib.Helpers
{
    public class TimeHelper
    {
        /// <summary>
        /// 秒
        /// </summary>
        /// <param name="timeStanm"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timeStanm)
        {
            var dongba = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);//东八区
            var lastTime = dongba.AddHours(8).AddSeconds(timeStanm);
            return lastTime;
        }

        /// <summary>
        /// 秒
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ConvertToTimeStamp(DateTime dateTime)
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.AddHours(-8) - Jan1st1970).TotalSeconds;
        }

        public static string ConvertToyMd(string strDate)
        {          
            strDate = DateTime.ParseExact(strDate, "yyyyMMdd", CultureInfo.CurrentCulture).ToString("yyyy-MM-dd");
            return strDate;
        }

        public static string FormatTime(long ms)
        {
            int ss = 1000;
            int mi = ss * 60;
            int hh = mi * 60;
            int dd = hh * 24;

            long day = ms / dd;
            long hour = (ms - day * dd) / hh;
            long minute = (ms - day * dd - hour * hh) / mi;
            long second = (ms - day * dd - hour * hh - minute * mi) / ss;
            long milliSecond = ms - day * dd - hour * hh - minute * mi - second * ss;

            string sDay = day < 10 ? "0" + day : "" + day; //天
            string sHour = hour < 10 ? "0" + hour : "" + hour;//小时
            string sMinute = minute < 10 ? "0" + minute : "" + minute;//分钟
            string sSecond = second < 10 ? "0" + second : "" + second;//秒
            string sMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//毫秒
            sMilliSecond = milliSecond < 100 ? "0" + sMilliSecond : "" + sMilliSecond;

            return string.Format("{0} 天 {1} 小时 {2} 分 {3} 秒", sDay, sHour, sMinute, sSecond);
        }
    }
}
