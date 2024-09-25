using OAuthImplementation.AuthorizationServer.Models;
using System.Security.Claims;

namespace OAuthImplementation.AuthorizationServer.Repositories
{
    public interface ITokenRepository
    {
        void SaveAuthorizeRequestInformation(string code, AuthorizeRequestInformation requestInformation);
        AuthorizeRequestInformation? GetAuthorizeRequestInformation(string code);
        void DeleteAuthorizeRequestInformation(string code);
        void SaveAccessToken(AccessTokenInformation accessTokenInformation);
        string GenerateAccessToken(string userId, List<Claim> claims, string audience, int lifeTimeMinutes);
    }
}
