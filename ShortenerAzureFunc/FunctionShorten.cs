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
    public static class FunctionShorten
    {
        [FunctionName("FunctionShorten")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string UrlFull = req.Query["UrlFull"];
            if (string.IsNullOrEmpty(UrlFull))
            {
                return new BadRequestObjectResult("No input full URL");
            }

            try
            {
                //prepare tables
                CloudTable tableUrl = Common.CreateTable(Common.tableNameURL);

                //index
                CloudTable tableIndex = Common.CreateTable(Common.tableNameIndex);

                IndexEntity IERetrieved = await Common.RetrieveEntityUsingPointQueryAsync(tableIndex, "0", "index");
                ulong index;
                if (IERetrieved == null)
                {
                    //not yet initialised
                    index = 0;
                }
                else
                {
                    // use stored value
                    index = Convert.ToUInt64(IERetrieved.Index);
                }
                

                string UrlShort = Common.uLongToBase62(index);
                index++;
                //now save index to the table. Ensure this does not fail, same index can't possibly be used again in case of an exception
                IndexEntity IE = new IndexEntity(index.ToString());
                var IndexRes = await Common.InsertOrMergeEntityAsync(tableIndex, IE);

                //insert URL pair to the DB
                UrlEntity UrlMap = new UrlEntity(UrlShort, UrlFull);
                UrlMap = (UrlEntity)await Common.InsertOrMergeEntityAsync(tableUrl, UrlMap);

                string result = req.Host.Value + "/api/FunctionRedirect?UrlShort=" + UrlShort;
                return (ActionResult)new OkObjectResult(result);
            }
            catch (Exception e)
            {
                //todo
            }

            //this point only will be reached if table operations did not succedd
            return (ActionResult)new OkObjectResult($"error occured bad you go");
        }





    }


   
}
