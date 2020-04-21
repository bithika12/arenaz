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
        [JsonProperty("userCoin")]
        public int UserCoin { get; set; }        
        [JsonProperty("userCup")]
        public int UserCup { get; set; }
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
        [JsonProperty("countryName")]
        public string CountryName { get; set; }
        [JsonProperty("languageName")]
        public string LanguageName { get; set; }
        [JsonProperty("userCoin")]
        public int UserCoin { get; set; }
        [JsonProperty("userCup")]
        public int UserCup { get; set; }
    }
}
