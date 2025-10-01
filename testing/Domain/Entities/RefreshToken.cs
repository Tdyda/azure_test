namespace testing.Models;

public sealed class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public string? CreatedByIp { get; private set; }
    public string? CreatedByUserAgent { get; private set; }

    private RefreshToken()
    {
    }

    public static RefreshToken Create(Guid userId, string tokenHash, DateTime nowUtc, DateTime expiresAtUtc, string? ip,
        string? ua)
        => new()
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = expiresAtUtc,
            CreatedByIp = ip,
            CreatedByUserAgent = ua
        };
    
    public bool IsActive(DateTime nowUtc) => RevokedAtUtc is null && ExpiresAtUtc > nowUtc;
    public void Revoke(DateTime nowUtc) => RevokedAtUtc = nowUtc;

    public void ReplaceWith(Guid newTokenId, DateTime nowUtc)
    {
        Revoke(nowUtc);
        ReplacedByTokenId = newTokenId;
    }
}