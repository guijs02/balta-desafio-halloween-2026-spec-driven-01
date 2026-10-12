using System.Security.Cryptography;

namespace Domain;

public interface IPasswordGenerator
{
    string Generate();
}

public sealed class PasswordGenerator : IPasswordGenerator
{
    private const string LettersAndDigits =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const string Punctuation = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
    private const string AllowedCharacters = LettersAndDigits + Punctuation;

    public string Generate()
    {
        var password = new char[PasswordPolicy.MinimumLength];

        for (var index = 0; index < password.Length; index++)
        {
            password[index] = AllowedCharacters[RandomNumberGenerator.GetInt32(AllowedCharacters.Length)];
        }

        var punctuationIndex = RandomNumberGenerator.GetInt32(password.Length);
        password[punctuationIndex] = Punctuation[RandomNumberGenerator.GetInt32(Punctuation.Length)];

        return new string(password);
    }
}
