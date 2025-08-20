using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfDay(this DateTime date)
        {
            return date.Date;
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return date.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startDay = DayOfWeek.Monday)
        {
            var diff = (7 + (date.DayOfWeek - startDay)) % 7;
            return date.AddDays(-diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime date, DayOfWeek startDay = DayOfWeek.Monday)
        {
            return date.StartOfWeek(startDay).AddDays(7).AddTicks(-1);
        }

        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return date.StartOfMonth().AddMonths(1).AddTicks(-1);
        }

        public static DateTime StartOfQuarter(this DateTime date)
        {
            var quarterMonth = (date.Month - 1) / 3 * 3 + 1;
            return new DateTime(date.Year, quarterMonth, 1);
        }

        public static DateTime EndOfQuarter(this DateTime date)
        {
            return date.StartOfQuarter().AddMonths(3).AddTicks(-1);
        }

        public static DateTime StartOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        public static DateTime EndOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 12, 31, 23, 59, 59, 999);
        }

        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static bool IsBusinessDay(this DateTime date)
        {
            return !date.IsWeekend();
        }

        public static DateTime NextBusinessDay(this DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            } while (!date.IsBusinessDay());

            return date;
        }

        public static DateTime PreviousBusinessDay(this DateTime date)
        {
            do
            {
                date = date.AddDays(-1);
            } while (!date.IsBusinessDay());

            return date;
        }

        public static int Age(this DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
                age--;

            return age;
        }

        public static string ToRelativeString(this DateTime date)
        {
            var timeSpan = DateTime.Now - date;

            return timeSpan switch
            {
                _ when timeSpan.TotalDays > 365 => $"{(int)(timeSpan.TotalDays / 365)} year(s) ago",
                _ when timeSpan.TotalDays > 30 => $"{(int)(timeSpan.TotalDays / 30)} month(s) ago",
                _ when timeSpan.TotalDays > 1 => $"{(int)timeSpan.TotalDays} day(s) ago",
                _ when timeSpan.TotalHours > 1 => $"{(int)timeSpan.TotalHours} hour(s) ago",
                _ when timeSpan.TotalMinutes > 1 => $"{(int)timeSpan.TotalMinutes} minute(s) ago",
                _ => "just now"
            };
        }

        public static bool IsBetween(this DateTime date, DateTime startDate, DateTime endDate)
        {
            return date >= startDate && date <= endDate;
        }

        public static DateTime ToTimeZone(this DateTime date, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTime(date, timeZone);
        }

        public static string ToIso8601String(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public static long ToUnixTimestamp(this DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }

        public static DateTime FromUnixTimestamp(this long timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }

        public static int Quarter(this DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }

        public static int WeekOfYear(this DateTime date)
        {
            var cal = CultureInfo.CurrentCulture.Calendar;
            return cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static DateTime AddBusinessDays(this DateTime date, int businessDays)
        {
            var sign = Math.Sign(businessDays);
            var unsignedDays = Math.Abs(businessDays);

            for (int i = 0; i < unsignedDays; i++)
            {
                do
                {
                    date = date.AddDays(sign);
                } while (!date.IsBusinessDay());
            }

            return date;
        }

        public static bool IsToday(this DateTime date)
        {
            return date.Date == DateTime.Today;
        }

        public static bool IsYesterday(this DateTime date)
        {
            return date.Date == DateTime.Today.AddDays(-1);
        }

        public static bool IsTomorrow(this DateTime date)
        {
            return date.Date == DateTime.Today.AddDays(1);
        }
    }
}
