using AzureFuncPOC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace ShortenerXUnitTests
{

    /// <summary>
    /// These are closer to an integration tests rather than unit tests, e.g. they use real DB instead of a mock
    /// </summary>
    public class IntegrationTests
    {

        private readonly ILogger logger = TestFactory.CreateLogger();
       
        [Fact]
        public async void Http_trigger_no_input()
        {
            var request = TestFactory.CreateHttpRequest("", "");
            var response = (ObjectResult)await FunctionShorten.Run(request, logger);
            var expect = new BadRequestObjectResult("No input full URL");
            Assert.Equal(expect.Value, response.Value);
            Assert.Equal(expect.StatusCode, response.StatusCode);
        }


        [Fact]
        public async void Http_trigger_interal_server_error()
        {
            //intentionally not set the enviroment: this simulates problem with the DB
            //TestUtil.SetConfiguration(); 
            string urlfull = "www.microsoft.com";
            var request = TestFactory.CreateHttpRequest("UrlFull", urlfull);
            var response = (StatusCodeResult)await FunctionShorten.Run(request, logger);            
            Assert.Equal(500, response.StatusCode);            
        }
               

        [Fact]
        public async void Http_trigger_valid_input()
        {
            string urlfull = "www.microsoft.com";
            string urlshort = await GetShortUrlAsync(urlfull);
            Assert.True(!String.IsNullOrEmpty(urlshort));
        }


        [Fact]
        public void Http_trigger_valid_input_and_inflate()
        {
            Http_trigger_valid_input_and_inflate_url("www.microsoft.com");
            Http_trigger_valid_input_and_inflate_url("http://www.microsoft.com");
            Http_trigger_valid_input_and_inflate_url("https://www.microsoft.com");
            Http_trigger_valid_input_and_inflate_url("https://www.bing.com/search?q=windows+spotlight+quiz&filters=IsConversation:%22True%22+BTWLKey:%22SaksunVillageFaroeIslands%22+BTWLType:%22Quiz%22&FORM=MLQZ01");
        }

        private async void Http_trigger_valid_input_and_inflate_url(string urlfull)
        {
            string urlshort = await GetShortUrlAsync(urlfull);
            Assert.True(!String.IsNullOrEmpty(urlshort));

            //try to inflate to original URL and compare
            var requestinflate = TestFactory.CreateHttpRequest("UrlShort", urlshort);
            var responseinflat = (ObjectResult)await FunctionInflate.Run(requestinflate, logger);
            var expectinflat = new OkObjectResult("");
            Assert.Equal(expectinflat.StatusCode, responseinflat.StatusCode);
            string urlfullinflated = responseinflat.Value.ToString();
            Assert.Equal(urlfull, urlfullinflated);
        }
         

        [Fact]
        public async void Http_trigger_redirect()
        {
            string urlfull = "https://docs.microsoft.com/en-us/azure/azure-functions/";
            string urlshort = await GetShortUrlAsync(urlfull);
            Assert.True(!String.IsNullOrEmpty(urlshort));

            //try to inflate to original URL and compare
            var requestredirect = TestFactory.CreateHttpRequest("UrlShort", urlshort);
            var responseredirect = (RedirectResult)await FunctionRedirect.Run(requestredirect, logger);           
            Assert.Equal(urlfull, responseredirect.Url);
        }



        //common code
        private async System.Threading.Tasks.Task<string> GetShortUrlAsync(string urlfull)
        {
            TestUtil.SetConfiguration();
            var request = TestFactory.CreateHttpRequest("UrlFull", urlfull);
            var response = (ObjectResult)await FunctionShorten.Run(request, logger);
            var expect = new OkObjectResult("");
            Assert.Equal(expect.StatusCode, response.StatusCode);
            string shorturl = response.Value.ToString();

            return shorturl;
        }

    }
}
