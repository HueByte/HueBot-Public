using HueProtocol.Services.APIs.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HueProtocol.Services.APIs
{
    public class CoronaService : IInjectableService
    {
        private readonly ILogger<CoronaService> _logger;
        private readonly HttpClient _httpClient;
        private string host;
        private string apiKey;
        private const string baseUrl = "https://covid-19-data.p.rapidapi.com/country?format=json&name=";

        public CoronaService(HttpClient httpClinet, Configuration conf, ILogger<CoronaService> logger)
        {
            host = conf.CoronaApiHost;
            apiKey = conf.CoronaApiKey;
            _httpClient = httpClinet;
            _logger = logger;
        }

        public async Task<CoronaResult> GetAsync(string country)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", host);
                _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", apiKey);

                _logger.LogInformation($"{baseUrl}{country}");
                var result = await _httpClient.ReadAsJson<List<CoronaResult>>($"{baseUrl}{country}");
                
                return result[0];
            }
            catch(Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return default;
            }
        }
    }
}
