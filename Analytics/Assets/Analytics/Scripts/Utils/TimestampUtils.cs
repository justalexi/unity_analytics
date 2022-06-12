using System;

namespace Analytics.Scripts.Utils
{
    public static class TimestampUtils
    {
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
    }
}