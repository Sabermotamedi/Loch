using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Loch.Shared.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualCaseInsensitive(this string source, string dest)
        {
            if (source.IsNull())
            {
                return false;
            }

            return source.Equals(dest, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s);
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return !s.IsNullOrEmpty();
        }

        public static T ToEnum<T>(this string s) where T : struct
        {
            return Enum.Parse<T>(s, true);
        }

        public static T? TryToEnum<T>(this string s) where T : struct
        {
            if (s.IsNullOrEmpty())
            {
                return null;
            }

            return Enum.Parse<T>(s, true);
        }

        public static string FormatWith(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

        public static string FixPersianYK(this string value)
        {
            return value.Replace('ي', 'ی').Replace("ك", "ک");
        }

        public static string ConvertDigitChar(this string str, CultureInfo source, CultureInfo destination)
        {
            for (int i = 0; i <= 9; i++)
            {
                str = str.Replace(source.NumberFormat.NativeDigits[i], destination.NumberFormat.NativeDigits[i]);
            }

            return str;
        }

        public static string ConvertDigitChar(this int digit, CultureInfo destination)
        {
            string res = digit.ToString();
            for (int i = 0; i <= 9; i++)
            {
                res = res.Replace(i.ToString(), destination.NumberFormat.NativeDigits[i]);
            }

            return res;
        }

        public static bool IsOnlyInEnglishOrPersianLetters(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, @"^[a-zA-Z\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\u200C\u200F ]+$");
        }

        public static bool IsValidNationalCode(this string input)
        {
            if (!Regex.IsMatch(input, @"^(?!(\d)\1{9})\d{10}$"))
            {
                return false;
            }

            if (!Regex.IsMatch(input, @"^\d{10}$"))
            {
                return false;
            }

            var check = Convert.ToInt32(input.Substring(9, 1));
            var sum = Enumerable.Range(0, 9)
                .Select(x => Convert.ToInt32(input.Substring(x, 1)) * (10 - x))
                .Sum() % 11;

            return (sum < 2 && check == sum) || (sum >= 2 && check + sum == 11);
        }

        public static bool IsValidLegalNationalCode(this string input)
        {
            if (!Regex.IsMatch(input, @"^\d{11}$"))
            {
                return false;
            }

            if (long.Parse(input) == 0)
            {
                return false;
            }

            var y = long.Parse(input.Substring(10, 1));
            var d = long.Parse(input.Substring(9, 1)) + 2;
            var z = new[] { 29, 27, 23, 19, 17 };
            long s = 0;
            for (var i = 0; i < 10; i++)
            {
                s += (d + long.Parse(input.Substring(i, 1))) * z[i % 5];
            }

            s = s % 11;
            if (s == 10)
            {
                s = 0;
            }

            return y == s;
        }

        public static bool IsValidMobileNo(this string value)
        {
            return Regex.IsMatch(value.ToString(), "^(09)[0-9]{9}$");
        }

        public static bool IsValidPhoneNo(this string value)
        {
            return Regex.IsMatch(value.ToString(), @"^0\d{2,3}\d{8}$");
        }

        public static bool IsEnAlphNumeric(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, @"^[A-Za-z0-9 ]+$");
        }
    }
}
