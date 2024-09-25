using Microsoft.AspNetCore.Authentication;
using OAuthImplementation.Client.Constants;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace OAuthImplementation.Client.RequestHandlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public BearerTokenHandler(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                SetBearerToken(request, accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private void SetBearerToken(HttpRequestMessage request, string accessToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        public async Task<string?> GetAccessTokenAsync()
        {
            //return await _httpContextAccessor.HttpContext!.GetTokenAsync(OAuthConstants.AccessToken);
            return _httpContextAccessor.HttpContext!.User.FindFirstValue(OAuthConstants.AccessToken);
            //var expiresAtToken = await _httpContextAccessor.HttpContext!.GetTokenAsync("expires_at");

            //if (expiresAtToken == null)
            //{
            //    return null;
            //}

            //var expiresAtDateTimeOffset = DateTimeOffset.Parse(expiresAtToken!, CultureInfo.InvariantCulture);
            //if (expiresAtDateTimeOffset.AddSeconds(-60).ToUniversalTime() > DateTime.UtcNow)
            //{
            //    return await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
            //}

            //return null;

            //var refreshResponse = await GetRefreshResponseFromIDP();

            //if (refreshResponse.IsError)
            //{
            //    return null;
            //}

            //var updatedTokens = GetUpdatedTokens(refreshResponse);
            //var currentAuthenticateResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //currentAuthenticateResult.Properties?.StoreTokens(updatedTokens);

            //await _httpContextAccessor.HttpContext!.SignInAsync(
            //    CookieAuthenticationDefaults.AuthenticationScheme,
            //    currentAuthenticateResult.Principal!,
            //    currentAuthenticateResult.Properties
            //);

            //return refreshResponse.AccessToken;
        }

        //private async Task<TokenResponse> GetRefreshResponseFromIDP()
        //{
        //    var idpClient = httpClientFactory.CreateClient("IDPClient");
        //    var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

        //    if (metaDataResponse.IsError)
        //    {
        //        return TokenResponse.FromException<TokenResponse>(new Exception(metaDataResponse.Error));
        //    }

        //    var refreshToken = await httpContextAccessor.HttpContext!.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
        //    var refreshResponse = await idpClient.RequestRefreshTokenAsync(
        //        new RefreshTokenRequest
        //        {
        //            Address = metaDataResponse.TokenEndpoint,
        //            ClientId = "book-store-client",
        //            ClientSecret = "secret",
        //            RefreshToken = refreshToken ?? throw new NullReferenceException("Refresh token is missing."),
        //        }
        //    );

        //    return refreshResponse;
        //}

        //private List<AuthenticationToken> GetUpdatedTokens(TokenResponse refreshResponse)
        //{
        //    var updatedTokens = new List<AuthenticationToken>
        //    {
        //        new AuthenticationToken
        //        {
        //            Name = OpenIdConnectParameterNames.IdToken,
        //            Value = refreshResponse.IdentityToken ?? throw new NullReferenceException("Identity token is missing.")
        //        },
        //        new AuthenticationToken
        //        {
        //            Name = OpenIdConnectParameterNames.AccessToken,
        //            Value = refreshResponse.AccessToken ?? throw new NullReferenceException("Access token is missing.")
        //        },
        //        new AuthenticationToken
        //        {
        //            Name = OpenIdConnectParameterNames.RefreshToken,
        //            Value = refreshResponse.RefreshToken ?? throw new NullReferenceException("Refresh token is missing.")
        //        },
        //        new AuthenticationToken
        //        {
        //            Name = "expires_at",
        //            Value = (DateTime.UtcNow + TimeSpan.FromSeconds(refreshResponse.ExpiresIn)).ToString("o", CultureInfo.InvariantCulture)
        //        }

        //    };

        //    return updatedTokens;
        //}
    }
}
