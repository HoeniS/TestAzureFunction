using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration;

namespace AzureFunctionsTest
{
    public static class FunctionwithoutAC
    {
        [FunctionName("FunctionwithoutAC")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Queue("outqueue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> msg, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //var config = new ConfigurationBuilder()
            //        .SetBasePath(context.FunctionAppDirectory)
            //        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            //        .AddEnvironmentVariables()
            //        .Build();

            //string setting = ConfigurationManager.AppSettings["url"];
            DateTime now = DateTime.Now;
            string headline = "Momentan ist es: " + now.ToString("H:mm:ss") +  " Uhr";
            string message = Environment.GetEnvironmentVariable("TestApp:Settings:Message");
            string mail = "Aktuelle Adresse für Supportanfragen: " + Environment.GetEnvironmentVariable("TestApp:Settings:Mail");
            message = headline + "\n" + message + "\n" + mail;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            message = message ?? data?.message;

            return message != null
                ? (ActionResult)new OkObjectResult(message)
                : new BadRequestObjectResult("Please pass a message from a configuration store, on the query string or in the request body");
        }
    }
}
