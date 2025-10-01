namespace testing.Application.Abstractions.Auth.Login;

public sealed record LoginResponse(string AccessToken, string RefreshToken);