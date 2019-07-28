using System;

namespace AzureFuncPOC
{
  
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Azure.Documents;

    public static class Common
    {
                                               
        public const string tableNameURL = "urlstore";
        public const string tableNameIndex = "indexstore";

        public static CloudTable CreateTable(string tableName)
        {
            string connstr = Environment.GetEnvironmentVariable("ConnectionStrings:CosmosConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connstr);
            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();

            return table;
        }




        public static string uLongToBase62(ulong number)
        {
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            const int Bits = 64; //bits in ulong type

            if (number == 0)
                return "0";

            uint radix = 62;

            int index = Bits - 1;
            ulong currentNumber = number;
            char[] charArray = new char[Bits];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new string(charArray, index + 1, Bits - index - 1);

            return result;
        }


        public static async Task<string> InflateURLAsync (string urlshort)
        {
            try
            {
                //prepare tables
                CloudTable tableUrl = Common.CreateTable(Common.tableNameURL);
                string partitionKey = urlshort.Substring(0, 1);
                TableOperation retrieveOperation = TableOperation.Retrieve<UrlEntity>(partitionKey, urlshort);
                TableResult result = await tableUrl.ExecuteAsync(retrieveOperation);
                UrlEntity entity = result.Result as UrlEntity;
                if (entity == null || string.IsNullOrEmpty(entity.UrlFull))
                {
                    throw new Exception("Bad link");
                }
                else
                {
                    return entity.UrlFull;
                }
            }
            catch
            {
                throw;
            }
        }


        public static async Task<TableEntity> InsertOrMergeEntityAsync(CloudTable table, TableEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                TableEntity insertedVal = result.Result as TableEntity;

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return insertedVal;
            }
            catch (StorageException e)
            {
                throw;
            }
        }


        public static async Task<IndexEntity> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<IndexEntity>(partitionKey, rowKey);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            IndexEntity entity = result.Result as IndexEntity;
            return entity;
        }


    }


}