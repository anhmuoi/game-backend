using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using TimeZoneConverter;

namespace ERPSystem.Common.Infrastructure;

public static class Helpers
{
    /// <summary>
    /// Get account id
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static int GetAccountId(this ClaimsPrincipal user)
    {
        int.TryParse(
            user.Claims.FirstOrDefault(m => m.Type == Constants.ClaimName.AccountId)?.Value, out var value);
        return value;
    }
    /// <summary>
    /// Get time zone
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static string GetTimezone(this ClaimsPrincipal user)
    {
        return user.Claims.FirstOrDefault(m => m.Type == Constants.ClaimName.Timezone)?.Value ?? string.Empty;
    }
    /// <summary>
    /// Get User Name
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static string GetUsername(this ClaimsPrincipal user)
    {
        return user.Claims.FirstOrDefault(m => m.Type == Constants.ClaimName.UserName)?.Value ?? string.Empty;
    }
    public static int GetUserId(this ClaimsPrincipal user)
    {
        try
        {
            return int.Parse(user.Claims.FirstOrDefault(m => m.Type == Constants.ClaimName.UserId)?.Value ?? "0");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }
    /// <summary>
    /// Check if the email is valid or not
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns></returns>
    public static bool IsEmailValid(this string emailAddress)
    {
        try
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return false;
            }
                
            MailAddress m = new MailAddress(emailAddress);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static string ConvertDefaultDateTimeToString(this DateTime date, string format = Constants.Settings.DateTimeFormatDefault)
    {
        try
        {
            return date.ToString(format);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    
    public static DateTime ConvertDefaultStringToDateTime(this string date, string format = Constants.Settings.DateTimeFormatDefault)
    {
        try
        {
            if (date.Length <= "MM.dd.yyyy".Length) date = date.Trim() + " 00:00:00";
            
            return DateTime.SpecifyKind(DateTime.ParseExact(date, format, null), DateTimeKind.Utc);
        }
        catch (Exception ex)
        {
            return DateTime.MinValue;
        }
    }
    /// <summary>
    /// Check property in object
    /// </summary>
    /// <param name="name"></param>
    /// <param name="textDefault"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string CheckPropertyInObject<T>(string name, string textDefault)
    {
        char[] names = name.ToCharArray();
        names[0] = char.ToUpper(names[0]);
        name = new string(names);
        PropertyInfo info = typeof(T).GetProperty(name);
        return info != null ? name : textDefault;
    }
    /// <summary>
    /// Convert date  to specify setting date string
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ToSettingDateString(this DateTime date)
    {
        var culture = Thread.CurrentThread.CurrentCulture.Name;
        string dateFormat = ApplicationVariables.Configuration[Constants.Settings.DateTimeServerFormat + ":" + culture];
        return date.ToString(dateFormat);
    }
    /// <summary>
    /// Get date server format by current culture
    /// </summary>
    /// <returns></returns>
    public static string GetDateServerFormat()
    {
        var culture = Thread.CurrentThread.CurrentCulture.Name;
        return ApplicationVariables.Configuration[Constants.Settings.DateTimeServerFormat + ":" + culture];
    }
    /// <summary>
    /// Comparre date
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public static bool CompareDate(string startDate, string endDate)
    {
        if (DateTimeHelper.IsDateTime(startDate) && DateTimeHelper.IsDateTime(endDate))
        {
            return Convert.ToDateTime(endDate).Date.Subtract(Convert.ToDateTime(startDate).Date).Days >= 0;
        }
        return true;
    }
    public static string GenerateCustomId(this int id, string key)
    {
        return string.Concat(key,"_",id);
    }
    public static TimeZoneInfo ToTimeZoneInfo(this string timeZone)
    {
        try
        {
            TimeZoneInfo cstZone = null;
            try
            {
                cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }
            catch
            {
                cstZone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone));
            }

            return cstZone;
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Local;
        }
    }

    public static DateTime ConvertToUserTime(this DateTime date, string userTimeZone = "")
    {
        if (date == DateTime.MinValue || DateTime.MaxValue - date <= new TimeSpan(0, 0, 1)) return date;

        // var culture = Thread.CurrentThread.CurrentCulture.Name;
        // string dateFormat = ApplicationVariables.Configuration[Constants.DateServerFormat + ":" + culture];

        if (!String.IsNullOrEmpty(userTimeZone))
        {
            TimeZoneInfo cstZone;

            try
            {
                cstZone = userTimeZone.ToTimeZoneInfo();

                DateTime cstTime = date.AddSeconds(cstZone.BaseUtcOffset.TotalSeconds);

                return cstTime;
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (ArgumentOutOfRangeException)
            {
                cstZone = userTimeZone.ToTimeZoneInfo();

                var maxDiff = DateTime.MaxValue - date;
                var minDiff = date - DateTime.MinValue;

                if (maxDiff <= cstZone.BaseUtcOffset)
                {
                    date = DateTime.MaxValue;
                }
                else if (minDiff <= cstZone.BaseUtcOffset)
                {
                    date = DateTime.MinValue;
                }
            }
        }
        return date;
    }

    public static DateTime ConvertToSystemTime(this DateTime date, string userTimeZone = "")
    {
        if (date == DateTime.MinValue || DateTime.MaxValue - date <= new TimeSpan(0, 0, 1)) return date;

        // var culture = Thread.CurrentThread.CurrentCulture.Name;
        // string dateFormat = ApplicationVariables.Configuration[Constants.DateServerFormat + ":" + culture];

        if (!String.IsNullOrEmpty(userTimeZone))
        {
            TimeZoneInfo cstZone;

            try
            {
                cstZone = userTimeZone.ToTimeZoneInfo();

                DateTime cstTime = date.Subtract(cstZone.BaseUtcOffset);

                return cstTime;
            }
            catch (TimeZoneNotFoundException)
            {

            }
            catch (ArgumentOutOfRangeException)
            {
                cstZone = userTimeZone.ToTimeZoneInfo();

                var maxDiff = DateTime.MaxValue - date;
                var minDiff = date - DateTime.MinValue;

                if (maxDiff <= cstZone.BaseUtcOffset)
                {
                    date = DateTime.MaxValue;
                }
                else if (minDiff <= cstZone.BaseUtcOffset)
                {
                    date = DateTime.MinValue;
                }
            }
        }
        return date;
    }
    
}