// -----------------------------------------------------------------------
// <copyright file="CustomValidators.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using Loch.Shared.Extensions;

namespace Loch.Shared.Core
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string> NationalCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(n => n.IsValidNationalCode());
        }

        public static IRuleBuilderOptions<T, string> PersianDate<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(n => n.IsPresianDate());
        }

        public static IRuleBuilderOptions<T, string> OnlyContainsEnglishOrPersianLetters<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(n => n.IsOnlyInEnglishOrPersianLetters());
        }

        public static IRuleBuilderOptions<T, string> IsEmpty<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var result = ruleBuilder.Must(n => n.IsNullOrEmpty());
            return result;
        }
    }
}
