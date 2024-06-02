using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

namespace PlaywrightTests.Tests;

[TestFixture]
public class HealthCheckTests
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
    public async Task HealthCheckReturns201Status()
    {
        var response = await Request.GetAsync("ping");
        Assert.That(response.Status, Is.EqualTo(201), "Expected status code: 201");

        var responseBody = await response.TextAsync();
        Console.WriteLine(responseBody);
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
