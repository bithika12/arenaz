using System;
using RedApple;
using UnityEngine;
using RedApple.Api.Data;

namespace ArenaZ.Manager
{
    public class LevelManager : Singleton<LevelManager>
    {
        // Private Variables

        // Public Variables

        private void Start()
        {
            SocketListener.Listen("gameStart", OnGameStart);
        }

        private void OnGameStart(string data)
        {
            Debug.Log($"Game Start : {data}");
            var gameRqstData = DataConverter.DeserializeObject<GamePlayDataFormat<GameRequest>>(data);
            Debug.Log("Room Name: " + gameRqstData.result.roomName);
        }
    }
}
