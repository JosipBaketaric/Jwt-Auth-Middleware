using JwtAuthMiddleware.Attributes;
using JwtAuthMiddleware.Common;
using JwtAuthMiddleware.JwtStuff;
using JwtAuthMiddleware.JwtStuff.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthMiddleware.JwtAuthorization
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CommonMethods _commonMethods;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public JwtAuthorizationMiddleware(RequestDelegate next, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _next = next;
            _commonMethods = new CommonMethods();
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            if(context == null || context.Request == null && context.Request.Headers == null || context.Request.Headers.Count == 0)
            {
                context.Response.StatusCode = 403;
                return;
            }

            if(!_commonMethods.MethodHasAttribute<JwtAuthorizeAttribute>(context, _actionDescriptorCollectionProvider))
            {
                await _next(context);
                return;
            }

            string token = context.Request.Headers["token"];
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 403;
                return;
            }

            //Get token payload
            TokenResolver resolver = new TokenResolver();
            JwtPayload payload = resolver.ResolvePayloadFromToken(token);

            //Get route requested claim
            List<string> claimes = _commonMethods.GetRequestedClaimFromJwtAuthorizationAttribute(context, _actionDescriptorCollectionProvider);

            if(claimes != null && (payload.clms == null || payload.clms.Length == 0))
            {
                return;
            }

            //User has to have all claims to access route
            foreach(var claime in claimes)
            {
                if(payload.clms.FirstOrDefault(x => x == claime) == null)
                {
                    context.Response.StatusCode = 403;
                    return;
                }
            }


            await _next(context);
        }    

    }
}
