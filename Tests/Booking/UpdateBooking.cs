using Microsoft.Playwright;
using PlaywrightTests.Models;
using System.Text.Json;

namespace PlaywrightTests.Tests;

[TestFixture]
public class UpdateBookingTests
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

        var updateBookingResponse = await Request.PutAsync($"booking/{bookingID}", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                   { "Cookie", $"token={_token}" }
                },
            DataObject = UpdatedBooking
        });
        Assert.That(updateBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200");

        var getBookingResponse = await Request.GetAsync($"booking/{bookingID}");
        var getBookingJsonResponse = await getBookingResponse.JsonAsync<  BookingRequest>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(getBookingResponse.Ok);
        Assert.That(getBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200");
        Assert.Multiple(() =>
        {
            Assert.That(UpdatedBooking.Firstname, Is.EqualTo(getBookingJsonResponse.Firstname), "Expected Firstname to be updated");
            Assert.That(UpdatedBooking.Lastname, Is.EqualTo(getBookingJsonResponse.Lastname), "Expected Lastname to be updated");
            Assert.That(UpdatedBooking.IsPaid, Is.EqualTo(getBookingJsonResponse.IsPaid), "Expected isPaid to be updated");
            Assert.That(UpdatedBooking.Totalprice, Is.EqualTo(getBookingJsonResponse.Totalprice), "Expected Totalprice to be updated");
            Assert.That(UpdatedBooking.Additionalneeds, Is.EqualTo(getBookingJsonResponse.Additionalneeds), "Expected Additionalneeds to be updated");
            Assert.That(UpdatedBooking.BookingDates.Checkin, Is.EqualTo(getBookingJsonResponse.BookingDates.Checkin), "Expected Checkin to be updated");
            Assert.That(UpdatedBooking.BookingDates.Checkout, Is.EqualTo(getBookingJsonResponse.BookingDates.Checkout), "Expected Checkout to be updated");
        });
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
    public async Task GenerateToken()
    {
        TokenRequest tokenRequest = new TokenRequest
        {
            Username = "admin",
            Password = "password123"
        };
        var authResponse = await Request.PostAsync("auth", new APIRequestContextOptions { DataObject = tokenRequest });

        var jsonAuthResponse = await authResponse.JsonAsync<TokenResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        _token = jsonAuthResponse.Token;
    }
}