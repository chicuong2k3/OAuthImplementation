using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAuthImplementation.Client.Constants;
using OAuthImplementation.Client.Extensions;
using OAuthImplementation.Client.Responses;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OAuthImplementation.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClientSettings _clientSettings;
        private readonly AuthServerSettings _authServerSettings;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILogger<HomeController> logger, 
            IOptions<ClientSettings> clientOptions,
            IOptions<AuthServerSettings> authServerOptions)
        {
            _clientSettings = clientOptions.Value;
            _authServerSettings = authServerOptions.Value;
            _logger = logger;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            ViewBag.Client = _clientSettings;
            ViewBag.AuthServer = _authServerSettings;

            ViewBag.AccessToken = User.FindFirstValue(OAuthConstants.AccessToken);

            return View();
        }

        [HttpGet("/authorize")]
        public IActionResult Authorize()
        {
            _logger.LogInformation("Started authorize endpoint [GET].");

            // we add a state parameter to prevent the attacker
            // bruting force the authorization code
            var randomState = "12345";

            var redirectUri = _authServerSettings.AuthorizationEndpoint.AddParamsToUrl(
                new Dictionary<string, string>()
                {
                    { OAuthConstants.ClientId, _clientSettings.ClientId },
                    { 
                        OAuthConstants.RedirectUri, 
                        _clientSettings.RedirectUris[0].AddParamsToUrl(new()
                        {
                            { OAuthConstants.State, randomState }
                        }) 
                    },
                    { OAuthConstants.ResponseType, "code" },
                    { OAuthConstants.Scope, "openid profile" }
                });

            _logger.LogInformation("Redirect Uri: {RedirectUri}", redirectUri);

            return Redirect(redirectUri);
        }

        [HttpGet("/callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(state) || !state.Equals("12345"))
            {
                return BadRequest();
            }

            var client = new HttpClient();

            // we need to use the same Redirect URI as one that
            // we have used in the Authorize request in order to prevent CSRF attacks
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { OAuthConstants.Code, code },
                { OAuthConstants.GrantType, OAuthConstants.AuthorizationCode },
                { OAuthConstants.RedirectUri, _clientSettings.RedirectUris[0] }
            });

            var request = new HttpRequestMessage(HttpMethod.Post, _authServerSettings.TokenEndpoint);
            request.Content = content;

            // set up Headers
            // use HTTP Basic Authentication
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"{_clientSettings.ClientId}:{_clientSettings.ClientSecret}"))
            );


            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonSerializer.Deserialize<TokenEndpointResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new TokenEndpointResponse();

                var claims = new List<Claim>
                {
                    new Claim(OAuthConstants.AccessToken, tokenResponse.AccessToken)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);


                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }
    }
}
