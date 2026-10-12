using Domain;

namespace Unit;

public sealed class PasswordPolicyTests
{
    [Theory]
    [InlineData("A!bcdefghijklmnop")]
    [InlineData("12345678901234!a")]
    public void IsValid_AcceptsPasswordWithSixteenOrMoreValidCharacters(string password)
    {
        Assert.True(PasswordPolicy.IsValid(password));
    }

    [Theory]
    [InlineData("Abcdefghijklmnop")]
    [InlineData("A!bcdefghijklmn")]
    [InlineData("A!bc defghijklmn")]
    [InlineData("A!bc\ndefghijklmn")]
    [InlineData("A!bcdefghijklmñ")]
    [InlineData("A!bcdefghijklm😀")]
    public void IsValid_RejectsPasswordsOutsideThePolicy(string password)
    {
        Assert.False(PasswordPolicy.IsValid(password));
    }

    [Fact]
    public void IsValid_RejectsNull()
    {
        Assert.False(PasswordPolicy.IsValid(null));
    }

    [Fact]
    public void IsValid_AcceptsEveryPrintableAsciiPunctuationRange()
    {
        const string punctuationPassword = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
        var password = punctuationPassword + "12345678901234567890";

        Assert.True(PasswordPolicy.IsValid(password));
    }

    [Fact]
    public void Generate_AlwaysReturnsValidPasswordOfAtLeastSixteenCharacters()
    {
        var generator = new PasswordGenerator();

        for (var attempt = 0; attempt < 100; attempt++)
        {
            Assert.True(PasswordPolicy.IsValid(generator.Generate()));
        }
    }

    [Fact]
    public void Create_AssignsDifferentIdentifiersToDifferentRecords()
    {
        var firstRecord = PasswordRecord.Create("A!bcdefghijklmnop");
        var secondRecord = PasswordRecord.Create("A!bcdefghijklmnop");

        Assert.NotEqual(Guid.Empty, firstRecord.Id);
        Assert.NotEqual(firstRecord.Id, secondRecord.Id);
    }

    [Fact]
    public void Create_RejectsPasswordThatDoesNotMeetThePolicy()
    {
        Assert.Throws<ArgumentException>(() => PasswordRecord.Create("Abcdefghijklmnop"));
    }
}
