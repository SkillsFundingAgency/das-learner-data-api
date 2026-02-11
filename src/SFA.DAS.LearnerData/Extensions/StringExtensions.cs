namespace SFA.DAS.LearnerData.Extensions;

public static class StringExtensions
{
    public static long ToLongOrDefault(this string? input)
    {
        if (input != null && long.TryParse(input, out long result))
            return result;

        return 0;
    }
}