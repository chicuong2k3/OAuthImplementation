namespace OAuthImplementation.AuthorizationServer.Requests
{

    public class ApproveRequest
    {
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public string ResponseType { get; set; }
        public string RedirectUri { get; set; }
    }
}
