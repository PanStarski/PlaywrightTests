using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

namespace PlaywrightTests.Tests;

[TestFixture]
public class GetBookingIdsTest
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
        var response = await Request.GetAsync("booking");
        var bookings = await response.JsonAsync<List<Models.BookingResponse>>();
        Assert.AreEqual(200, response.Status, "Expected status code: 200"); 
        Assert.That(bookings.Count, Is.GreaterThan(0));

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
