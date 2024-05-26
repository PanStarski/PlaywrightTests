using Microsoft.Playwright;
using PlaywrightTests.Models;
using System.Text.Json;

namespace PlaywrightTests.Tests;

[TestFixture]
    public class CreateTokenTest
    {
    IPlaywright playwright;

        private IAPIRequestContext Request;
        public void GlobalSetup()
        {
        log4net.Config.XmlConfigurator.Configure();
        }
        private static readonly ILog log = LogManager.GetLogger(typeof(CreateTokenTest));

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
        log.Info("Starting a test");
        var response = await Request.PostAsync("auth", new() { DataObject = data });

        log.Debug("Assert that response status is 200 OK");
        Assert.That(response.Ok);
        Assert.That(response.Status, Is.EqualTo(200), "Expected status code: 200");

        log.Debug("Assert that response body is not empty");
        var jsonResponse = await response.JsonAsync<TokenResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(jsonResponse.Token, Is.Not.Empty);
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
        Assert.That(response.Status, Is.EqualTo(200), "Expected status code: 200");

        var jsonResponse = await response.JsonAsync<TokenResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(jsonResponse.Token, Is.Null);
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
