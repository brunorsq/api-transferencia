namespace Transferencia.Auth
{
    public class TokenJWT
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
    }
}
