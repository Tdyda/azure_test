namespace testing.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; set; }
    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Email is required", nameof(value));
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(value);
            if (addr.Address != value) throw new Exception();
        }
        catch { throw new ArgumentException($"Email is invalid", nameof(value)); }
        
        return new Email(value.Trim());
    }
}