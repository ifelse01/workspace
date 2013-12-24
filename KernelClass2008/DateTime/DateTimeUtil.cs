using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace KernelClass
{
    public class DateTimeUtil
    { 
        //dd-MMM-yy   20-Nov-13
        //MMMM yy     November 13
        //MM yy       11-13
        private static readonly string DatePattern = "yyyy-MM-dd";
        private static readonly string ShrinkedDatePattern = "yyyyMMdd";
        private static readonly string CalendarDatePattern = "MM/dd/yyyy";
        private static readonly string MonthPattern = "MM-dd";
        private static readonly string DateTimePattern = "yyyy-MM-dd hh:mm tt";
        private static readonly string DateTimeUniversalPattern = "yyyy-MM-ddTHH:mm:sszzz";
        private static readonly string defaultCountry = "CN";

        public static string GetDisplayText(DateTime? dateTime, DateTimeUtil.Pattern pattern, DateTimeUtil.ValueType valueType)
        {
            return GetDisplayText(dateTime, pattern, valueType, defaultCountry);
        }

        public static string GetDisplayText(DateTime? dateTime, DateTimeUtil.Pattern pattern, DateTimeUtil.ValueType valueType, string country)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }

            var formatString = GetFormatString(pattern);
            var normalizedDateTime = NormalizeDateTimeValue(dateTime, valueType, country);

            return normalizedDateTime.ToString(formatString, CultureInfo.InvariantCulture);
        }

        private static DateTime NormalizeDateTimeValue(DateTime? dateTime, DateTimeUtil.ValueType valueType, string country)
        {
            switch (valueType)
            {
                case ValueType.Utc:
                    return TimeZoneUtil.ConvertFromUTC(dateTime.Value, country);
                case ValueType.Local:
                    return dateTime.Value;
            }

            throw new ArgumentException("Invalid value type has been encountered");
        }

        public static string GetFormatString(DateTimeUtil.Pattern pattern)
        {
            switch (pattern)
            {
                case Pattern.Date:
                    return DatePattern;
                case Pattern.Month:
                    return MonthPattern;
                case Pattern.DateTime:
                    return DateTimePattern;
                case Pattern.CalendarDate:
                    return CalendarDatePattern;
                case Pattern.ShrinkedDate:
                    return ShrinkedDatePattern;
                case Pattern.Universal:
                    return DateTimeUniversalPattern;
            }

            throw new ArgumentException("Invalid pattern has been encountered");
        }

        public enum Pattern
        {
            Unknown = 0,
            Date = 1,
            Month = 2,
            DateTime = 3,
            CalendarDate = 4,
            ShrinkedDate = 5,
            Universal = 6
        }

        public enum ValueType
        {
            Unknown = 0,
            Utc = 1,
            Local = 2
        }
    }
}
