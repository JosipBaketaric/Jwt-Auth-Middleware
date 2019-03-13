using JwtAuthMiddleware.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JwtAuthMiddleware.Common
{
    internal class CommonMethods
    {

        public string GetValueFromRequest(HttpContext context, string prop)
        {
            if (context == null || context.Request == null || context.Request.Headers == null)
            {
                return null;
            }

            var val = context.Request.Headers.Where(x => x.Key == prop).Select(x => x.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val;
        }

        public List<string> GetRequestedClaimFromJwtAuthorizationAttribute(HttpContext context, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            ControllerActionDescriptor route = GetRoute(context, actionDescriptorCollectionProvider);

            var result = route.MethodInfo.CustomAttributes
                .Where(x => x.AttributeType == typeof(JwtAuthorizeAttribute))
                .FirstOrDefault();

            var namedArguments = result.NamedArguments.FirstOrDefault();

            if(namedArguments.MemberName != "Claims")
            {
                return null;
            }

            if(namedArguments.TypedValue == null || namedArguments.TypedValue.Value == null)
            {
                return null;
            }

            //if(namedArguments.TypedValue.Value is )

            string[] convertedToArray = ((IEnumerable)namedArguments.TypedValue.Value).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();


            return convertedToArray.ToList().Select(x => x.Replace("\"", string.Empty)).ToList();
        }

        public bool MethodHasAttribute<T>(HttpContext context, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider) where T : Attribute
        {
            ControllerActionDescriptor route = GetRoute(context, actionDescriptorCollectionProvider);

            if(route == null)
            {
                throw new Exception("JwtAuthMiddleware couldn't resolve route");
            }

            var result = route.MethodInfo.CustomAttributes
                .Where(x => x.AttributeType == typeof(T))
                .FirstOrDefault();


            if (result == null)
            {
                return false;
            }

            return true;
        }

        public bool IsDefaultRequest(HttpContext context)
        {
            if (context != null && context.Request != null && context.Request.Path.HasValue && context.Request.Path.Value == "/")
            {
                return true;
            }
            return false;
        }

        private ControllerActionDescriptor GetRoute(HttpContext context, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            if (context.Request == null || !context.Request.Path.HasValue)
            {
                return null;
            }

            string requestPath = context.Request.Path.Value;
            string method = context.Request.Method;

            var route = UrlPathPaterns.MatchingPathPatern(actionDescriptorCollectionProvider, requestPath, method);

            return route as ControllerActionDescriptor;
        }

    }
}
