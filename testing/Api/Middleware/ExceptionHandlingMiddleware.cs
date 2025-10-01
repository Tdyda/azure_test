using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace testing.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ProblemDetailsFactory pdf)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblem(ctx, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteProblem(ctx, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteProblem(ctx, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (Exception)
        {
            await WriteProblem(ctx, StatusCodes.Status500InternalServerError, "Unexpected error.");
        }
    }

    private Task WriteProblem(HttpContext ctx, int status, string detail)
    {
        var problem = pdf.CreateProblemDetails(ctx, statusCode: status, detail: detail);
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";
        return ctx.Response.WriteAsJsonAsync(problem);
    }
}