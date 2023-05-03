using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Helper
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 获取今天开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetTodayStart()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day);
        }

        /// <summary>
        /// 获取今天结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetTodayEnd()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
        }

        /// <summary>
        /// 获取当前unix时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            return (long)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        /// <summary>
        /// 获取当前unix时间戳
        /// </summary>
        /// <returns></returns>
        public static long ToTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

    }
}
