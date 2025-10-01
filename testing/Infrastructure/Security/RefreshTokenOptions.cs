namespace testing.Infrastructure.Security;

public sealed class RefreshTokenOptions
{
    public string HashKeyBase64 { get; set; } = default!;
    public TimeSpan Lifetime { get; set; } = TimeSpan.FromDays(14);
    public int TokenSizeBytes { get; set; } = 32;
}