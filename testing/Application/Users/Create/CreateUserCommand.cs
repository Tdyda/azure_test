using MediatR;

namespace testing.Application.Users.Create;

public sealed record CreateUserCommand(string Email, string Name, string Surname, string Username, string Password) : IRequest<UserResponse>;