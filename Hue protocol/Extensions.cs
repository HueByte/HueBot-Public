using System;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Console = Colorful.Console;

namespace HueProtocol {
    public static class Extensions {
        public static IServiceCollection AddServicesOfT<T>(this IServiceCollection serviceCollection) {
            var servicesOfT = typeof(Program).Assembly
                .GetTypes()
                .Where(x => !x.IsInterface && typeof(T).IsAssignableFrom(x));

            foreach (var service in servicesOfT) {
                serviceCollection.AddSingleton(service);
            }

            return serviceCollection;
        }

        public static Color GetLogLevelColor(this LogLevel logLevel) {
            return logLevel switch {
                LogLevel.Trace       => Color.LightBlue,
                LogLevel.Debug       => Color.PaleVioletRed,
                LogLevel.Information => Color.GreenYellow,
                LogLevel.Warning     => Color.Coral,
                LogLevel.Error       => Color.Crimson,
                LogLevel.Critical    => Color.Red,
                LogLevel.None        => Color.Coral,
                _                    => Color.White
            };
        }

        public static string GetShortLogLevel(this LogLevel logLevel) {
            return logLevel switch {
                LogLevel.Trace       => "TRCE",
                LogLevel.Debug       => "DBUG",
                LogLevel.Information => "INFO",
                LogLevel.Warning     => "WARN",
                LogLevel.Error       => "EROR",
                LogLevel.Critical    => "CRIT",
                LogLevel.None        => "NONE",
                _                    => "UKON"
            };
        }

        public static void PrintHeader() {
            const string LOGO =
                @"
  ___ ___                 __________                __                      .__   
 /   |   \ __ __   ____   \______   \_______  _____/  |_  ____   ____  ____ |  |  
/    ~    \  |  \_/ __ \   |     ___/\_  __ \/  _ \   __\/  _ \_/ ___\/  _ \|  |  
\    Y    /  |  /\  ___/   |    |     |  | \(  <_> )  | (  <_> )  \__(  <_> )  |__
 \___|_  /|____/  \___  >  |____|     |__|   \____/|__|  \____/ \___  >____/|____/
       \/             \/                                            \/            
";

            Console.WriteWithGradient(LOGO, Color.MediumPurple, Color.Cyan);
            Console.ReplaceAllColorsWithDefaults();
        }

        public static async Task<T> ReadAsJson<T>(this HttpClient httpClient, string url) {
            
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            using var responseMessage = await httpClient.SendAsync(requestMessage);

            if (!responseMessage.IsSuccessStatusCode) {
                throw new Exception(responseMessage.ReasonPhrase);
            }

            using var content = responseMessage.Content;
            if (content.Headers.ContentType.MediaType != "application/json") {
                throw new Exception("URL returned invalid content type.");
            }

            var stream = await content.ReadAsByteArrayAsync();
            var result = JsonSerializer.Deserialize<T>(stream);
            httpClient.DefaultRequestHeaders.Clear();
            return result;
        }
    }
}