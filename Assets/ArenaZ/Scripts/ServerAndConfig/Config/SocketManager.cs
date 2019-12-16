using UnityEngine;
using System;
using socket.io;
using Newtonsoft.Json;

namespace RedApple
{
    public class SocketManager : Singleton<SocketManager>
    {
        public static Socket socket;
        public static event Action<string, string> onListen;

        private void Start()
        {
            Connect();
        }

        private void Connect()
        {
            socket = Socket.Connect(Config.SocketConfig.Host);

            socket.On(SystemEvents.connect, OnConnected);
            socket.On(SystemEvents.reconnect, OnReConnected);
            socket.On(SystemEvents.connectError, OnConnectedError);
            socket.On(SystemEvents.disconnect, OnDisconnected);
            socket.On(SystemEvents.connectTimeOut, OnTimeout);
        }

        public void AddUser()
        {
            AccesToken acToken = new AccesToken
            {
                AccessToken = User.accessToken
            };
            string addUserJsonData = DataConverter.SerializeObject(acToken);
            Debug.Log("Access Token: " + addUserJsonData);
            socket.EmitJson(SocketEmitEvents.addUser.ToString(), addUserJsonData);
        }

        public void GameRequest()
        {
            AccesToken acToken = new AccesToken
            {
                AccessToken = User.accessToken
            };
            string gameRqstJsonData = DataConverter.SerializeObject(acToken);
            Debug.Log("Access Token: " + gameRqstJsonData);
            socket.EmitJson(SocketEmitEvents.gameRequest.ToString(), gameRqstJsonData);
        }

        private void OnConnected()
        {
            Debug.LogWarning("SocketConnected : " + socket);
            AddEvents();
            SocketListener.ActivateListener();
        }

        private void OnReConnected(int val)
        {
            Debug.LogWarning("SocketReConnected : " + val);
        }

        private void OnConnectedError(Exception error)
        {
            Debug.LogError("ConnectedError");
        }

        private void OnDisconnected()
        {
            Debug.LogError("Disconnected");
        }

        private void OnTimeout()
        {
            Debug.LogError("Timeout");
        }

        private void AddEvents()
        {
            Debug.Log("Add Events");
            socket.On("gameStart", onGameStart);
            //Config.SocketConfig.SocketListenEvents.ForEach(evt => socket.On(evt, onListen));
            //Config.SocketConfig.SocketListenEvents.ForEach(evt => Debug.Log("Events Name:  "+evt));
        }

        private void onGameStart(string evtName, string value)
        {
            Debug.Log($"Event Name {evtName} {value}");
        }
    }
}

[Serializable]
public struct AccesToken
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
}
