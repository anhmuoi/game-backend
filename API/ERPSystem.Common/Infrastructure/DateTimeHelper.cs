namespace ERPSystem.Common.Infrastructure;

public static  class DateTimeHelper
{
    /// <summary>
    /// Method checks if passed string is datetime
    /// </summary>
    /// <param name="dateTime">string text for checking</param>
    /// <returns>bool - if text is datetime return true, else return false</returns>
    public static bool IsDateTime(string dateTime)
    {
        // Allow DateTime string is null
        if (string.IsNullOrEmpty(dateTime))
        {
            return true;
        }

        return dateTime.ConvertDefaultStringToDateTime() != null || DateTime.TryParse(dateTime, out _);
    }
    /// <summary>
    /// convert date time utc to systen tine with tinezone
    /// </summary>
    /// <param name="timeZone"></param>
    /// <returns></returns>
    public static DateTime ConvertDateTimeUTCToSystemTimeZone(this DateTime date, string timeZone)
    {
        try
        {
            if (!string.IsNullOrEmpty(timeZone))
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

                DateTime convertDateTime = TimeZoneInfo.ConvertTimeFromUtc(date, cstZone);
        
                return convertDateTime;
            }

            return date;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return date;
        }
    }
    public static DateTime ConvertTimeUserToUTC(this DateTime date, string timeZone)
    {
        try
        {
            if (!string.IsNullOrEmpty(timeZone))
            {
                TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(date, userTimeZone);
                return utcDateTime;
            }

            return date;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return date;
        }
    }
}