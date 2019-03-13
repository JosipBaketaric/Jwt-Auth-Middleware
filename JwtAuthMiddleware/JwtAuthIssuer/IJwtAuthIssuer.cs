using JwtAuthMiddleware.JwtStuff.Models;

namespace JwtAuthMiddleware.JwtAuthIssuer
{
    public interface IJwtAuthIssuer
    {
        string IssueToken(string secretKey, JwtPayload payload, int tokenLifetime);
    }
}
