namespace Domain;

public sealed class PasswordRecord
{
    private PasswordRecord()
    {
    }

    private PasswordRecord(Guid id, string value, DateTime createdAt)
    {
        Id = id;
        Value = value;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Value { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public static PasswordRecord Create(string value)
    {
        if (!PasswordPolicy.IsValid(value))
        {
            throw new ArgumentException("A senha não atende aos critérios definidos.", nameof(value));
        }

        return new PasswordRecord(Guid.NewGuid(), value, DateTime.UtcNow);
    }
}
