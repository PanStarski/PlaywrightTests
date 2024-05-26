using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Text.Json;
using PlaywrightTests.Models;
using System.Reflection.PortableExecutable;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace PlaywrightTests.Tests;

[TestFixture]
public class PartialUpdateBookingTests
{
    IPlaywright playwright;

    private IAPIRequestContext Request;

    private string _token;

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

    BookingRequestBase updatedBooking = new BookingRequestBase
    {
        Firstname = "Jane",
        Lastname = "Poe",
    };

    [SetUp]
    public async Task GenerateToken()
    {
        var data = new Dictionary<string, object>()
            {
                {"username", "admin"},
                {"password", "password123" }
            };
        var tokenResponse = await Request.PostAsync("auth", new() { DataObject = data });

        var jsonTokenResponse = await tokenResponse.JsonAsync<TokenResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        _token = jsonTokenResponse.Token;
    }

    [Test]
    public async Task PartialUpdateBookingChangesTheExistentEntryContent()
    {

        var responseb = await Request.PostAsync("booking", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
            DataObject = newBooking
        });

        Assert.That(responseb.Status, Is.EqualTo(200), "Expected status code: 200");

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
                    { "Cookie", $"token={_token}" }
                },
            DataObject = updatedBooking
        });
        Assert.That(responsec.Status, Is.EqualTo(200), "Expected status code: 200");

        var responsed = await Request.GetAsync($"booking/{bookingID}");
        var jsonResponsed = await responsed.JsonAsync<BookingRequest>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(responsed.Ok);
        Assert.That(responsed.Status, Is.EqualTo(200), "Expected status code: 200");

        Assert.That(updatedBooking.Firstname, Is.EqualTo(jsonResponsed.Firstname), "Expected Lastname to be updated");
        Assert.That(updatedBooking.Lastname, Is.EqualTo(jsonResponsed.Lastname), "Expected Lastname to be updated");

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
        Assert.That(response.Status, Is.EqualTo(403), "Expected status code: 403");
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