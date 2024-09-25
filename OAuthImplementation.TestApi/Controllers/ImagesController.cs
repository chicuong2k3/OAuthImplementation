using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuthImplementation.TestApi.Services;

namespace OAuthImplementation.TestApi.Controllers
{
    [ApiController]
    [Route("/images")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(id);
        }

        [HttpPost]
        public IActionResult Upload()
        {

            return Ok();
        }
    }
}
