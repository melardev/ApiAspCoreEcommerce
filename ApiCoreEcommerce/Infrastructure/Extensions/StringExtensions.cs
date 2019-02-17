using System.Text;
using System.Text.RegularExpressions;

namespace ApiCoreEcommerce.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string Slugify(this string value)
        {
            //First to lower case
            value = value.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);

            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            //Trim dashes from end
            value = value.Trim('-', '_');

            //Replace double occurences of - or _
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }

        public static string RemoveAccent(this string txt)
        {
            //   byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            // return System.Text.Encoding.ASCII.GetString(bytes);
            return "";
        }
    }
}