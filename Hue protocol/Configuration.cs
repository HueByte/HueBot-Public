using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HueProtocol {
    public sealed class Configuration {
        public char Prefix { get; set; }
        public string Token { get; set; }
        public string ConnectionString { get; set; }
        public string CoronaApiHost { get; set; }
        public string CoronaApiKey { get; set; }
        public string GiphyApiKey { get; set; }

        public const string FILE_NAME = "config.json";

        [JsonIgnore]
        public static bool IsCreated
            => File.Exists(FILE_NAME);

        public static Configuration Create() {
            if (IsCreated) {
                return Load();
            }

            var configuration = new Configuration {
                Prefix = '$',
                Token = ""
            };

            File.WriteAllBytes(FILE_NAME, JsonSerializer.SerializeToUtf8Bytes(configuration));
            return configuration;
        }

        public static Configuration Load() {
            var readBytes = File.ReadAllBytes(FILE_NAME);
            var configuration = JsonSerializer.Deserialize<Configuration>(readBytes);
            return configuration;
        }
    }
}