using BlobStorage.Models;
using BlobStorage.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorage.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string containerName)
        {
            var blobs = await _blobService.GetBlobs(containerName);
            return View(blobs);
        }

        public IActionResult AddFile(string containerName)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(string containerName, Blob blob, IFormFile file)
        {
            if (file == null || file.Length < 1) return View();

            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var result = await _blobService.UploadBlob(fileName, file, containerName, blob);

            if (result)
                return RedirectToAction("Manage", "Blob", new { containerName });
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile(string blobName, string containerName)
        {
            return Redirect(await _blobService.GetBlob(blobName, containerName));
        }

        public async Task<IActionResult> DeleteFile(string blobName, string containerName)
        {
            await _blobService.DeleteBlob(blobName, containerName);
            return RedirectToAction("Manage", "Blob", new { containerName });
        }
    }
}
