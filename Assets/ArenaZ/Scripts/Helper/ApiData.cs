using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class GameHistoryMatchDetails
{
    [JsonProperty("matchDetails")]
    public List<GameHistoryGameDetails> gameHistoryGameDetails { get; set; } = new List<GameHistoryGameDetails>();
}

[System.Serializable]
public class GameHistoryGameDetails
{
    [JsonProperty("game_time")]
    public int GameTime { get; set; }
    [JsonProperty("last_time")]
    public int LastTime { get; set; }
    [JsonProperty("gameDetails")]
    public List<GameHistoryUserData> gameHistoryUserDatas { get; set; } = new List<GameHistoryUserData>();
}

[System.Serializable]
public class GameHistoryUserData
{
    [JsonProperty("userId")]
    public string Id { get; set; }
    [JsonProperty("userName")]
    public string Name { get; set; }
    [JsonProperty("userScore")]
    public string Score { get; set; }
    [JsonProperty("cupNumber")]
    public int CupNumber { get; set; }
    [JsonProperty("colorName")]
    public string ColorName { get; set; }
    [JsonProperty("raceName")]
    public string RaceName { get; set; }
    [JsonProperty("coinNumber")]
    public int CoinNumber { get; set; }
    [JsonProperty("gameResult")]
    public int GameResult { get; set; }
}

[System.Serializable]
public class MessageDetails
{
    [JsonProperty("details")]
    public List<MessageData> messageDatas { get; set; } = new List<MessageData>();
}

[System.Serializable]
public class MessageData
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("cretaedTime")]
    public string CretaedTime { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
}

[System.Serializable]
public class LeaderboardDetails
{
    [JsonProperty("details")]
    public List<LeaderboardData> leaderboardDatas { get; set; } = new List<LeaderboardData>();
}

[System.Serializable]
public class LeaderboardData
{
    [JsonProperty("userRank")]
    public int Rank { get; set; }
    [JsonProperty("userName")]
    public string Name { get; set; }
    [JsonProperty("cupNumber")]
    public int CupNumber { get; set; }
    [JsonProperty("colorName")]
    public string ColorName { get; set; }
    [JsonProperty("raceName")]
    public string RaceName { get; set; }
}

[System.Serializable]
public class UserConnectedDetails
{
    [JsonProperty("roomName")]
    public string RoomName { get; set; }
    [JsonProperty("users")]
    public List<UserJoin> Users = new List<UserJoin>();
}
