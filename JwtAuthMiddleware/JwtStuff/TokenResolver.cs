using JwtAuthMiddleware.JwtStuff.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;

namespace JwtAuthMiddleware.JwtStuff
{
    public class TokenResolver
    {
        public JwtPayload ResolvePayloadFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new NullReferenceException();
            }

            var tokenSplit = token.Split(".");

            if(tokenSplit.Length != 3)
            {
                throw new ArgumentException();
            }

            string payloadPart = tokenSplit[1];
            string decodedPayload = Base64UrlEncoder.Decode(payloadPart);

            JwtPayload payload = JsonConvert.DeserializeObject<JwtPayload>(decodedPayload);

            return payload;
        }
    }
}
