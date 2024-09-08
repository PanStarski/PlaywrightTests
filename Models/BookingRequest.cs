using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlaywrightTests.Models
{
    public class BookingRequest : BookingRequestBase
    {
        [JsonPropertyName("totalprice")]
        public int? Totalprice { get; set;}
        [JsonPropertyName("depositpaid")]
        public bool? IsPaid { get; set; }
        [JsonPropertyName("bookingdates")]
        public BookingDates? BookingDates { get; set; }
        [JsonPropertyName("additionalneeds")]
        public string? Additionalneeds { get; set; }
    }
    public class BookingDates
    {
        [JsonPropertyName("checkin")]
        public DateTime? Checkin { get; set; }
        [JsonPropertyName("checkout")]
        public DateTime? Checkout { get; set; }
    }
}
