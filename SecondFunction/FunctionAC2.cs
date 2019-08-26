using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace SecondFunction
{
    public static class FunctionAC2
    {
        [FunctionName("FunctionAC2")]
        public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(Environment.GetEnvironmentVariable("ConnectionString"))
                       .Use(KeyFilter.Any, LabelFilter.Null)
                       .Use(KeyFilter.Any, "Development");
            });

                var config = builder.Build();
            DateTime now = DateTime.Now;
            string headline = "Momentan ist es: " + now.ToString("H:mm:ss") + " Uhr";
            string message = config["TestApp:Settings:Message"];
            string mail = "Aktuelle Adresse für Supportanfragen: " + config["TestApp:Settings:Mail"];
            message = headline + "\n" + message + "\n" + mail;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            message = message ?? data?.message;

            return message != null
                ? (ActionResult)new OkObjectResult(message)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
