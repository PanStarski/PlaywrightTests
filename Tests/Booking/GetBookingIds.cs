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
        var getBookingResponse = await Request.GetAsync("booking");
        var bookings = await getBookingResponse.JsonAsync<List<Models.BookingResponse>>();
        Assert.That(getBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200"); 
        Assert.That(bookings.Count, Is.GreaterThan(0));
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
