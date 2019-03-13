using JwtAuthMiddleware.JwtStuff.Models;
using JwtAuthMiddleware.JwtStuff.Signing;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;


namespace JwtAuthMiddleware.JwtStuff.Validator
{
    internal class JwtValidator : IJwtValidator
    {
        public bool IsValid(string token, string secretKey)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            //1. split token parts
            var tokenParts = token.Split(".");

            if(tokenParts.Length != 3)
            {
                return false;
            }

            string signature = tokenParts[2];

            //Check integrity
            SigningCalculator signingCalculator = new SigningCalculator();
            string calculatedSignature = signingCalculator.CalculateSignature(secret: secretKey, encodedHeader: tokenParts[0], encodedPayload: tokenParts[1], algorithm: SigningAlgorithmsSupportedEnum.HMACSHA256);

            if(signature != calculatedSignature)
            {
                return false;
            }

            //Check expiration time from payload
            string payloadString = Base64Decode(tokenParts[1]);
            JwtPayload payload = JsonConvert.DeserializeObject<JwtPayload>(payloadString);

            if (string.IsNullOrEmpty(payload.exp))
            {
                return false;
            }

            if(!long.TryParse(payload.exp, out long expirationTime))
            {
                return false;
            }

            long currentTicks = DateTime.UtcNow.Ticks;

            if(expirationTime <= currentTicks)
            {
                return false;
            }

            return true;
        }


        private string Base64Decode(string plainText)
        {
            return Base64UrlEncoder.Decode(plainText);
        }


    }
}
