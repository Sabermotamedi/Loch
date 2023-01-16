// -----------------------------------------------------------------------
// <copyright file="BusinessRuleValidationException.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

namespace Loch.Shared.Core.Domain
{
    public class BusinessRuleValidationException : Exception
    {
        public List<Error> Errors { get; }

        public BusinessRuleValidationException(string message) : base(message)
        {
        }

        public BusinessRuleValidationException(List<Error> errorList)
        {
            Errors = errorList;
        }

        public BusinessRuleValidationException(Error error)
        {
            Errors = new List<Error>
            {
                error
            };
        }
    }
}
