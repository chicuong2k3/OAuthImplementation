using Microsoft.IdentityModel.Tokens;
using OAuthImplementation.AuthorizationServer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OAuthImplementation.AuthorizationServer.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private readonly Dictionary<string, AuthorizeRequestInformation> _authorizationTokenMapper = [];
        private readonly List<AccessTokenInformation> _accessTokenMapper = [];
        private readonly IConfiguration _configuration;

        public void DeleteAuthorizeRequestInformation(string code)
        {
            _authorizationTokenMapper.Remove(code);
        }

        public string GenerateAccessToken()
        {
            return "Access_Token_1234567890";
        }

        public string GenerateAccessToken(
            string userId, 
            List<Claim> claims,
            string audience, 
            int lifeTimeMinutes)
        {
            var secretKey = Encoding.UTF8.GetBytes("secretkey_123456789123131232133132");
            var key = new SymmetricSecurityKey(secretKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AuthServer:BaseUri"),
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(lifeTimeMinutes),
                signingCredentials: creds);

            // Return token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public AuthorizeRequestInformation? GetAuthorizeRequestInformation(string code)
        {
            return _authorizationTokenMapper.GetValueOrDefault(code);
        }

        public void SaveAccessToken(AccessTokenInformation accessTokenInformation)
        {
            _accessTokenMapper.Add(accessTokenInformation);
        }


        public void SaveAuthorizeRequestInformation(string code, AuthorizeRequestInformation requestInformation)
        {
            _authorizationTokenMapper.Add(code, requestInformation);
        }
    }
}
