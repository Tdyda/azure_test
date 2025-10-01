using MediatR;

namespace testing.Application.Abstractions.Auth.Login;

public sealed record LoginCommand(string Email, string Password, string? Ip, string? UserAgent) : IRequest<LoginResponse>;