using System;
using System.Collections.Generic;
using System.Text;

namespace JwtAuthMiddleware.Attributes
{
    public class JwtAuthorizeAttribute : Attribute
    {
        public string[] Claims { get; set; }
        public JwtAuthorizeAttribute()
        {
        }
    }
}
