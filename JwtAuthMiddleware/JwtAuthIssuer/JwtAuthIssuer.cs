using JwtAuthMiddleware.JwtStuff.Factory;
using JwtAuthMiddleware.JwtStuff.Models;


namespace JwtAuthMiddleware.JwtAuthIssuer
{
    public class JwtAuthIssuer : IJwtAuthIssuer
    {
        public string IssueToken(string secretKey, JwtPayload payload, int tokenLifetime)
        {
            JwtFactory jwtFactory = new JwtFactory(secretKey, tokenLifetime);

            string token = jwtFactory.CreateToken(payload);

            return token;
        }
    }
}
