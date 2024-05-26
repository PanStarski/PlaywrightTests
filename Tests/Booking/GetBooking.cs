﻿using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;
using PlaywrightTests.Models;
using System.Text.Json;

namespace PlaywrightTests.Tests;

[TestFixture]
public class GetBookingTest
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
    [Test]
    public async Task GetBookingReturnsTheCreatedEntry()
    {
        var responsea = await Request.PostAsync("booking", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
            DataObject = newBooking
        });

        Assert.AreEqual(200, responsea.Status, "Expected status code: 200");

        var jsonResponse = await responsea.JsonAsync<BookingResponse>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var bookingID = jsonResponse.Id;

        var responseb = await Request.GetAsync($"booking/{bookingID}");
        var jsonResponseb = await responseb.JsonAsync<Models.BookingRequest>(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.That(responseb.Ok);
        Assert.AreEqual(200, responseb.Status, "Expected status code: 200");

        Assert.IsNotEmpty(jsonResponseb.Firstname, "The Firstname should not be empty.");
        Assert.IsNotEmpty(jsonResponseb.Lastname, "The Lastname should not be empty.");

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
