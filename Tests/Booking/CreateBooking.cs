using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Text.Json;
using PlaywrightTests.Models;

namespace PlaywrightTests.Tests;

[TestFixture]
internal class CreateBookingTests : ApiTestsBase
{
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
    public async Task CreateBookingAddsNewEntry()
    {
        BookingResponse? createBookingJsonResponse = await _driver.CreateBooking(newBooking);
        var bookingID = createBookingJsonResponse.Id;

        log.Info("Sending GET request with BookingID from POST Request");
        var getBookingResponse = await _driver.RequestContext.GetAsync($"booking/{bookingID}");

        Assert.That(getBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200. The booking hasn't been found.");
        log.Info("The booking entry has been found. Test passed");
    }


}