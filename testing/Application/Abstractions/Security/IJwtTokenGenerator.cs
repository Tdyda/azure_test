namespace testing.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, string username, IEnumerable<KeyValuePair<string, string>>? extraClaims);
}