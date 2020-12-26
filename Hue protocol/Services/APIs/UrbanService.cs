using System;
using System.Net.Http;
using System.Threading.Tasks;
using HueProtocol.Services.APIs.Results;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Services.APIs {
    public sealed class UrbanService : IInjectableService {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public UrbanService(HttpClient httpClient, ILogger<UrbanService> logger) {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UrbanDefinition> GetAsync(string term) {
            try {
                var urbanResult =
                    await _httpClient.ReadAsJson<UrbanResult>($"http://api.urbandictionary.com/v0/define?term={term}");

                if (urbanResult.Definitions.Count <= 0) {
                    throw new Exception($"Nothing found for term: {term}");
                }

                return urbanResult.Definitions[0];
            }
            catch (Exception exception) {
                _logger.LogCritical(exception, exception.Message);
                return default;
            }
        }
    }
}