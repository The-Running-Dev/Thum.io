using System.Reflection;
using System.Text.RegularExpressions;

namespace Thum.io.Screenshots
{
    public static class Extensions
    {
        /// <summary>
        /// Checks if string is empty, null or white space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Checks if string is not empty, null or white space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Formats a string with the properties of the given object
        /// </summary>
        /// <param name="sourceString">The source string to interpolate</param>
        /// <param name="values">The object containing the values</param>
        /// <remarks>
        /// Ex:
        /// "Today is {DateTime:h:mm tt on dddd, MMMM dd yyyy} {TimeZone}".FormatWith(
        ///		new { datetime = dateTime, timezone = "CET" }
        /// );
        /// Variables in both the format string and the expressions can be aNY case.
        /// </remarks>
        /// <returns></returns>
        public static string FormatWith(this string sourceString, object values)
        {
            var pattern = @"\{([A-Za-z0-9]+)(:.*?)?\}";

            if (values == null)
            {
                return sourceString;
            }

            return Regex.Replace(sourceString, pattern, (match) =>
            {
                var value = values.GetType()
                    .GetProperty(match.Groups[1].Value, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?
                    .GetValue(values);
                var format = match.Groups[2].Value;

                return value != null ?
                    format.IsNotEmpty()
                        ? string.Format($"{{0:{format.Substring(1)}}}", value)
                        : value.ToString()
                    : string.Empty;
            });
        }
    }
}