namespace testing.Application.Users.Create;

public sealed record CreateUserRequest(string Email, string Name, string Surname, string Username, string Password);