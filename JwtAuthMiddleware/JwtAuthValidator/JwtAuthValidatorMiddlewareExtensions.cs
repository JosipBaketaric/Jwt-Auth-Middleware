using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JwtAuthMiddleware.JwtAuthValidator
{
    public static class JwtAuthValidatorMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtAuthValidatorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtAuthorizationMiddleware>();
        }


        public static IServiceCollection AddJwtAuthValidatorMiddleware(this IServiceCollection service, Action<JwtAuthValidatorOptions> options = default)
        {
            options = options ?? (opts => { });
            service.Configure(options);
            return service;
        }

    }
}
