using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime Today { get; }
        DateTime UtcToday { get; }
        DateTimeOffset NowOffset { get; }
        DateTimeOffset UtcNowOffset { get; }

        DateTime ConvertToTimeZone(DateTime dateTime, string timeZoneId);
        DateTime ConvertFromTimeZone(DateTime dateTime, string timeZoneId);
        TimeZoneInfo GetTimeZone(string timeZoneId);
        string[] GetAvailableTimeZones();

        DateTime ParseDate(string dateString, string format = null);
        string FormatDate(DateTime dateTime, string format = "yyyy-MM-dd");
        string FormatDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss");

        bool IsBusinessDay(DateTime date);
        DateTime GetNextBusinessDay(DateTime date);
        DateTime GetPreviousBusinessDay(DateTime date);

        DateTime StartOfDay(DateTime date);
        DateTime EndOfDay(DateTime date);
        DateTime StartOfWeek(DateTime date);
        DateTime EndOfWeek(DateTime date);
        DateTime StartOfMonth(DateTime date);
        DateTime EndOfMonth(DateTime date);
        DateTime StartOfYear(DateTime date);
        DateTime EndOfYear(DateTime date);
    }
}
