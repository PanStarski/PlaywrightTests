using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlaywrightTests.Models
{
    internal class BookingRequestBase
    {
        [JsonPropertyName("firstname")]
        public string? Firstname { get; set; }
        [JsonPropertyName("lastname")]
        public string? Lastname { get; set; }
    }
}
