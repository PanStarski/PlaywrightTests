using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Text.Json;
using PlaywrightTests.Models;

namespace PlaywrightTests.Tests;

[TestFixture]
public class CreateBookingTests
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
    public async Task CreateBookingAddsNewEntry()
    {
        Models.BookingRequest newBooking = new Models.BookingRequest
        {
            Firstname = "John",
            Lastname = "Doe",
            Totalprice = 123,
            IsPaid = true,
            BookingDates = new Models.BookingDates
            {
                Checkin = new DateTime(2024, 5, 14),
                Checkout = new DateTime(2024, 5, 15)
            },
            Additionalneeds = "Breakfast"
        };

        var responsea = await Request.PostAsync("booking", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
            DataObject =  newBooking
        });

        Assert.AreEqual(200, responsea.Status, "Expected status code: 200");

        var jsonResponse = await responsea.JsonAsync<BookingResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var bookingID = jsonResponse.Id;


        var responseb = await Request.GetAsync($"booking/{bookingID}");

        Assert.AreEqual(200, responseb.Status, "Expected status code: 200");
        var responseBody = await responseb.TextAsync();
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