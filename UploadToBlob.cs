using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;

namespace ABC_Retail_POE_Functions
{
    public class UploadToBlob
    {
        // Define the blob client
        private readonly BlobServiceClient _blobServiceClient;

        public UploadToBlob()
        {
            // Get the azure storage connection string from the environment variable
            string connectionString = 
                "DefaultEndpointsProtocol=https;AccountName=st10345224storage;AccountKey=zdeyBrr0K7fic1JKgKCrGi5fVkuSS4I2FWFlrvyBACf0WP7qP72A2+tWncTowQ7Hzf3XIRIno1dd+AStrD9UVg==;EndpointSuffix=core.windows.net";

            // Initialize the blob client
            _blobServiceClient = new BlobServiceClient(connectionString);
        }


        [FunctionName("UploadToBlob")]
        public async Task<string> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, ILogger log)
        {
            // Get a logger instance
            log.LogInformation("Uploading to blob storage...");

            // Get a blob container client
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient("products");

            // Create a blob container if to does not exist
            await blobContainerClient.CreateIfNotExistsAsync();

            // Get a blob client for the specific blob
            using var stream = req.Body;

            string filename = stream.GetHashCode().ToString();
            var blobClient = blobContainerClient.GetBlobClient(filename);

            // Upload the file to the blob storage
            try
            {
                await blobClient.UploadAsync(stream, overwrite: true);

            }
            catch (Exception ex)
            {
                log.LogError("Error uploading file to blob storage: {0}", ex.Message);
                throw; // Re-throw the exception or handle it appropriately
            }

            // Return the blob URI
            return blobClient.Uri.ToString();

        }
    }
}
