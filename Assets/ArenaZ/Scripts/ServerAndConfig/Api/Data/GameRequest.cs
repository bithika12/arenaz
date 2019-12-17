
using Newtonsoft.Json;

public struct GameRequest
{
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("score")]
    public string Score { get; set; }
    [JsonProperty("total")]
    public string Total { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("isWin")]
    public string IsWin { get; set; }
    [JsonProperty("turn")]
    public string Turn { get; set; }
}
