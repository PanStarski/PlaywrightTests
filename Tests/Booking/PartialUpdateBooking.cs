using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Text.Json;
using PlaywrightTests.Models;
using System.Reflection.PortableExecutable;
using System.ComponentModel;

namespace PlaywrightTests.Tests;

[TestFixture]
public class PartialUpdateBookingTests
{
    IPlaywright playwright;

    private IAPIRequestContext Request;

    [OneTimeSetUp]
    public async Task SetupApiTesting()
    {
        playwright = await Playwright.CreateAsync();
        await CreateAPIRequestContext();
    }
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

    Models.BookingRequestBase updatedBooking = new Models.BookingRequestBase
    {
        Firstname = "Jane",
        Lastname = "Poe",
    };

    [Test]
    public async Task PartialUpdateBookingChangesTheExistentEntryContent()
    {
        var data = new Dictionary<string, object>()
            {
                {"username", "admin"},
                {"password", "password123" }
            };
        var responsea = await Request.PostAsync("auth", new() { DataObject = data });

        var jsonResponsea = await responsea.JsonAsync<TokenResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        string token = jsonResponsea.Token;


        var responseb = await Request.PostAsync("booking", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
            DataObject = newBooking
        });

        Assert.AreEqual(200, responseb.Status, "Expected status code: 200");

        var jsonResponseb = await responseb.JsonAsync<BookingResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var bookingID = jsonResponseb.Id;

        var responsec = await Request.PatchAsync($"booking/{bookingID}", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Cookie", $"token={token}" }
                },
            DataObject = updatedBooking
        });
        Assert.AreEqual(200, responsec.Status, "Expected status code: 200");

        var responsed = await Request.GetAsync($"booking/{bookingID}");
        var jsonResponsed = await responsed.JsonAsync<Models.BookingRequest>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(responsed.Ok);
        Assert.AreEqual(200, responsed.Status, "Expected status code: 200");

        Assert.AreEqual(jsonResponsed.Firstname, updatedBooking.Firstname, "Expected Lastname to be updated");
        Assert.AreEqual(jsonResponsed.Lastname, updatedBooking.Lastname, "Expected Lastname to be updated");

    }

    [Test]

    public async Task UpdatedBookingReturns403StatusWithoutAuthentication()
    {
        var response = await Request.PatchAsync($"booking/1", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                },
            DataObject = updatedBooking
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