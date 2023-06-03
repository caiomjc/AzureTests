using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using BlobStorage.Models;

namespace BlobStorage.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<bool> DeleteBlob(string blobName, string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            return await blobClient.DeleteIfExistsAsync();

        }

        public Task<string> GetBlob(string blobName, string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            return Task.FromResult(blobClient.Uri.AbsoluteUri);
        }

        public async Task<List<string>> GetBlobs(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobList = blobContainerClient.GetBlobsAsync();

            List<string> blobs = new();

            await foreach (var blob in blobList)
            {
                blobs.Add(blob.Name);
            }

            return blobs;
        }

        public async Task<List<Blob>> GetBlobsWithUri(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobList = blobContainerClient.GetBlobsAsync();
            string sasContainerSignature = string.Empty;

            if (blobContainerClient.CanGenerateSasUri)
            {
                BlobSasBuilder blobSasBuilder = new()
                {
                    BlobContainerName = containerName,
                    Resource = "c", //container
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };

                blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

                sasContainerSignature = blobContainerClient.GenerateSasUri(blobSasBuilder).AbsoluteUri.Split('?')[1].ToString();
            }

            List<Blob> blobs = new();

            await foreach (var blob in blobList)
            {
                var blobClient = blobContainerClient.GetBlobClient(blob.Name);

                Blob blobRetrieved = new()
                {
                    Uri = $"{blobClient.Uri.AbsoluteUri}?{sasContainerSignature}"
                };

                BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                if (blobProperties.Metadata.ContainsKey("title"))
                    blobRetrieved.Title = blobProperties.Metadata["title"];

                if (blobProperties.Metadata.ContainsKey("comment"))
                    blobRetrieved.Comment = blobProperties.Metadata["comment"];

                blobs.Add(blobRetrieved);
            }

            return blobs;
        }

        public async Task<bool> UploadBlob(string blobName, IFormFile file, string containerName, Blob blob)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            IDictionary<string, string> metadata = new Dictionary<string, string>
            {
                { "title", blob.Title },
                { "comment", blob.Comment }
            };

            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders, metadata);

            return result != null;
        }
    }
}
