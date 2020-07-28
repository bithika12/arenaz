using Newtonsoft.Json;

public class GameOver
{
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("roomName")]
    public string RoomName { get; set; }
}
