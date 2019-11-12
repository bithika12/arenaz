using Newtonsoft.Json;

namespace RedApple.Api.Data
{
    public class UserLogin
    {
        [JsonProperty("userID")]
        public string UserId { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }
}
