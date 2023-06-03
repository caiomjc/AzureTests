namespace BlobStorage.Services
{
    public interface IContainerService
    {
        Task<List<string>> GetContainerAndBlobs();
        Task<List<string>> GetContainers();
        Task CreateContainer(string containerName);
        Task DeleteContainer(string containerName);
    }
}
