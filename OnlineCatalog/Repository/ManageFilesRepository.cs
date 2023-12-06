using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class ManageFilesRepository : IManageFilesRepository
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;

        public ManageFilesRepository(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _configuration = configuration;
        }

        public async Task<bool> UploadFileAsync(IFormFile file, string bucketName, string key)
        {
            var accessKeyId = _configuration["AWS:AccessKeyId"];
            var secretAccessKey = _configuration["AWS:SecretAccessKey"];
            var region = _configuration["AWS:Region"];
            var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
            var s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(region));
            var response = await s3Client.ListBucketsAsync();
            var bucketExists = response.Buckets.Any(b => b.BucketName == bucketName);
            if (!bucketExists)
            {
                return false;
            }
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = key,
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await s3Client.PutObjectAsync(request);
            return true;
        }

        public async Task<GetObjectResponse> GetFileByKeyAsync(string bucketName, string key)
        {
            var accessKeyId = _configuration["AWS:AccessKeyId"];
            var secretAccessKey = _configuration["AWS:SecretAccessKey"];
            var region = _configuration["AWS:Region"];
            var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
            var s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(region));

            var request = new GetObjectRequest()
            {
                BucketName = bucketName,
                Key = key
            };
            var response = await s3Client.GetObjectAsync(request);
            return response;

        }

        public Task<bool> DeleteFileByKeyAsync(string bucketName, string key)
        {
            throw new NotImplementedException();
        }
    }
}
