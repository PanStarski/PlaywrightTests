using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Text.Json;
using PlaywrightTests.Models;
using System.Reflection.PortableExecutable;
using log4net;

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
        var responsea = await Request.PostAsync("auth", new() { DataObject = data });

        var jsonResponsea = await responsea.JsonAsync<TokenResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        _token = jsonResponsea.Token;
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

    [Test]
    public async Task DeleteBookingRemovesExistingEntry()
    {

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


        var responsec = await Request.DeleteAsync($"booking/{bookingID}", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Cookie", $"token={_token}" }
                }
        });
        Assert.AreEqual(201, responsec.Status, "Expected status code: 201");

        var responsed = await Request.GetAsync($"booking/{bookingID}");
        Assert.AreEqual(404, responsed.Status, "Expected status code: 404");

    }
    [Test]
    public async Task DeleteBookingReturns403StatusWithoutAuthentication()
    {
        var response = await Request.DeleteAsync($"booking/1");
        Assert.AreEqual(403, response.Status, "Expected status code: 403");
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