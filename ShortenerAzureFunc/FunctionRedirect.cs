using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AzureFuncPOC
{
    public static class FunctionRedirect
    {
        [FunctionName("FunctionRedirect")]
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
                string urlfull = await Common.InflateURLAsync(urlshort);
                return new RedirectResult(urlfull, true); //redirect permanently
            }
            catch
            {
                return new BadRequestObjectResult("Bad link");
            }
        }
    }
}
