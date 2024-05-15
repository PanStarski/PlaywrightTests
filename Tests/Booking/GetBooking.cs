using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace PlaywrightTests.Tests;

[TestFixture]
public class GetBookingTest
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
    public async Task GetBookingReturnsListOfBookings()
    {
        var response = await Request.GetAsync("booking/1");
        var jsonResponse = await response.JsonAsync<Models.BookingRequest>(new System.Text.Json.JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(response.Ok);
        Assert.AreEqual(200, response.Status, "Expected status code: 200");

        Assert.IsNotEmpty(jsonResponse.Firstname, "The Firstname should not be empty.");
        Assert.IsNotEmpty(jsonResponse.Lastname, "The Lastname should not be empty.");

        Console.WriteLine(jsonResponse.Firstname);

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
