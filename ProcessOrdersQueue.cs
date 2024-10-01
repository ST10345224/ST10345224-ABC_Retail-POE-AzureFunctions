using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Storage.Queues;

namespace ABC_Retail_POE_Functions
{
    public class ProcessOrdersQueue
    {
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=st10345224storage;AccountKey=zdeyBrr0K7fic1JKgKCrGi5fVkuSS4I2FWFlrvyBACf0WP7qP72A2+tWncTowQ7Hzf3XIRIno1dd+AStrD9UVg==;EndpointSuffix=core.windows.net";

        [FunctionName("ProcessOrdersQueue")]
        public void Run([QueueTrigger("orders", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log,

            [Queue("processedorders", Connection = "AzureWebJobsStorage")] ICollector<string> processedQueue)
        {
            try
            {
                log.LogInformation($"Queue trigger function processed: {myQueueItem}");

                string processedMessage = $"PROCESSED ORDER: [{myQueueItem}]";

                log.LogInformation("Adding processed order to the processed orders queue...");

                processedQueue.Add(processedMessage);

                // send the message to the queue

            }
            catch (Exception ex) {
                log.LogError($"Error processing order: {ex.Message}");
            }
        }
    }
}
