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


            //prepare tables
            //CloudTable tableUrl = Common.CreateTable(Common.tableNameURL);
            //string partitionKey = UrlShort.Substring(0, 1);
            //TableOperation retrieveOperation = TableOperation.Retrieve<UrlEntity>(partitionKey, UrlShort);
            //TableResult result = await tableUrl.ExecuteAsync(retrieveOperation);
            //UrlEntity entity = result.Result as UrlEntity;
            try
            {
                string urlfull = await Common.InflateURLAsync(urlshort);
                return new RedirectResult(urlfull, true, true); //redirect perm & preserve
            }
            catch
            {
                return new BadRequestObjectResult("Bad link");
            }

            //if (entity==null || string.IsNullOrEmpty(entity.UrlFull) )
            //{
            //    return new BadRequestObjectResult("Bad link");
            //}
            //else
            //{
            //    return new RedirectResult(entity.UrlFull, true, true); //redirect perm & preserve
            //}
        }
    }
}
