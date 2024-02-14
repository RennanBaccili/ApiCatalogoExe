using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace ApiCatalogo.Exceptions;

public static class ApiExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var error = contextFeature.Error;
                    string message = error.Message;
                    string trace = error.StackTrace;
                    var errorDetails = new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = message,
                        Trace = trace
                    };
                    await context.Response.WriteAsync(errorDetails.ToString());
                }
            });
        });
    }
}
