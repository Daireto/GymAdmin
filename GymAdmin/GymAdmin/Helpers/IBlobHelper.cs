namespace GymAdmin.Helpers
{
    public interface IBlobHelper
    {
        Task DeleteBlobAsync(Guid id, string containerName);
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);
        Task<Guid> UploadBlobAsync(string image, string containerName);
        Task DeleteBlobsAsync(string containerName);
    }
}
