using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NarLib
{
    public class RegistrationResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
		[JsonPropertyName("clientId")]
		public int ClientId { get; set; }
		[JsonPropertyName("secret")]
		public string Secret { get; set; }
    }
}
