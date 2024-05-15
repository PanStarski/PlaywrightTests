using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Text.Json;
using PlaywrightTests.Models;
using System.Reflection.PortableExecutable;

namespace PlaywrightTests.Tests;

[TestFixture]
public class DeleteBookingTests
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
    public async Task DeleteBookingRemovesExistingEntry()
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
            DataObject = newBooking
        });

        Assert.AreEqual(200, responsea.Status, "Expected status code: 200");

        var jsonResponse = await responsea.JsonAsync<BookingResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var bookingID = jsonResponse.Id;


        var responseb = await Request.DeleteAsync($"booking/{bookingID}", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
        });
        Assert.AreEqual(200, responseb.Status, "Expected status code: 200");

    }
    [Test]
    public async Task DeleteBookingReturns401StatusWithoutAuthentication()
    {
        var response = await Request.DeleteAsync($"booking/1", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
        });
        Assert.AreEqual(403, response.Status, "Expected status code: 403");
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