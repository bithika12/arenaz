using Newtonsoft.Json;
using System.Collections.Generic;

namespace RedApple.Api.Data
{
    public class CreateAccount
    {
        [JsonProperty("userID")]
        public string UserId { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }

    public class UserSelectionDetails
    {
        [JsonProperty("colorName")]
        public string ColorName { get; set; }
        [JsonProperty("raceName")]
        public string RaceName { get; set; }
        [JsonProperty("characterId")]
        public string CharacterId { get; set; }
        [JsonProperty("dartName")]
        public string DartName { get; set; }
    }
}
