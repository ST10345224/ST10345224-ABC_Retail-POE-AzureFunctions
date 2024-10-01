using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;
using System.ComponentModel.DataAnnotations;

namespace ABC_Retail_POE_Functions
{
    public class AddUserToTable
    {
        private readonly TableClient _tableClient;

        public AddUserToTable()
        {
            string connectionString = 
                "DefaultEndpointsProtocol=https;AccountName=st10345224storage;AccountKey=zdeyBrr0K7fic1JKgKCrGi5fVkuSS4I2FWFlrvyBACf0WP7qP72A2+tWncTowQ7Hzf3XIRIno1dd+AStrD9UVg==;EndpointSuffix=core.windows.net";

            var serviceClient = new TableServiceClient(connectionString);

            _tableClient = serviceClient.GetTableClient("Users");

            _tableClient.CreateIfNotExists();
        }

        [FunctionName("AddUserToTable")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Adding product to Azure Table...");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Users>(requestBody);

            if (data == null)
            {
                return new BadRequestObjectResult("Invalid user data.");
            }

            await _tableClient.AddEntityAsync(data);

            return new OkObjectResult("User added successfully.");
        }
    }

    public class Users : ITableEntity
    {
        [Key]
        public string? UserId { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }

        // Required for Azure Table Storage
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;

    }
}
