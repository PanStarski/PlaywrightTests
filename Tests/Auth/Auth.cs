using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

namespace PlaywrightTests.Tests;

    [TestFixture]
    public class CreateTokenTest
    {
    IPlaywright playwright;

        private IAPIRequestContext Request;

        [OneTimeSetUp]
        public async Task SetupApiTesting()
        {
            playwright = await Playwright.CreateAsync();
            await CreateAPIRequestContext();
        }

        [Test]
        public async Task AuthSuccessPath()
        {
        var data = new Dictionary<string, object>()
            {
                {"username", "admin"},
                {"password", "password123" }
            };
        var response = await Request.PostAsync("auth", new() { DataObject = data });
        Assert.That(response.Ok);
        Assert.AreEqual(200, response.Status, "Expected status code: 200");

        var responseBody = await response.TextAsync();
        Console.WriteLine(responseBody);

        Assert.IsNotEmpty(responseBody, "Response body should not be empty");
        }
    [Test]
    public async Task AuthWithWrongCredentials()
    {
        var data = new Dictionary<string, object>()
            {
                {"username", "admin"},
                {"password", "password12345" } // wrong password
            };
        var response = await Request.PostAsync("auth", new() { DataObject = data });
        Assert.That(response.Ok);
        Assert.AreEqual(200, response.Status, "Expected status code: 200");

        var responseBody = await response.TextAsync();
        Console.WriteLine(responseBody);

        Assert.IsNotEmpty(responseBody, "Response body should not be empty");
    }
        private async Task CreateAPIRequestContext()
    {
        var headers = new Dictionary<string, string>();
        headers.Add("Accept", "application/json");

        Request = await playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "https://restful-booker.herokuapp.com/",
            ExtraHTTPHeaders = headers,
            IgnoreHTTPSErrors = true
        });
    }
    }
