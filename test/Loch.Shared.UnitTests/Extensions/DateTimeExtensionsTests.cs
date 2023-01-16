// -----------------------------------------------------------------------
// <copyright file="DateTimeExtensionsTests.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------


namespace Loch.Shared.Extensions
{
    public class DateTimeExtensionsTests
    {
        [Theory]
        [InlineData("1398/04/24")]
        public void PersianDateToDateConverterExtension(string samplePersianDate)
        {
            var georgianDate = samplePersianDate.ToGeorgianDate();
            georgianDate.Should().BeSameDateAs(new DateTime(2019, 07, 15));
        }

        [Theory]
        [InlineData("")]
        [InlineData("1398")]
        [InlineData("1398/1")]
        [InlineData("1398/1123123")]
        [InlineData("1398/1123123/123")]
        [InlineData("8/1/1")]
        [InlineData("98/1/1")]
        [InlineData("1398/1/1")]
        [InlineData("1398/01/1")]
        [InlineData("DummyText")]
        [InlineData("سیب")]
        [InlineData("<br/>")]
        [InlineData("1398 /04/25")]
        [InlineData("1398/04/  25")]
        public void ToGeorgianDateExtension_Should_Throw_InvalidException_When_InputString_Is_Invalid(string samplePersianDate)
        {
            Action invalidDateAction = () => { samplePersianDate.ToGeorgianDate(); };
            invalidDateAction.Should().Throw<ArgumentException>().WithMessage("incorrect date format");
        }

        [Theory]
        [InlineData("1398/04/25")]
        public void ToGeorgianDateExtension_Should_Return_ValidDate_When_InputString_Is_Valid(string samplePersianDate)
        {
            var georgianDate = samplePersianDate.ToGeorgianDate();
            georgianDate.Should().HaveYear(2019).And.HaveMonth(07).And.HaveDay(16);
        }

        [Theory]
        [InlineData("1398/04/25")]
        public void ToPersianDateExtension_Should_Work_When_String_Is_Valid(string samplePersianDate)
        {
            var georgianDate = samplePersianDate.ToGeorgianDate();
            georgianDate.Should().HaveYear(2019).And.HaveMonth(07).And.HaveDay(16);
        }

        [Theory]
        [InlineData("2020-01-01")]
        public void ToPersianDateExtension_Should_Work(string sampleDate)
        {
            var _sampleDate = DateTime.Parse(sampleDate);
            var persianDate = _sampleDate.ToPersianDate();
            persianDate.Should().Be("1398/10/11");
        }

    }
}
