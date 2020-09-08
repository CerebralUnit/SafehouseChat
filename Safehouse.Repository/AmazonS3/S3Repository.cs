using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace Safehouse.Repository.AmazonS3
{
    public class S3Repository
    {
        public async Task<string> UploadImage(IFormFile file)
        { 
            var imageBytes = await GetByteArrayFromImageAsync(file);
            AmazonS3Client client = new AmazonS3Client(RegionEndpoint.USWest2);
            var key = $"icons/{Guid.NewGuid().ToString("N")}.png";
            
            // Create a PutObject request
            var request = new PutObjectRequest
            {
                BucketName = "safehousechat",
                Key = key,
                InputStream = new MemoryStream(imageBytes),
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };
             
            PutObjectResponse response = await client.PutObjectAsync(request);
            
            return response.HttpStatusCode == HttpStatusCode.OK ? $"https://safehousechat.s3-us-west-2.amazonaws.com/{key}" : null;
        }

      
        private async Task<byte[]> GetByteArrayFromImageAsync(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                await file.CopyToAsync(target);
                return target.ToArray();
            }
        }
    }
}
