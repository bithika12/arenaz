using Newtonsoft.Json;

namespace RedApple.Api.Data
{
    public struct GamePlayDataFormat<T>
    {
        public int status;
        public string message;
        [JsonProperty("result")]
        public Result<T> result;
    }

    public struct Result<T>
    {
        [JsonProperty("roomName")]
        public string RoomName { get; set; }
        [JsonProperty("users")]
        public T[] Users;
    }

}
