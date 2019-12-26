using Newtonsoft.Json;

public class DartThrow
{
    [JsonProperty("roomName")]
    public string RoomName { get; set; }
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("remaining Score")]
    public string RemainingScore { get; set; }
    [JsonProperty("dartPoint")]
    public string DartPoint { get; set; }
}
