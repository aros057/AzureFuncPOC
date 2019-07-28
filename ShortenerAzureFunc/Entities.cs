using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFuncPOC
{
    using Microsoft.Azure.Cosmos.Table;

    public class IndexEntity : TableEntity
    {
        public string Index { get; set; }

        public IndexEntity()
        {
        }

        public IndexEntity(string Index)
        {
            PartitionKey = "0"; //no real need for Partition, 
            RowKey = "index";   //there will be a single value in the table
            this.Index = Index;
        }
    }

    public class UrlEntity : TableEntity
    {
        public string UrlShort { get; set; }
        public string UrlFull { get; set; }

        public UrlEntity()
        {
        }
        public UrlEntity(string UrlShort, string UrlFull)
        {
            PartitionKey = UrlShort.Substring(0,1); //need to have Partition, let it be first character, 
            RowKey = UrlShort;
            this.UrlShort = UrlShort;
            this.UrlFull = UrlFull;
        }
    }
}

