namespace OAuthImplementation.AuthorizationServer
{
    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<string> RedirectUris { get; set; }
        public string AllowScopes { get; set; }
        public string BaseUri { get; set; }
    }
}
