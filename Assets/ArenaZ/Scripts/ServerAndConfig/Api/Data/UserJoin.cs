using Newtonsoft.Json;

public class UserJoin
{
    [JsonProperty("userName")]
    public string UserName { get; set; }
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("colorName")]
    public string ColorName { get; set; }
    [JsonProperty("raceName")]
    public string RaceName { get; set; }
    [JsonProperty("dartName")]
    public string DartName { get; set; }
}
