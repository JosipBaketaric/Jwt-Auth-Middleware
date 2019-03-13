
using JwtAuthMiddleware.JwtStuff.Models;

namespace JwtAuthMiddleware.JwtStuff.Factory
{
    internal interface IJwtFactory
    {
        string CreateToken(JwtPayload payload);
        string ExtendToken(string token);
    }
}
