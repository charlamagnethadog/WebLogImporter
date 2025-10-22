using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Fenton.WebLogImporter
{
    public static class StringExtensions
    {
        public static DateTime? JsonToDateTime(this string jsDateObj)
        {
            if (jsDateObj == null || jsDateObj == "NaN")
                return null;

            DateTime? date = null;
            try
            {
                if (jsDateObj.Contains("("))
                    jsDateObj = jsDateObj.Substring(jsDateObj.IndexOf("(") + 1);
                if (jsDateObj.Contains(")"))
                    jsDateObj = jsDateObj.Remove(jsDateObj.IndexOf(")"));

                if (double.TryParse(jsDateObj, out var milliseconds))
                    date = new DateTime(1970, 1, 1).AddMilliseconds(milliseconds).ToLocalTime();

                return date;
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<string> ToUpperCase(this IEnumerable<string> list)
        {
            return list.Select(p => p.ToUpper());
        }
        public static string ToProperCase(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            return myTI.ToTitleCase(text.ToLower());
        }
        public static bool ContainsAny(this string stringToSearchIn, params string[] stringsToSearchFor)
        {
            if (string.IsNullOrWhiteSpace(stringToSearchIn))
                return false;

            foreach (var stringToSearchFor in stringsToSearchFor)
            {
                if (stringToSearchFor != null && stringToSearchIn.Contains(stringToSearchFor))
                    return true;
            }
            return false;
        }
        public static bool ContainsAny(this string stringToSearchIn, IEnumerable<string> iEnumerableString)
        {
            return ContainsAny(stringToSearchIn, iEnumerableString.ToArray());
        }
        public static bool EqualsAny(this string stringToSearchIn, params string[] stringsToSearchFor)
        {
            if (string.IsNullOrWhiteSpace(stringToSearchIn))
                return false;

            foreach (var stringToSearchFor in stringsToSearchFor)
            {
                if (stringToSearchFor != null && stringToSearchIn == stringToSearchFor)
                    return true;
            }
            return false;
        }
        public static bool ContainsAll(this string stringToSearchIn, params string[] stringsToSearchFor)
        {
            if (string.IsNullOrWhiteSpace(stringToSearchIn))
                return false;

            foreach (var str in stringsToSearchFor)
            {
                if (!stringToSearchIn.Contains(str))
                    return false;
            }
            return true;
        }
        public static bool ContainsAll(this string stringToSearchIn, IEnumerable<string> iEnumerableString)
        {
            return ContainsAll(stringToSearchIn, iEnumerableString.ToArray());
        }
        public static bool EndsWithAny(this string stringToSearchIn, params string[] stringsToSearchFor)
        {
            if (string.IsNullOrWhiteSpace(stringToSearchIn))
                return false;

            foreach (var stringToSearchFor in stringsToSearchFor)
            {
                if (stringToSearchFor != null && stringToSearchIn.EndsWith(stringToSearchFor))
                    return true;
            }
            return false;
        }
        public static bool EndsWithAny(this string stringToSearchIn, IEnumerable<string> iEnumerableString)
        {
            return EndsWithAny(stringToSearchIn, iEnumerableString.ToArray());
        }
        public static string ToDelimitedString(this IEnumerable elems, string separator =",")
        {
            if (elems == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            foreach (object elem in elems)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                sb.Append(elem);
            }

            return sb.ToString();
        }
        public static string TrimToLength(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string RemoveAll(this string value, IEnumerable<string> stringsToRemove)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            foreach (var str in stringsToRemove)
            {
                value.Replace(str, "");
            }

            return value;
        }
        public static string RemoveUnwantedCharsFromPhoneNumber(this string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return "";

            var array = number.ToCharArray();
            number = string.Empty;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] != '(' && array[i] != ')' && array[i] != '.' && array[i] != '-' && array[i] != ' ')
                {
                    number = number.Insert(number.Length, array[i].ToString());
                }
            }
            return number;
        }

    }
}
