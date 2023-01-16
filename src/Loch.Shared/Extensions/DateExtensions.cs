// -----------------------------------------------------------------------
// <copyright file="DateExtensions.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Text.RegularExpressions;

namespace Loch.Shared.Extensions
{
    public static class DateExtensions
    {
        /// <summary>
        /// تبدیل تاریخ شمسی به میلادی
        /// </summary>
        /// <param name="date">تاریخ شمسی استرینگ</param>
        /// <returns> DateTime تاریخ میلادی به صورت </returns>
        /// <exception cref="ArgumentException">اکسپشن در صورت غلط بودن ساختار تاریخ</exception>
        public static DateTime ToGeorgianDate(this string date)
        {
            const string pattern = @"^$|^([1][0-9]{3}[/\/]([0][1-6])[/\/]([0][1-9]|[12][0-9]|[3][01])|[1][0-9]{3}[/\/]([0][7-9]|[1][012])[/\/]([0][1-9]|[12][0-9]|(30)))$";
            if (string.IsNullOrEmpty(date) || !Regex.IsMatch(date, pattern))
            {
                throw new ArgumentException("incorrect date format");
            }

            var persianCalendar = new PersianCalendar();
            var dateParts = date.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return persianCalendar.ToDateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]), 0, 0, 0, 0);
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به تاریخ شمسی.
        /// </summary>
        /// <param name="date">تاریخ میلادی.</param>
        /// <returns>.تاریخ شمسی با ساختار مرسوم روز/ماه/سال</returns>
        public static string ToPersianDate(this DateTime date)
        {
            var persianCalendar = new PersianCalendar();
            var year = persianCalendar.GetYear(date);
            var month = persianCalendar.GetMonth(date);
            var day = persianCalendar.GetDayOfMonth(date);
            return $"{year}/{month}/{day}";
        }

        /// <summary>
        /// Checking the persian date.
        /// </summary>
        /// <param name="value">date.</param>
        /// <returns>bool.</returns>
        public static bool IsPresianDate(this string value)
        {
            return Regex.IsMatch(value, @"^(13\d{2}|[1-9]\d)/(1[012]|0?[1-9])/([12]\d|3[01]|0?[1-9])$");
        }
    }
}