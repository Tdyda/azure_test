using FluentValidation;
using MediatR;
using testing.Application.Users.Create;

namespace testing.Api.DependencyInjection;

public static class ApiComposition
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));
        
        services.AddValidatorsFromAssembly(typeof(CreateUserRequest).Assembly, includeInternalTypes: true);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));
        
        return services;
    }
}

public sealed class RequestLoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var name = typeof(TRequest).Name;

        // Uwaga na dane wrażliwe; nie loguj haseł!
        var shouldLogPayload =
            !name.Contains("Password", StringComparison.OrdinalIgnoreCase) &&
            !name.Contains("CreateUser", StringComparison.OrdinalIgnoreCase);

        if (shouldLogPayload)
            _logger.LogInformation("Handling {RequestName}: {@Request}", name, request);
        else
            _logger.LogInformation("Handling {RequestName}", name);

        try
        {
            var response = await next();
            _logger.LogInformation("Handled {RequestName}", name);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {RequestName}", name);
            throw;
        }
    }
}