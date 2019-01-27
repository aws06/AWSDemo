using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace AWS
{
    public class AWSS3Storage
    {
        private static readonly string FilePathSecrets = @"C:\ServiceLogs\AWS06\AWS06Secretes.txt";
        private const string UploadFilePath = @"C:\ServiceLogs\AWS06\RawImages\AWSLogo.png";
        private const string _BucketName = "aws06demo";
        // Example creates two objects (for simplicity, we upload same file twice).
        // You specify key names for these objects.
        private const string key01 = "AWSLogo-Key01";
        private const string key02 = "AWSLogo-Key02";
        private const string key03 = "AWSLogo-Key03";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private static readonly string _AccessKeyId = string.Empty;
        private static readonly string _SecretAccessKey = string.Empty;
        private static IAmazonS3 s3Client;

        static AWSS3Storage()
        {
            string Secrets = ReadServerSecrets();
            Secrets Keys = DeserializeJSONData<Secrets>(Secrets);
            _AccessKeyId = Keys.AWS06Secretes.users.accesskeyid;
            _SecretAccessKey = Keys.AWS06Secretes.users.secretaccesskey;
        }

        public void StartAmazonMain()
        {
            try
            {
                s3Client = new AmazonS3Client(_AccessKeyId, _SecretAccessKey, bucketRegion);
                var fileTransferUtility = new TransferUtility(s3Client);
                UploadFileWithFileNameAsync(fileTransferUtility).Wait();
                UploadObjectKeyNameValueExplicitlyAsync(fileTransferUtility).Wait();
                UploadDataFromIOStreamAsync(fileTransferUtility).Wait();
                UploadDataWithAdvancedSetting(fileTransferUtility).Wait();
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        /// <summary>
        /// Option 1. Upload a file. The file name is used as the object key name.
        /// </summary>
        /// <returns></returns>
        private async Task UploadFileWithFileNameAsync(TransferUtility fileTransferUtility)
        {
            
            await fileTransferUtility.UploadAsync(UploadFilePath, _BucketName);
            Console.WriteLine("Upload 1 completed");   
        }

        /// <summary>
        /// Option 2. Specify object key name explicitly.
        /// </summary>
        /// <returns></returns>
        private async Task UploadObjectKeyNameValueExplicitlyAsync(TransferUtility fileTransferUtility)
        {
            await fileTransferUtility.UploadAsync(UploadFilePath, _BucketName, key01);
            Console.WriteLine("Upload 2 completed");
        }

        /// <summary>
        /// Option 3. Upload data from a type of System.IO.Stream.
        /// </summary>
        /// <param name="fileTransferUtility"></param>
        /// <returns></returns>
        private async Task UploadDataFromIOStreamAsync(TransferUtility fileTransferUtility)
        {
            using (var fileToUpload = new FileStream(UploadFilePath, FileMode.Open, FileAccess.Read))
            {
                await fileTransferUtility.UploadAsync(fileToUpload, _BucketName, key02);
            }
            Console.WriteLine("Upload 3 completed");
        }

        /// <summary>
        /// Option 4. Specify advanced settings.
        /// </summary>
        /// <param name="fileTransferUtility"></param>
        /// <returns></returns>
        private async Task UploadDataWithAdvancedSetting(TransferUtility fileTransferUtility)
        {
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = _BucketName,
                FilePath = UploadFilePath,
                StorageClass = S3StorageClass.StandardInfrequentAccess,
                PartSize = 6291456, // 6 MB.
                Key = key03,
                CannedACL = S3CannedACL.PublicRead
            };
            fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
            fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

            await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
            Console.WriteLine("Upload 4 completed");
        }

        private static string ReadServerSecrets()
        {
            string secrets = string.Empty;
            using (var fileStream = new FileStream(FilePathSecrets, FileMode.Open, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                secrets = streamReader.ReadToEnd();
            }

            return secrets;
        }

        /// <summary>
        /// Serialize the JSON data. Using with Default contract resolver for non public fields.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static dynamic DeserializeJSONData<T>(string message)
        {
            Newtonsoft.Json.JsonSerializerSettings jss = new Newtonsoft.Json.JsonSerializerSettings();

            Newtonsoft.Json.Serialization.DefaultContractResolver dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
            jss.ContractResolver = dcr;

            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message);

            return response;
        }
    }
}
