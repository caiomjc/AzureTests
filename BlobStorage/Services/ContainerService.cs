using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BlobStorage.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public ContainerService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task CreateContainer(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

        }

        public async Task DeleteContainer(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetContainerAndBlobs()
        {
            List<string> containersAndBlobs = new()
            {
                $"Account Name : {_blobServiceClient.AccountName}",
                "------------------------------------------------------------------------------------------------------------"
            };

            await foreach (var blobContainerItem in _blobServiceClient.GetBlobContainersAsync())
            {
                containersAndBlobs.Add($"-- {blobContainerItem.Name}");
                var blobContainer = _blobServiceClient.GetBlobContainerClient(blobContainerItem.Name);

                await foreach (BlobItem blobItem in blobContainer.GetBlobsAsync())
                {
                    //get metadata
                    var blobClient = blobContainer.GetBlobClient(blobItem.Name);
                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                    string blob = blobItem.Name;

                    if (blobProperties.Metadata.ContainsKey("title"))
                        blob += $" ({blobProperties.Metadata["title"]})";

                    containersAndBlobs.Add($"------ {blob}");
                }

                containersAndBlobs.Add("------------------------------------------------------------------------------------------------------------");
            }

            return containersAndBlobs;
        }

        public async Task<List<string>> GetContainers()
        {
            List<string> containerNames = new();

            await foreach (var blobContainerItem in _blobServiceClient.GetBlobContainersAsync())
            {
                containerNames.Add(blobContainerItem.Name);
            }

            return containerNames;
        }
    }
}
