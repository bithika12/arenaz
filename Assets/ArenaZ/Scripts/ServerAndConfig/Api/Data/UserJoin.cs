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
    [JsonProperty("score")]
    public int Score { get; set; }
    [JsonProperty("total")]
    public int Total { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("isWin")]
    public int IsWin { get; set; }
    [JsonProperty("turn")]
    public int Turn { get; set; }
    [JsonProperty("dartPoint")]
    public string DartPoint { get; set; }
    [JsonProperty("total_no_win")]
    public int TotalNoWin { get; set; }
    [JsonProperty("roomCoin")]
    public int RoomCoin { get; set; }
    [JsonProperty("totalCupWin")]
    public int TotalCupWin { get; set; }
}
