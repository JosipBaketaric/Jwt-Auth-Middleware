
namespace JwtAuthMiddleware.JwtStuff.Validator
{
    internal interface IJwtValidator
    {
        bool IsValid(string token, string secretKey);
    }
}
