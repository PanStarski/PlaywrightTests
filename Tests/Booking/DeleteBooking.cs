using Microsoft.Playwright;
using System.Text.Json;
using PlaywrightTests.Models;

namespace PlaywrightTests.Tests;

[TestFixture]
public class DeleteBookingTests
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
    public async Task DeleteBookingRemovesExistingEntry()
    {

        var addBookingResponse = await Request.PostAsync("booking", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
            DataObject = newBooking
        });

        Assert.That(addBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200");

        var jsonAddBookingResponse = await addBookingResponse.JsonAsync<BookingResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var bookingID = jsonAddBookingResponse.Id;


        var deleteBookingResponse = await Request.DeleteAsync($"booking/{bookingID}", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Cookie", $"token={_token}" }
                }
        });
        Assert.That(deleteBookingResponse.Status, Is.EqualTo(201), "Expected status code: 201");

        var getBookingResponse = await Request.GetAsync($"booking/{bookingID}");
        Assert.That(getBookingResponse.Status, Is.EqualTo(404), "Expected status code: 404");

    }
    [Test]
    public async Task DeleteBookingReturns403StatusWithoutAuthentication()
    {
        var response = await Request.DeleteAsync($"booking/1");
        Assert.That(response.Status, Is.EqualTo(403), "Expected status code: 403");
        var responseBody = await response.TextAsync();
        Console.WriteLine(responseBody);
    }

    private async Task CreateAPIRequestContext()
    {
        var headers = new Dictionary<string, string>
        {
            {"Content-Type", "application/json" }
        };

        Request = await playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "https://restful-booker.herokuapp.com/",
            ExtraHTTPHeaders = headers,
            IgnoreHTTPSErrors = true
        });
    }

    }