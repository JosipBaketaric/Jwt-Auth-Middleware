using JwtAuthMiddleware.JwtStuff.Models;
using JwtAuthMiddleware.JwtStuff.Signing;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace JwtAuthMiddleware.JwtStuff.Factory
{
    /// <summary>
    /// JWT format: header.payload.signature
    /// </summary>
    internal class JwtFactory : IJwtFactory
    {
        private readonly int _tokenTimeValidity;
        private readonly string _secretKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secretKey">Used for signing token</param>
        /// <param name="tokenTimeValidity">Token lifetime (in minutes). Recommendation: max 60 minutes </param>
        public JwtFactory(string secretKey, int tokenTimeValidity)
        {
            this._secretKey = secretKey;
            _tokenTimeValidity = tokenTimeValidity;
        }

        public string ExtendToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token not valid");
            }

            //1. Decompose token
            var tokenSplit = token.Split(".");

            if (tokenSplit.Length != 3)
            {
                throw new Exception("Token not valid");
            }

            //string headerPartDecoded = Base64Decode(tokenSplit[0]);
            string payloadPartDecoded = Base64Decode(tokenSplit[1]);


            JwtPayload payload = JsonConvert.DeserializeObject<JwtPayload>(payloadPartDecoded);
            payload.exp = GenerateTokenExpirationTime();

            string extendedPayload = Base64Encode(JsonConvert.SerializeObject(payload));

            //3. Sign token
            SigningCalculator signingCalculator = new SigningCalculator();
            var signature = signingCalculator.CalculateSignature(_secretKey, tokenSplit[0], extendedPayload, SigningAlgorithmsSupportedEnum.HMACSHA256);

            string resultToken = GenerateToken(tokenSplit[0], extendedPayload, signature);

            return resultToken;
        }

        public string CreateToken(JwtPayload payload)
        {
            //1. Create token header
            string header = GenerateTokenHeader();
            //2. Create token payload
            string payloadString = GeneratePayload(payload);
            //3. Sign token
            SigningCalculator signingCalculator = new SigningCalculator();
            string signature = signingCalculator.CalculateSignature(secret: _secretKey, encodedHeader: header, encodedPayload: payloadString, algorithm: SigningAlgorithmsSupportedEnum.HMACSHA256);

            //Put all together
            string token = GenerateToken(header, payloadString, signature);

            return token;
        }


        private string GenerateTokenExpirationTime()
        {
            return DateTime.UtcNow.AddMinutes(_tokenTimeValidity).Ticks.ToString();
        }

        /// <summary>
        /// Header is used for token type and signing algorithm
        /// Only JWT type
        /// Signing algorithm will be HS256 for now 
        /// </summary>
        /// <returns></returns>
        private string GenerateTokenHeader()
        {
            //First create header object and convert it to JSON
            JwtHeader header = new JwtHeader("HS256", "JWT");

            string jsonHeader = JsonConvert.SerializeObject(header);

            //Encode JSON to Base64Url
            string encodedHeader = Base64Encode(jsonHeader);

            return encodedHeader;
        }

        //TODO: check for payload size
        private string GeneratePayload(JwtPayload payload)
        {
            var expirationTime = GenerateTokenExpirationTime();
            payload.exp = expirationTime;

            string jsonPayload = JsonConvert.SerializeObject(payload);

            //Encode JSON to Base64Url
            string encodedPayload = Base64Encode(jsonPayload);

            return encodedPayload;
        }

        /// <summary>
        /// Put all together and generate result token
        /// </summary>
        /// <param name="header"></param>
        /// <param name="payload"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        private string GenerateToken(string header, string payload, string signature)
        {
            var sb = new StringBuilder();

            sb.Append(header);
            sb.Append(".");
            sb.Append(payload);
            sb.Append(".");
            sb.Append(signature);

            return sb.ToString();
        }

        #region Signing algorithms
        private byte[] EncodeHMACSHA256(byte[] value, byte[] secret)
        {
            using (HMACSHA256 encoder = new HMACSHA256(secret))
            {
                return encoder.ComputeHash(value);
            }
        }
        #endregion

        #region Helper methods
        private string Base64Decode(string plainText)
        {
            return Base64UrlEncoder.Decode(plainText);
        }

        private string Base64Encode(string plainText)
        {
            return Base64UrlEncoder.Encode(plainText);
        }
        #endregion

    }

}
