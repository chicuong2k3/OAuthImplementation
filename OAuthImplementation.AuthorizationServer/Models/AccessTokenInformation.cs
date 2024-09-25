namespace OAuthImplementation.AuthorizationServer.Models
{
    public class AccessTokenInformation
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string ClientId { get; set; }
        public string Scope { get; set; }
    }
}
