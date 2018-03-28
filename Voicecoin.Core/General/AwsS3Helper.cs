using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Voicecoin.Core
{
    public class AwsS3Helper
    {
        private IConfiguration config;

        public AwsS3Helper(IConfiguration config)
        {
            this.config = config;
        }

        public AmazonS3Client GetAwsS3Client()
        {
            string awsSecretAccessKey = config.GetSection("AWS:AWSSecretKey").Value;
            string awsAccessKeyId = config.GetSection("AWS:AWSAccessKey").Value;
            Amazon.RegionEndpoint REGION = Amazon.RegionEndpoint.USEast1;

            return new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, REGION);
        }

        private string GetS3BucketName()
        {
            return config.GetSection("AWS:AWSBucketPrefix").Value;
        }

        public async Task<string> GetFileFromS3(string path)
        {
            AmazonS3Client client = GetAwsS3Client();
            var buckets = client.ListBucketsAsync();

            string bucketName = GetS3BucketName();
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = path
            };

            using (GetObjectResponse response = await client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            {
                string tempPath = AppDomain.CurrentDomain.GetData("DataDirectory") + @"\Temp\" + response.Key;
                await response.WriteResponseStreamToFileAsync(tempPath, false, new System.Threading.CancellationToken());
                //data = new byte[response.ContentLength];
                //responseStream.Read(data, 0, data.Length);
                return tempPath;
            }
        }

        public async Task<bool> ValidateFileSizeFromS3(string subDirectoryInBucket, string path, long size)
        {
            AmazonS3Client client = GetAwsS3Client();
            var buckets = client.ListBucketsAsync();

            string bucketName = GetS3BucketName();
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = path
            };

            if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
            {
                request.BucketName = bucketName; //no subdirectory just bucket name
            }
            else
            {   // subdirectory and bucket name
                request.BucketName = bucketName + @"/" + subDirectoryInBucket;
            }

            using (GetObjectResponse response = await client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            {
                return response.ContentLength == size;
            }
        }

        public bool SaveFileToS3(string localFilePath, string subDirectoryInBucket, string fileNameInS3)
        {
            // input explained :
            // localFilePath = the full local file path e.g. "c:\mydir\mysubdir\myfilename.zip"
            // bucketName : the name of the bucket in S3 ,the bucket should be alreadt created
            // subDirectoryInBucket : if this string is not empty the file will be uploaded to
            // a subdirectory with this name
            // fileNameInS3 = the file name in the S3

            AmazonS3Client client = GetAwsS3Client();
            string bucketName = GetS3BucketName();
            // create a TransferUtility instance passing it the IAmazonS3 created in the first step
            TransferUtility utility = new TransferUtility(client);
            // making a TransferUtilityUploadRequest instance
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

            if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
            {
                request.BucketName = bucketName; //no subdirectory just bucket name
            }
            else
            {   // subdirectory and bucket name
                request.BucketName = bucketName + @"/" + subDirectoryInBucket;
            }
            request.Key = fileNameInS3; //file name up in S3
            request.FilePath = localFilePath; //local file name
            utility.Upload(request); //commensing the transfer

            return true; //indicate that the file was sent
        }

        public string GeneratePreSignedURLFromS3(string subDirectoryInBucket, string objectKey)
        {
            AmazonS3Client client = GetAwsS3Client();
            string bucketName = GetS3BucketName();

            string urlString = "";
            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Expires = DateTime.Now.AddDays(1)
            };

            if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
            {
                request.BucketName = bucketName; //no subdirectory just bucket name
            }
            else
            {   // subdirectory and bucket name
                request.BucketName = bucketName + @"/" + subDirectoryInBucket;
            }

            try
            {
                urlString = client.GetPreSignedURL(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine(
                    "To sign up for service, go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                     "Error occurred. Message:'{0}' when listing objects",
                     amazonS3Exception.Message);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return urlString;
        }
    }
}
