using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces;

namespace IskoopDemo.Shared.Domain.Entities
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Today => DateTime.Today;
        public DateTime UtcToday => DateTime.UtcNow.Date;
        public DateTimeOffset NowOffset => DateTimeOffset.Now;
        public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
        public DateTime ConvertToTimeZone(DateTime dateTime, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }

        public DateTime ConvertFromTimeZone(DateTime dateTime, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        }

        public TimeZoneInfo GetTimeZone(string timeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        public string[] GetAvailableTimeZones()
        {
            var timeZones = new List<string>();
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                timeZones.Add(tz.Id);
            }
            return timeZones.ToArray();
        }

        public DateTime ParseDate(string dateString, string format = null)
        {
            if (string.IsNullOrWhiteSpace(format))
                return DateTime.Parse(dateString);

            return DateTime.ParseExact(dateString, format, null);
        }

        public string FormatDate(DateTime dateTime, string format = "yyyy-MM-dd")
        {
            return dateTime.ToString(format);
        }

        public string FormatDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }

        public bool IsBusinessDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        public DateTime GetNextBusinessDay(DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            } while (!IsBusinessDay(date));

            return date;
        }

        public DateTime GetPreviousBusinessDay(DateTime date)
        {
            do
            {
                date = date.AddDays(-1);
            } while (!IsBusinessDay(date));

            return date;
        }

        public DateTime StartOfDay(DateTime date) => date.Date;
        public DateTime EndOfDay(DateTime date) => date.Date.AddDays(1).AddTicks(-1);
        public DateTime StartOfWeek(DateTime date) => date.Date.AddDays(-(int)date.DayOfWeek);
        public DateTime EndOfWeek(DateTime date) => StartOfWeek(date).AddDays(7).AddTicks(-1);
        public DateTime StartOfMonth(DateTime date) => new DateTime(date.Year, date.Month, 1);
        public DateTime EndOfMonth(DateTime date) => StartOfMonth(date).AddMonths(1).AddTicks(-1);
        public DateTime StartOfYear(DateTime date) => new DateTime(date.Year, 1, 1);
        public DateTime EndOfYear(DateTime date) => new DateTime(date.Year, 12, 31, 23, 59, 59, 999);
    }
}
