using Amazon.S3.Model;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IManageFilesRepository
    {
        Task<bool> UploadFileAsync(IFormFile file, string bucketName, string key);
        Task<GetObjectResponse> GetFileByKeyAsync(string bucketName, string key);
        Task<bool> DeleteFileByKeyAsync(string bucketName, string key);
    }
}
