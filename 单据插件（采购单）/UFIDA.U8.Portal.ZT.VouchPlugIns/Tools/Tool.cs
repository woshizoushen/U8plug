using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Tools
{
    public class Tool
    {

        /// <summary>
        /// 时间戳返回日期
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        /// <summary>
        /// 时间戳返回日期
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp);
            TimeZoneInfo tz = TimeZoneInfo.Local;
            TimeSpan offset = tz.GetUtcOffset(unixEpoch);
            DateTime converted = unixEpoch.Add(offset);
            return converted;
        }


        /// <summary>
        /// 时间返回毫秒时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalMilliseconds);
        }
    }
}
