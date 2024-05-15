using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlaywrightTests.Models
{
    internal class BookingResponse
    {
        [JsonPropertyName("bookingid")]
        public int Id { get; set; }
    }
}
