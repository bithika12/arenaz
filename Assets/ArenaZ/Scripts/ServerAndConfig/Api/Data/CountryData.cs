using Newtonsoft.Json;

namespace RedApple.Api.Data
{
    public class CountryData
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("country_code")]
        public string Country_code { get; set; }
        [JsonProperty("country_name")]
        public string Country_name { get; set; }
        [JsonProperty("region_code")]
        public string Region_code { get; set; }
        [JsonProperty("region_name")]
        public string Region_name { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("zip_code")]
        public string Zip_code { get; set; }
        [JsonProperty("time_zone")]
        public string Time_zone { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        [JsonProperty("metro_code")]
        public string Metro_code { get; set; }
    }
}
