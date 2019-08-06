using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration;

namespace AzureFunctionsTest
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("ConnectionString"));
            var config = builder.Build();
            string message = config["TestApp:Settings:Message"];
            message = message ?? req.Query["message"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            message = message ?? data?.message;

            return message != null
                ? (ActionResult)new OkObjectResult(message)
                : new BadRequestObjectResult("Please pass a message from a configuration store, on the query string or in the request body");
        }
    }
}
