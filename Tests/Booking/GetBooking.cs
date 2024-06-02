using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;
using PlaywrightTests.Models;
using System.Text.Json;

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
    BookingRequest newBooking = new BookingRequest
    {
        Firstname = "John",
        Lastname = "Doe",
        Totalprice = 123,
        IsPaid = true,
        BookingDates = new BookingDates
        {
            Checkin = new DateTime(2024, 5, 14),
            Checkout = new DateTime(2024, 5, 15)
        },
        Additionalneeds = "Breakfast"
    };
    [Test]
    public async Task GetBookingReturnsTheCreatedEntry()
    {
        var createBookingResponse = await Request.PostAsync("booking", new APIRequestContextOptions
        {
            DataObject = newBooking
        });

        Assert.That(createBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200");

        var createBookingJsonResponse = await createBookingResponse.JsonAsync<BookingResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var bookingID = createBookingJsonResponse.Id;

        var getBookingResponse = await Request.GetAsync($"booking/{bookingID}");
        var getBookingJsonResponse = await getBookingResponse.JsonAsync<Models.BookingRequest>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(getBookingResponse.Ok);
        Assert.That(getBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200");

        Assert.Multiple(() =>
        {
            Assert.IsNotEmpty(getBookingJsonResponse.Firstname, "The Firstname should not be empty.");
            Assert.IsNotEmpty(getBookingJsonResponse.Lastname, "The Lastname should not be empty.");
            Assert.IsNotNull(getBookingJsonResponse.IsPaid, "The IsPaid should not be empty");
            Assert.IsNotNull(getBookingJsonResponse.Totalprice, "Totalprice should not be empty");
            Assert.IsNotEmpty(getBookingJsonResponse.Additionalneeds, "AdditionalNeeds should not be empty");
        });
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
