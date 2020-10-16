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
    }
}
