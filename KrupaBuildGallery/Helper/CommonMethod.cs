using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public static class CommonMethod
    {
        public static string ConvertFromUTC(DateTime? utcDateTime)
        {
            if (utcDateTime == null)
                return "";

            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcDateTime), nzTimeZone);
            string dt = dateTimeAsTimeZone.ToString("dd/MM/yyyy hh:mm tt");
            return dt;
        }

        public static DateTime ConvertToUTC(DateTime dateTime, string timeZone)
        {
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, nzTimeZone);
            return utcDateTime;
        }
    }
}