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
        [JsonProperty("userCoin")]
        public int UserCoin { get; set; }
        [JsonProperty("userCup")]
        public int UserCup { get; set; }
        [JsonProperty("ip_verify")]
        public int IPVerify { get; set; }
        [JsonProperty("auto_refill_coins")]
        public int Auto_Refill_Coins { get; set; }

    }
}
