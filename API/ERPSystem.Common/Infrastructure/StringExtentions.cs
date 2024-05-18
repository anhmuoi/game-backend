namespace ERPSystem.Common.Infrastructure;

public static class StringExtentions
{
    /// <summary>
    /// Convert a string to camel case
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string str)
    {
        if (!string.IsNullOrEmpty(str) && str.Length > 1)
        {
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
        return str;
    }
    /// <summary>
    /// Uppercases the first character of a string
    /// </summary>
    /// <param name="input">The string which first character should be uppercased</param>
    /// <returns>The input string with it'input first character uppercased</returns>
    public static string FirstCharToUpper(this string input)
    {
        return string.IsNullOrEmpty(input)
            ? ""
            : string.Concat(input.Substring(0, 1).ToUpper(), input.Substring(1).ToLower());
    }
}