namespace SharedProject.Extensions;

public static class StringExtensions
{
    public static string CleanNumberInput(this string input, bool isInteger = false)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var cleaned = new string([.. input.Where(c => char.IsDigit(c) ||
                         (!isInteger && (c == '.' || c == ',')))]);

        return isInteger
            ? cleaned
            : cleaned.Replace(",", ".");
    }
}