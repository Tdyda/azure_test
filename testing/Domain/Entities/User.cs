using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using testing.Domain.ValueObjects;

namespace testing.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key] public Guid Id { get; private set; }
    [EmailAddress] public Email Email { get; private set; } = null!;

    public string Password { get; private set; } = null!;
    public string UserName { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string Surname { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    private User()
    {
    }
    
    private User(Email email, string userName, string name, string surname, DateTime createdAtUtc)
    {
        Email = email;
        UserName = userName;
        Name = name;
        Surname = surname;
        CreatedAtUtc = createdAtUtc;
    }
    public static User Create(Email email, string username, string name, string surname, DateTime nowUtc) =>
        new(email, username, name, surname, nowUtc);
    
    public static void SetPassword(User user, string password) => user.Password = password;
}