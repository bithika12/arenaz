using Newtonsoft.Json;

public class DartThrow
{
    [JsonProperty("roomName")]
    public string RoomName { get; set; }
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("remainingScore")]
    public string RemainingScore { get; set; }
    [JsonProperty("dartPoint")]
    public string DartPoint { get; set; }
    [JsonProperty("playerScore")]
    public string PlayerScore { get; set; }
    [JsonProperty("hitScore")]
    public int HitScore { get; set; }
    [JsonProperty("scoreMultiplier")]
    public int ScoreMultiplier { get; set; }
    [JsonProperty("playStatus")]
    public string PlayStatus { get; set; }   
}
