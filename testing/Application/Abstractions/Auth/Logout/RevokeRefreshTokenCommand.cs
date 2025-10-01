using MediatR;

namespace testing.Application.Abstractions.Auth.Logout;

public sealed record RevokeRefreshTokenCommand(string RefreshToken) : IRequest<Unit>;