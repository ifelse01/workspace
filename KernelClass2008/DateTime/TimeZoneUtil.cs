using System;

namespace KernelClass
{
    /// <summary>
    /// TimeZoneInfoUtil is a helper class for handling TimeZone related operations.
    /// </summary>
    public static class TimeZoneUtil
    {
        public static DateTime ConvertFromUTC(DateTime utcDateTime, string countryCode)
        {
            var offset = GetUtcOffset(countryCode);
            var countryDateTime = utcDateTime.AddMinutes(offset);

            return countryDateTime;
        }

        public static DateTime ConvertToUTC(DateTime countryDateTime, string countryCode)
        {
            var offset = GetUtcOffset(countryCode);
            var utcDateTime = countryDateTime.AddMinutes(-offset);

            return utcDateTime;
        }

        public static DateTime ConvertFromCountryToCountry(DateTime fromCountryDateTime, string fromCountryCode, string toCountryCode)
        {
            var utcDateTime = ConvertToUTC(fromCountryDateTime, fromCountryCode);
            var toCountryDateTime = ConvertFromUTC(utcDateTime, toCountryCode);

            return toCountryDateTime;
        }

        public static double GetUtcOffset(string countryCode)
        {
            //Based on the implementation in v6
            string countryCodeUpper = countryCode.ToUpperInvariant();

            switch (countryCodeUpper)
            {
                case "HK":
                    return 480;		//8 * 60
                case "SG":
                    return 480;		//8 * 60
                case "MY":
                    return 480;		//8 * 60
                case "PH":
                    return 480;		//8 * 60
                case "TH":
                    return 420;		//7 * 60
                case "TW":
                    return 480;		//8 * 60
                case "IN":
                    return 480;		//8 * 60
                case "ID":
                    return 420;		//7 * 60
                case "AU":
                    return 660;		//11 * 60
                case "US":
                    return -420;	//-7 * 60
                case "KR":
                    return 540;		//9 * 60
                case "CN":
                    return 480;		//8 * 60			
                default:
                    throw new ArgumentException("Invalid country code: " + countryCode);
            }
        }
    }
}
