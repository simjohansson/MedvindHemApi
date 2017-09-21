namespace MedvindhemApi.Models
{
    using Newtonsoft.Json;

    public partial class WeatherData
    {
        [JsonProperty("station")]
        public StationData[] Stations { get; set; }
    }

    public partial class StationData
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }

        [JsonProperty("from")]
        public double From { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("to")]
        public long To { get; set; }

        [JsonProperty("value")]
        public Value[] Value { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("quality")]
        public string Quality { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("value")]
        public string WeatherValue { get; set; }
    }


    public partial class WeatherData
    {
        public static WeatherData FromJson(string json)
        {
            return JsonConvert.DeserializeObject<WeatherData>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this WeatherData self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
