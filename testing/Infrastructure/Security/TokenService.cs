using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using testing.Application;
using testing.Application.Abstractions;
using testing.Application.Abstractions.Security;

namespace testing.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly IJwtTokenGenerator _jwt;
    private readonly IClock _clock;
    private readonly RefreshTokenOptions _rtOptions;
    private readonly byte[] _hashKey;
 
    public TokenService(IJwtTokenGenerator jwt, IClock clock, IOptions<RefreshTokenOptions> rtOptions)
    {
        _jwt = jwt;
        _clock = clock;
        _rtOptions = rtOptions.Value;
        _hashKey = Convert.FromBase64String(_rtOptions.HashKeyBase64);
    }
    public (string accessToken, string refreshToken) IssueTokenPair(Guid userId, string email, string username)
    {
        var access = _jwt.GenerateToken(userId, email, username, null);

        var bytes = RandomNumberGenerator.GetBytes(_rtOptions.TokenSizeBytes);
        var refresh = Base64UrlEncode(bytes);
        
        return (access, refresh);
    }

    public string HashRefreshToken(string refreshToken)
    {
        using var h = new HMACSHA256(_hashKey);
        var bytes = System.Text.Encoding.UTF8.GetBytes(refreshToken);
        var mac = h.ComputeHash(bytes);
        return Convert.ToBase64String(mac);
    }

    public DateTime GetRefreshExpiryUtc()
    {
        var r = _clock.UtcNow.Add(_rtOptions.Lifetime);
        return r;
    }

    private static string Base64UrlEncode(byte[] input)
    {
        var r = Convert.ToBase64String(input).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        return r;
    }
}