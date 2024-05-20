using Microsoft.Playwright;
using PlaywrightTests.Models;
using System.Text.Json;

namespace PlaywrightTests.Tests;

[TestFixture]
public class UpdateBookingTests
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
    BookingRequest UpdatedBooking = new BookingRequest
    {
        Firstname = "Jane",
        Lastname = "Poe",
        Totalprice = 321,
        IsPaid = false,
        BookingDates = new BookingDates
        {
            Checkin = new DateTime(2023, 5, 14),
            Checkout = new DateTime(2023, 5, 15)
        },
        Additionalneeds = "Dinner"
    };

    [Test]
    public async Task UpdateBookingChangesTheExistentEntryContent()
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

        var responsec = await Request.PutAsync($"booking/{bookingID}", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Cookie", $"token={token}" }
                },
            DataObject = UpdatedBooking
        });
        Assert.AreEqual(200, responsec.Status, "Expected status code: 200");

        var responsed = await Request.GetAsync($"booking/{bookingID}");
        var jsonResponsed = await responsed.JsonAsync<  BookingRequest>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(responsed.Ok);
        Assert.AreEqual(200, responsed.Status, "Expected status code: 200");

        Assert.AreEqual(jsonResponsed.Firstname, UpdatedBooking.Firstname, "Expected Firstname to be updated");
        Assert.AreEqual(jsonResponsed.Lastname, UpdatedBooking.Lastname, "Expected Lastname to be updated");
        Assert.AreEqual(jsonResponsed.IsPaid, UpdatedBooking.IsPaid, "Expected isPaid to be updated");
        Assert.AreEqual(jsonResponsed.Totalprice, UpdatedBooking.Totalprice, "Expected Totalprice to be updated");
        Assert.AreEqual(jsonResponsed.Additionalneeds, UpdatedBooking.Additionalneeds, "Expected Additionalneeds to be updated");
        Assert.AreEqual(jsonResponsed.BookingDates.Checkin, UpdatedBooking.BookingDates.Checkin, "Expected Checkin to be updated");
        Assert.AreEqual(jsonResponsed.BookingDates.Checkout, UpdatedBooking.BookingDates.Checkout, "Expected Checkout to be updated");

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
            DataObject = UpdatedBooking
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