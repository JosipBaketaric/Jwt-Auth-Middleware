using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace JwtAuthMiddleware.JwtAuthorization
{
    public class JwtAuthorizationMiddlewarePipeline
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseJwtAuthorizationMiddleware();
        }
    }
}
