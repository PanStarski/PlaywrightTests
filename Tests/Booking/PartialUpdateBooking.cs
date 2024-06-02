using Microsoft.Playwright;
using System.Text.Json;
using PlaywrightTests.Models;


namespace PlaywrightTests.Tests;

[TestFixture]
public class PartialUpdateBookingTests
{
    IPlaywright playwright;

    private IAPIRequestContext Request;

    string _token;

    [OneTimeSetUp]
    public async Task SetupApiTesting()
    {
        playwright = await Playwright.CreateAsync();
        await CreateAPIRequestContext();
        await GenerateToken();
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

    [Test]
    public async Task PartialUpdateBookingChangesTheExistentEntryContent()
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

        var updateBookingResponse = await Request.PatchAsync($"booking/{bookingID}", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Cookie", $"token={_token}" }
                },
            DataObject = updatedBooking
        });
        Assert.That(updateBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200");

        var getBookingResponse = await Request.GetAsync($"booking/{bookingID}");
        var getBookingJsonResponse = await getBookingResponse.JsonAsync<BookingRequest>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(getBookingResponse.Ok);
        Assert.That(getBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200");

        Assert.That(updatedBooking.Firstname, Is.EqualTo(getBookingJsonResponse.Firstname), "Expected Lastname to be updated");
        Assert.That(updatedBooking.Lastname, Is.EqualTo(getBookingJsonResponse.Lastname), "Expected Lastname to be updated");

    }

    [Test]

    public async Task UpdatedBookingReturns403StatusWithoutAuthentication()
    {
        var response = await Request.PatchAsync($"booking/1", new APIRequestContextOptions
        {
            DataObject = updatedBooking
        });
        Assert.That(response.Status, Is.EqualTo(403), "Expected status code: 403");
    }
    public async Task UpdatedBookingReturns403StatusWithWrongToken()
    {
        _token = "1234567890";
        var response = await Request.PatchAsync($"booking/1", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Cookie", $"token={_token}" }
                },
            DataObject = updatedBooking
        });
        Assert.That(response.Status, Is.EqualTo(403), "Expected status code: 403");
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