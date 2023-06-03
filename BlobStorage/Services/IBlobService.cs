using BlobStorage.Models;

namespace BlobStorage.Services
{
    public interface IBlobService
    {
        Task<string> GetBlob(string blobName, string containerName);
        Task<List<string>> GetBlobs(string containerName);
        Task<List<Blob>> GetBlobsWithUri(string containerName);
        Task<bool> UploadBlob(string blobName, IFormFile file, string containerName, Blob blob);
        Task<bool> DeleteBlob(string blobName, string containerName);
    }
}
