
using Newtonsoft.Json;

public struct GameRequest
{
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("userName")]
    public string UserName { get; set; }
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
}
