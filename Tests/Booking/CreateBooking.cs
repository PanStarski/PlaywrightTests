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
    public void GlobalSetup()
    {
        log4net.Config.XmlConfigurator.Configure();
    }
    private static readonly ILog log = LogManager.GetLogger(typeof(CreateTokenTest));

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
    [Test]
    public async Task CreateBookingAddsNewEntry()
    {
        var createBookingResponse = await Request.PostAsync("booking", new APIRequestContextOptions
        {
            DataObject =  newBooking
        });

        Assert.That(createBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200");

        var createBookingJsonResponse = await createBookingResponse.JsonAsync<BookingResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var bookingID = createBookingJsonResponse.Id;


        var getBookingResponse = await Request.GetAsync($"booking/{bookingID}");

        Assert.That(getBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200. The booking hasn't been found.");
        
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