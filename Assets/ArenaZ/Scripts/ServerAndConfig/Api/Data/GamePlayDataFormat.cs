using Newtonsoft.Json;

namespace RedApple.Api.Data
{
    public class GamePlayDataFormat<T>
    {
        public int status;
        public string message;
        public Result<T> result;
    }

    public class Result<T>
    {
        [JsonProperty("roomName")]
        public string roomName;
        [JsonProperty("users")]
        public T[] users;
    }
}
