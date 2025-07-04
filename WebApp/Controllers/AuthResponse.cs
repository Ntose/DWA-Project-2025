// Models/AuthResponse.cs
using System;
using Newtonsoft.Json;

namespace WebApp.Models
{
    /// <summary>
    /// Represents the payload returned by the authentication API.
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// JWT issued by the API.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// Expiration date/time of the issued token.
        /// </summary>
        [JsonProperty("expiration")]
        public DateTime Expiration { get; set; }
    }
}
