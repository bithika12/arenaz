using Newtonsoft.Json;

public struct NextTurn
{
    [JsonProperty("userId")]
    public string UserId { get; set; }
}
