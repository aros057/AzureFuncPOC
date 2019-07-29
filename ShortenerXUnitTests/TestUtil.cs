using System;
using Microsoft.Extensions.Configuration;


namespace ShortenerXUnitTests
{
    public static class TestUtil
    {
        public static IConfigurationRoot Config { get; set; }

        public static void SetConfiguration()
        {
            var builder = new ConfigurationBuilder()                
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Config = builder.Build();

            var val = Config.GetSection("ConnectionStrings:CosmosConnectionString");
            Environment.SetEnvironmentVariable(val.Path, val.Value);
        }
    }    
}
