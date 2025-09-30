using System.Net.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using testing.Application.Abstractions;
using testing.Application.Abstractions.Persistence;
using testing.Application.Abstractions.Security;
using testing.Data;
using testing.Data.Repositories;
using testing.Infrastructure.Security;
using testing.Models;

namespace testing.Infrastructure.DependencyInjection;

public static class InfrastructureComposition
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connStr)
    {
        services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connStr));

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        
        return services;
    }
}

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}