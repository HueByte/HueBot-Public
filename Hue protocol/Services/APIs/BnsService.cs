using HueProtocol.Services.APIs.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HueProtocol.Services.APIs
{
    public class BnsService : IInjectableService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public BnsService(HttpClient httpClient, ILogger<BnsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BnsGearResult> GetAsync(string _character)
        {
            try
            {
                string character = _character.Replace(" ", "+");

                var bnsResult = await _httpClient.ReadAsJson<BnsGearResult>($"https://api.silveress.ie/bns/v3/character/full/eu/{character}");

                return string.IsNullOrEmpty(bnsResult.AccountName) || string.IsNullOrWhiteSpace(bnsResult.AccountName) ? default : bnsResult;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return default;
            }
        }
    }
}
