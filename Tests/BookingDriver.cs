using Microsoft.Playwright;
using PlaywrightTests.Models;
using System.Text.Json;

namespace PlaywrightTests.Tests
{
    public class BookingDriver
    {
        public readonly IAPIRequestContext RequestContext;
        public BookingDriver(IAPIRequestContext requestContext)
        {
            RequestContext = requestContext; 
        }
        public async Task<BookingResponse?> CreateBooking(BookingRequest body)
        {
            log.Info("Starting a test");
            log.Info("Sending a POST Booking request");
            var createBookingResponse = await RequestContext.PostAsync("booking", new APIRequestContextOptions
            {
                DataObject = body
            });
            log.Info("Asserting that request return 200 OK");
            Assert.That(createBookingResponse.Status, Is.EqualTo(200), "Expected status code: 200");
            log.Info("Getting BookingID from response body");
            var createBookingJsonResponse = await createBookingResponse.JsonAsync<BookingResponse>(new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            return createBookingJsonResponse;
        }
    }
}
