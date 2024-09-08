using Microsoft.Playwright;
using PlaywrightTests.Models;
using System.Text.Json;
using static PlaywrightTests.Logger;

namespace PlaywrightTests.Tests;

[TestFixture]
    public class AuthBookingTest
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
        log.Info("AuthSuccessPath: Starting a test");
        TokenRequest tokenRequest = new TokenRequest
        {
            Username = "admin",
            Password = "password123"
        };
        log.Info("Sending a POST Request");
        var response = await Request.PostAsync("auth", new APIRequestContextOptions { DataObject = tokenRequest });

        log.Info("Assert that response status is 200 OK");
        Assert.That(response.Ok);
        Assert.That(response.Status, Is.EqualTo(200), "Expected status code: 200");

        log.Info("Request OK. Assert that response body is not empty");
        var jsonResponse = await response.JsonAsync<TokenResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(jsonResponse.Token, Is.Not.Empty);
        log.Info("Token was generated. Test passed");
    }
        [Test]
    public async Task AuthWithWrongCredentials()
    {
        log.Info("AuthWithWrongCredentials: Starting a test");
        TokenRequest tokenRequest = new TokenRequest
        {
            Username = "admin",
            Password = "123456789" // wrong password
        };
        log.Info("Sending POST Request");
        var response = await Request.PostAsync("auth", new APIRequestContextOptions { DataObject = tokenRequest });
        Assert.That(response.Ok);
        Assert.That(response.Status, Is.EqualTo(200), "Expected status code: 200");
        log.Info("Request OK. Assert, that token wasn't generated");

        var jsonResponse = await response.JsonAsync<TokenResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(jsonResponse.Token, Is.Null);
        log.Info("Token wasn't generated. Test passed");
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
