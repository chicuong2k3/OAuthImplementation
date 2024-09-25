using Microsoft.AspNetCore.Mvc;
using OAuthImplementation.AuthorizationServer.Repositories;
using OAuthImplementation.AuthorizationServer.Requests;
using OAuthImplementation.AuthorizationServer.Models;
using OAuthImplementation.AuthorizationServer.Responses;
using System.Text;
using OAuthImplementation.AuthorizationServer.Extensions;
using OAuthImplementation.AuthorizationServer.Constants;

namespace OAuthImplementation.AuthorizationServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClientRepository _clientRepository;
        private readonly ITokenRepository _tokenRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IClientRepository clientRepository,
            ITokenRepository tokenRepository)
        {
            _logger = logger;
            _clientRepository = clientRepository;
            _tokenRepository = tokenRepository;
        }

        [HttpGet("/authorize")]
        public IActionResult Authorize(AuthorizeRequest request)
        {
            _logger.LogInformation("Started authorize endpoint [GET].");

            var client = _clientRepository.GetClientById(request.ClientId);

            if (client is null)
            {
                return BadRequest("Invalid Client Id.");
            }

            _logger.LogInformation("Redirect Uri: {RedirectUri}", request.RedirectUri);

            return View(new ApproveRequest()
            {
                ClientId = client.ClientId,
                RedirectUri = request.RedirectUri,
                ResponseType = request.ResponseType,
                Scope = request.Scope
            });
        }

        [HttpPost("/approve")]
        public IActionResult Approve(ApproveRequest request)
        {
            _logger.LogInformation("Started approve endpoint [POST].");

            var client = _clientRepository.GetClientById(request.ClientId);

            if (client is null)
            {
                return BadRequest("Invalid Client Id.");
            }

            var redirectUriWithoutParams = new Uri(request.RedirectUri).GetLeftPart(UriPartial.Path);

            if (!client.RedirectUris.Contains(redirectUriWithoutParams))
            {
                return BadRequest("Invalid Redirect Uri.");
            }

            var clientScope = client.AllowScopes.Split(' ') ?? [];
            var requestedScope = request.Scope.Split(' ') ?? [];

            if (requestedScope.Except(clientScope).Any())
            {
                return BadRequest("Invalid Scope.");
            }

            if (request.ResponseType.Equals(OAuthConstants.Code))
            {
                var code = Guid.NewGuid().ToString();

                request.RedirectUri = request.RedirectUri.AddParamsToUrl(
                    new Dictionary<string, string>()
                    {
                        { OAuthConstants.Code, code }
                    });

                _logger.LogInformation("Redirect Uri: {RedirectUri}", request.RedirectUri);

                _tokenRepository.SaveAuthorizeRequestInformation(code, new AuthorizeRequestInformation()
                {
                    ClientId = client.ClientId,
                    RedirectUri = request.RedirectUri,
                    ResponseType = request.ResponseType,
                    Scope = requestedScope,
                    User = User
                });


                return Redirect(request.RedirectUri);
            }

            return BadRequest("Unsupported Response Type.");
        }

        [HttpPost("/token")]
        public IActionResult Token(
            string code,
            string grantType,
            string redirectUri)
        {
            _logger.LogInformation("Started token endpoint [POST].");

            var clientId = string.Empty;
            var clientSecret = string.Empty;

            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                var clientCredentialsString = authorizationHeader
                    .Substring(authorizationHeader.IndexOf("Basic") + 5)
                    .Trim();

                _logger.LogInformation($"Client Credentials: {clientCredentialsString}");

                var clientCredentials = Encoding.UTF8.GetString(
                        Convert.FromBase64String(clientCredentialsString))
                                .Split(':');
                clientId = clientCredentials[0];
                clientSecret = clientCredentials[1];
            }

            if (Request.Form.ContainsKey(OAuthConstants.ClientId) && Request.Form.ContainsKey(OAuthConstants.ClientSecret))
            {
                if (!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(clientSecret))
                {
                    return BadRequest();
                }

                clientId = Request.Form[OAuthConstants.ClientId];
                clientSecret = Request.Form[OAuthConstants.ClientSecret];
            }

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                return Unauthorized();
            }

            var client = _clientRepository.GetClientById(clientId);
            if (client == null)
            {
                return Unauthorized();
            }

            if (clientSecret != client.ClientSecret)
            {
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(grantType))
            {
                if (grantType.Equals(OAuthConstants.AuthorizationCode))
                {
                    var authorizeRequest = _tokenRepository.GetAuthorizeRequestInformation(code);

                    if (authorizeRequest is null)
                    {
                        return BadRequest("Invalid Authorization Code.");
                    }

                    _tokenRepository.DeleteAuthorizeRequestInformation(code);

                    if (authorizeRequest.ClientId.Equals(client.ClientId))
                    {
                        var accessToken = _tokenRepository.GenerateAccessToken(
                            Guid.NewGuid().ToString(),
                            new List<System.Security.Claims.Claim>()
                            {
                                //new System.Security.Claims.Claim("sub", )
                            },
                            client.BaseUri,
                            120
                        );

                        var clientScope = string.Empty;

                        if (authorizeRequest.Scope is not null)
                        {
                            clientScope = string.Join(' ', authorizeRequest.Scope);
                        }

                        var accessTokenInfo = new AccessTokenInformation()
                        {
                            AccessToken = accessToken,
                            TokenType = "Bearer",
                            ClientId = client.ClientId,
                            Scope = clientScope
                        };
                        _tokenRepository.SaveAccessToken(accessTokenInfo);

                        _logger.LogInformation("Issued access token.");

                        return Ok(new TokenEndpointResponse()
                        {
                            AccessToken = accessTokenInfo.AccessToken
                        });
                    }
                }

                return BadRequest("Unsupported Grant Type.");
            }

            return BadRequest("Invalid Grant Type.");
        }
    }
}
