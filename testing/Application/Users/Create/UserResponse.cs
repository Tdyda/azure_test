namespace testing.Application.Users.Create;

public sealed record UserResponse(Guid Id, string Email, string Name, string Surname, string Username, DateTime CreatedAtUtc);