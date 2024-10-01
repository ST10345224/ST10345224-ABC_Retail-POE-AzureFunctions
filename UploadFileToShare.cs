using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Files.Shares;
using Azure;

namespace ABC_Retail_POE_Functions
{
    public class UploadFileToShare
    {
        private readonly ShareClient _shareClient;

        public UploadFileToShare()
        {
            string connectionString = 
                "DefaultEndpointsProtocol=https;AccountName=st10345224storage;AccountKey=zdeyBrr0K7fic1JKgKCrGi5fVkuSS4I2FWFlrvyBACf0WP7qP72A2+tWncTowQ7Hzf3XIRIno1dd+AStrD9UVg==;EndpointSuffix=core.windows.net";

            _shareClient = new ShareClient(connectionString, "productshare");

            _shareClient.CreateIfNotExists();
        }


        [FunctionName("UploadFileToShare")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Uploading file to Azure Files...");

            // Headers contain the directory name, and the file name, while the body contains the file content Stream object

            // To get the directory name:
            string directoryName = req.Headers["directoryName"];

            // To get the file name:
            string fileName = req.Headers["fileName"];

            // Get a directory client
            var directoryClient = _shareClient.GetDirectoryClient(directoryName);

            await directoryClient.CreateIfNotExistsAsync();

            // Get a file client
            var fileClient = directoryClient.GetFileClient(fileName);

            // Get the file content
            using (var stream = req.Body)
            {
                await fileClient.CreateAsync(stream.Length);

                await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
            }

            var responseMessage = $"File uploaded successfully to {directoryName}/{fileName}";

            log.LogInformation(responseMessage);

        }
    }
}
