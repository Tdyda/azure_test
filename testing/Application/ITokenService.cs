namespace testing.Application;

public interface ITokenService
{
    (string accessToken, string refreshToken) IssueTokenPair(Guid userId, string email, string username);
    string HashRefreshToken(string refreshToken);
    DateTime GetRefreshExpiryUtc();
}