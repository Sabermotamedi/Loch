// -----------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------



namespace Loch.Shared.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("09124210251")]
        public void IsValidMobileNo_Should_Work_When_IsValid(string sampleMobileNo)
        {
            var isValid = sampleMobileNo.IsValidMobileNo();
            isValid.Should().Be(true);
        }

        [Theory]
        [InlineData("9124210251")]
        public void IsValidMobileNo_Should_Not_Work_When_IsInvalid(string sampleMobileNo)
        {
            var isValid = sampleMobileNo.IsValidMobileNo();
            isValid.Should().Be(false);
        }

        [Theory]
        [InlineData("02144441386")]
        public void IsValidPhoneNo_Should_Work_When_IsValid(string samplePhoneNo)
        {
            var isValid = samplePhoneNo.IsValidPhoneNo();
            isValid.Should().Be(true);
        }

        [Theory]
        [InlineData("021 44441386 الی 7")]
        public void IsValidPhoneNo_Should_Not_Work_When_IsInvalid(string samplePhoneNo)
        {
            var isValid = samplePhoneNo.IsValidPhoneNo();
            isValid.Should().Be(false);
        }

        [Theory]
        [InlineData("test 2")]
        public void IsEnAlphNumeric_Should_Work_When_IsValid(string mpleName)
        {
            var isValid = mpleName.IsEnAlphNumeric();
            isValid.Should().Be(true);
        }

        [Theory]
        [InlineData("jhj ت")]
        public void IsEnAlphNumeric_Should_Not_Work_When_IsInvalid(string mpleName)
        {
            var isValid = mpleName.IsEnAlphNumeric();
            isValid.Should().Be(false);
        }
    }
}
