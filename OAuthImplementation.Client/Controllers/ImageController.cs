using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace OAuthImplementation.Client.Controllers
{
    public class ImageController : Controller
    {
        private readonly ImageApiSettings _apiSettings;
        private readonly HttpClient _httpClient;
        public ImageController(
            IOptions<ImageApiSettings> apiOptions,
            IHttpClientFactory httpClientFactory)
        {
            _apiSettings = apiOptions.Value;
            _httpClient = httpClientFactory.CreateClient("ImageApi");
        }

        [HttpGet("/image/{imageId}")]
        public async Task<IActionResult> Index(Guid imageId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiSettings.BaseUri}/images/{imageId}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return View();
            }

            return BadRequest();
        }
    }
}
