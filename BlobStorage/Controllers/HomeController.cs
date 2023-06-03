using BlobStorage.Models;
using BlobStorage.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlobStorage.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContainerService _containerService;
        private readonly IBlobService _blobService;

        public HomeController(IContainerService containerService, IBlobService blobService)
        {
            _containerService = containerService;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _containerService.GetContainerAndBlobs());
        }

        public async Task<IActionResult> Images()
        {
            return View(await _blobService.GetBlobsWithUri("images"));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}