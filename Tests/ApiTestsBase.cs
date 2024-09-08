using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightTests.Tests
{
    abstract class ApiTestsBase
    {
        protected BookingDriver _driver;

        [SetUp]
        public void Setup()
        {
            var requestContext = InitializeApiDriver().Result; 
            _driver = new BookingDriver(requestContext);
        }
        public async Task<IAPIRequestContext> InitializeApiDriver()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Accept", "application/json");

            var playwright = await Playwright.CreateAsync();
            return await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = "https://restful-booker.herokuapp.com/",
                ExtraHTTPHeaders = headers,
                IgnoreHTTPSErrors = true
            });
        }
    }
}
