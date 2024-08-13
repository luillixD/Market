using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Market.Services.Interfaces;
using Microsoft.Extensions.Options;


namespace Market.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly ILogger<S3Service> _logger;

        public S3Service(IOptions<AWSOptions> awsOptions, ILogger<S3Service> logger)
        {
            var options = awsOptions.Value;
            if (options == null)
            {
                throw new ArgumentNullException(nameof(awsOptions), "AWS options are not configured properly.");
            }

            _s3Client = new AmazonS3Client(options.AccessKey, options.SecretKey, RegionEndpoint.GetBySystemName(options.Region));
            _bucketName = options.BucketName;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            try
            {
                var fileName = file.FileName;
                using var newMemoryStream = new MemoryStream();
                await file.CopyToAsync(newMemoryStream);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = fileName,
                    BucketName = _bucketName,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);

                _logger.LogInformation($"File uploaded successfully to {_bucketName}/{fileName}");

                return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError($"AmazonS3Exception: {ex.Message}");
                throw new Exception($"Error encountered on server. Message:'{ex.Message}' when writing an object");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                throw new Exception($"Unknown error encountered on server. Message:'{ex.Message}'");
            }
        }

    }
}
