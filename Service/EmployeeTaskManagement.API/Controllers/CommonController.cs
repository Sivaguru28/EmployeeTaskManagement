using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly IAzureBlobService _blobStorageService;

        public CommonController(IAzureBlobService azureBlobService)
        {
            _blobStorageService = azureBlobService;
        }

        [HttpGet("login-image")]
        public async Task<IActionResult> GetLoginImage()
        {
            var stream = await _blobStorageService.DownloadAsync("login-background.jpg");

            return File(stream, "image/jpeg");
        }

        [HttpGet("task-image")]
        public async Task<IActionResult> GetImage()
        {
            var stream = await _blobStorageService.DownloadAsync("task-mgt.jpg");

            return File(stream, "image/jpeg");
        }
    }
}
