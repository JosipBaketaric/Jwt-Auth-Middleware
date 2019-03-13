namespace JwtAuthMiddleware.JwtAuthValidator
{
    //Some day allow user to provide its implementation of validator and token factory. 
    public class JwtAuthValidatorOptions
    {
        public string SecretKey { get; set; }
        public string HeaderTokenBearerName { get; set; } = "token";
        public int TokenTimeValidity { get; set; } = 10;

    }
}
