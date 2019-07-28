using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;

namespace AzureFuncPOC
{
    public static class FunctionInflate
    {
        [FunctionName("FunctionInflate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string urlshort = req.Query["UrlShort"];
            if (string.IsNullOrEmpty(urlshort))
            {
                return new BadRequestObjectResult("No input short URL");
            }

            try
            {
                //url short has form something like this: 
                //aros057.azurewebsites.net/api/FunctionRedirect?UrlShort=Y
                //the last bit is the token to look in the DB. 
                urlshort = urlshort.Substring(urlshort.LastIndexOf("=")+1);
                string urlfull = await Common.InflateURLAsync(urlshort);
                return (ActionResult)new OkObjectResult(urlfull);
            }
            catch
            {
                return new BadRequestObjectResult("Bad link");
            }
        }
    }
}
