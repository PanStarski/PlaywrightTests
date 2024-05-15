using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlaywrightTests.Models
{
    internal class TokenRequest
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}
