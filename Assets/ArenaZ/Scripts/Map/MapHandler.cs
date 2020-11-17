using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedApple;
using ArenaZ.Wallet;
using System;
using RedApple.Utils;
using Newtonsoft.Json;
using System.Linq;

namespace ArenaZ
{
    public class MapHandler : MonoBehaviour
    {
        [SerializeField] private List<MapElement> mapElements = new List<MapElement>();
        private List<GameData> gameDatas = new List<GameData>();

        private void Start()
        {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
        }

        public void GetGames()
        {
            RestManager.GetGameList(onComplete, onError);
        }

        private void onComplete(GameListResponse a_Response)
        {
            gameDatas = a_Response.GameList;
            for (int i = 0; i < gameDatas.Count && i < mapElements.Count; i++)
            {
                mapElements[i].Initialize(gameDatas[i], onClickMapElement);
            }
        }

        private void onError(RestUtil.RestCallError a_Error)
        {
            Debug.LogError(a_Error.Error);
        }

        private void onClickMapElement(GameData a_Data)
        {
            User.UserSelectedGame = a_Data.Id;
            Debug.Log($"Selected GameId: {User.UserSelectedGame}");
        }
    }

    [System.Serializable]
    public class GameListResponse
    {
        [JsonProperty("allgames")]
        public List<GameData> GameList = new List<GameData>();
    }

    [System.Serializable]
    public class GameData
    {
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("_id")]
        public string Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("score")]
        public string Score;
        [JsonProperty("details")]
        public string Details;
        //[JsonProperty("__v")]
        //public string Version;
    }
}