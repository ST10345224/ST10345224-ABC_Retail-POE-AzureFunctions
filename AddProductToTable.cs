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
    public class AddProductToTable
    {
        private readonly TableClient _tableClient;

        public AddProductToTable()
        {
            string connectionString = 
                "DefaultEndpointsProtocol=https;AccountName=st10345224storage;AccountKey=zdeyBrr0K7fic1JKgKCrGi5fVkuSS4I2FWFlrvyBACf0WP7qP72A2+tWncTowQ7Hzf3XIRIno1dd+AStrD9UVg==;EndpointSuffix=core.windows.net";

            var serviceClient = new TableServiceClient(connectionString);

            _tableClient = serviceClient.GetTableClient("Products");

            _tableClient.CreateIfNotExists();
        }

        [FunctionName("AddProductToTable")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Adding product to Azure Table...");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Products>(requestBody);

            if (data == null)
            {
                return new BadRequestObjectResult("Invalid product data.");
            }
                        
            await _tableClient.AddEntityAsync(data);

            return new OkObjectResult("Product added successfully.");
        }
    }

    public class Products : ITableEntity
    {
        [Key]
        public string? ProductId { get; set; } // Primary Key
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }

        // Required for Azure Table Storage
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
