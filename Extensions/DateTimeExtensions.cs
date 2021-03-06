using System;

namespace WebTestProteus.Extensions
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Converts a given DateTime into a Unix timestamp
        /// </summary>
        /// <param name="value">Any DateTime</param>
        /// <returns>The given DateTime in Unix timestamp format</returns>
        public static long ToUnixTimestamp(this DateTime value)
        {
            return (long)(value.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }

        /// <summary>
        /// Returns a local DateTime based on provided unix timestamp
        /// </summary>
        /// <param name="timestamp">Unix/posix timestamp</param>
        /// <returns>Local datetime</returns>
        public static DateTime ParseUnixTimestamp(long timestamp)
        {
            return (new DateTime(1970, 1, 1)).AddMilliseconds(timestamp).ToLocalTime();
        }

    }
}
