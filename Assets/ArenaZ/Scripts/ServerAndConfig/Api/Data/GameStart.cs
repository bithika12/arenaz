using Newtonsoft.Json;

public class GameStart
{
    [JsonProperty("userName")]
    public string UserName { get; set; }
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("colorName")]
    public string ColorName { get; set; }
    [JsonProperty("raceName")]
    public string RaceName { get; set; }
}
