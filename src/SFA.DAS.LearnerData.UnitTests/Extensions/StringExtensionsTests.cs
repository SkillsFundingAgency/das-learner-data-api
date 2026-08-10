using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerData.Extensions;

namespace SFA.DAS.LearnerData.UnitTests.Extensions;

public class StringExtensionsTests
{
    [TestCase(null, 0)]
    [TestCase("123", 123)]
    [TestCase("X123", 0)]
    public void Convert_String_ToLongOrDefault(string? input, long expectedOutput)
    {
        var result = input.ToLongOrDefault();

        result.Should().Be(expectedOutput);
    }
}