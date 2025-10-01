using MediatR;

namespace testing.Application.Abstractions.Auth.Refresh;

public record RefreshTokenCommand(string RefreshToken, string? Ip, string? UserAgent) : IRequest<RefreshTokenResponse>;