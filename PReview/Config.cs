using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace PReview
{
    public class Config
    {
        public static readonly string CONFIG_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PReview");
        public static readonly string CONFIG_PATH;

        private static readonly JsonSerializerSettings settings;

        static Config() {
            CONFIG_PATH = Path.Combine(CONFIG_DIR, "config.json");
            settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }

        public static Config fromFile(string path)
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path), settings);
        }

        [JsonProperty]
        public string GithubUrl { get; private set; } = "https://github.devcloud.elisa.fi/";
        [JsonProperty]
        public string Organization { get; private set; } = "";
        [JsonProperty]
        public string Repository { get; private set; } = "";
        [JsonProperty]
        public string ApiToken { get; private set; } = "";

        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, settings));
        }
    }
}
