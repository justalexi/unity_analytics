using System;
using System.IO;

namespace Analytics.Scripts.Utils
{
    public static class TimestampUtils
    {
        /// <summary>
        /// Human-readable, sortable way of timestamping (events, files, etc.)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimestamp(DateTime dateTime)
        {
            long result = 0;

            result += dateTime.Millisecond;
            result += dateTime.Second * 1000;
            result += dateTime.Minute * 100_000;
            result += dateTime.Hour * 100_00_000;
            result += (long)dateTime.Day * 100_00_00_000;
            result += dateTime.Month * 100_00_00_00_000;
            result += dateTime.Year * 100_00_00_00_00_000;

            return result;
        }

        public static long GetTimestampFromFileName(string filePath, string extension)
        {
            var timestampStr = filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            timestampStr = timestampStr.Replace(extension, "");
            var timestampAndCounter = timestampStr.Split('_');
            var timestamp = long.Parse(timestampAndCounter[0]);
            return timestamp;
        }
    }
}