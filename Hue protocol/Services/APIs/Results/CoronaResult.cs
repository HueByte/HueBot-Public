using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace HueProtocol.Services.APIs.Results
{
    public struct CoronaResult
    {
        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("confirmed")]
        public long Confirmed { get; set; }

        [JsonPropertyName("recovered")]
        public long Recovered { get; set; }

        [JsonPropertyName("critical")]
        public long Critical { get; set; }

        [JsonPropertyName("deaths")]
        public long Deaths { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("lastChange")]
        public DateTimeOffset LastChange { get; set; }

        [JsonPropertyName("lastUpdate")]
        public DateTimeOffset LastUpdate { get; set; }
    }
}
