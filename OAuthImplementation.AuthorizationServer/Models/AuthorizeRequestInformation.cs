using System.Security.Claims;

namespace OAuthImplementation.AuthorizationServer.Models
{
    public class AuthorizeRequestInformation
    {
        public string ClientId { get; set; }
        public string ResponseType { get; set; }
        public string RedirectUri { get; set; }
        public string[] Scope { get; set; }
        public ClaimsPrincipal User { get; set; }
    }
}
