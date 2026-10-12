namespace Domain;

public static class PasswordPolicy
{
    public const int MinimumLength = 16;

    private const string Punctuation = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

    public static bool IsValid(string? password)
    {
        if (password is null || password.Length < MinimumLength)
        {
            return false;
        }

        var containsPunctuation = false;

        foreach (var character in password)
        {
            var isAsciiLetterOrDigit =
                character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9';
            var isPunctuation = Punctuation.Contains(character);

            if (!isAsciiLetterOrDigit && !isPunctuation)
            {
                return false;
            }

            containsPunctuation |= isPunctuation;
        }

        return containsPunctuation;
    }
}
