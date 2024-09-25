namespace OAuthImplementation.AuthorizationServer.Requests
{
    public class AuthorizeRequest
    {
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string ResponseType { get; set; }

        public string Scope { get; set; }
    }
}
