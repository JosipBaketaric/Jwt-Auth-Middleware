using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using JwtAuthMiddleware.Attributes;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using JwtAuthMiddleware.JwtStuff.Validator;
using JwtAuthMiddleware.JwtStuff.Factory;
using JwtAuthMiddleware.Common;

namespace JwtAuthMiddleware.JwtAuthValidator
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtAuthValidatorOptions _options;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private CommonMethods commonMethods;

        public JwtAuthorizationMiddleware(RequestDelegate next, IOptionsMonitor<JwtAuthValidatorOptions> optionsAccessor, 
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _options = optionsAccessor.CurrentValue;
            _next = next;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            commonMethods = new CommonMethods();
        }

        /// <summary>
        /// Token validation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (commonMethods.IsDefaultRequest(context))
            {
                await _next(context);
                return;
            }

            // 1. Check if method has SkpJwtAuthAttribute
            if (commonMethods.MethodHasAttribute<SkpJwtAuthAttribute>(context, _actionDescriptorCollectionProvider))
            {
                await _next(context);
                return;
            }

            // 2. Check token and extend it
            string token = commonMethods.GetValueFromRequest(context, _options.HeaderTokenBearerName);

            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            JwtValidator validator = new JwtValidator();
            if(!validator.IsValid(token, _options.SecretKey))
            {
                return;
            }

            //Extend token
            string extendedToken;
            IJwtFactory jwtFactory = new JwtFactory(_options.SecretKey, _options.TokenTimeValidity);

            try
            {
                extendedToken = jwtFactory.ExtendToken(token);
            }
            catch(Exception ex)
            {
                return;
            }

            if (string.IsNullOrEmpty(extendedToken))
            {
                return;
            }

            //Append new token to response header
            context.Response.Headers.Add(_options.HeaderTokenBearerName, extendedToken);

            await _next(context);
        }    

    }
}
