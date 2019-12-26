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

        #region Socket Emit
        public void AddUser()
        {
            AccesToken acToken = new AccesToken
            {
                AccessToken = User.UserAccessToken
            };
            string addUserJsonData = DataConverter.SerializeObject(acToken);
            Debug.Log("Add User");
            Debug.Log("Access Token: " + addUserJsonData);
            socket.EmitJson(SocketEmitEvents.addUser.ToString(), addUserJsonData);
        }

        public void GameRequest()
        {
            AccesToken acToken = new AccesToken
            {
                AccessToken = User.UserAccessToken
            };
            string gameReqJsonData = DataConverter.SerializeObject(acToken);
            Debug.Log("Game Request");
            Debug.Log("Access Token: " + gameReqJsonData);
            socket.EmitJson(SocketEmitEvents.gameRequest.ToString(), gameReqJsonData);
        }

        public void ColRequest()
        {
            ColorRequest colorRequest = new ColorRequest
            {
                AccessToken = User.UserAccessToken,
                Color = User.userColor,
                CharRace = User.userRace
            };
            string colorReqJsonData = DataConverter.SerializeObject(colorRequest);
            Debug.Log("Color Request");
            Debug.Log("Access Token: " + colorReqJsonData);
            socket.EmitJson(SocketEmitEvents.colorRequest.ToString(), colorReqJsonData);
        }

        public void ThrowDartData(int hitValue, Vector3 point)
        {
            ThrowDart throwDart = new ThrowDart
            {
                AccessToken = User.UserAccessToken,
                Score = hitValue.ToString(),
                RoomName = User.RoomName,
                DartPoint = point.ToString()
            };
            string throwDartData = DataConverter.SerializeObject(throwDart);
            Debug.Log("Throw Dart");
            Debug.Log("Access Token: " + throwDartData);
            socket.EmitJson(SocketEmitEvents.throwDart.ToString(), throwDartData);
        }

        #endregion

        #region Socket Callbacks

        private void OnConnected()
        {
            Debug.LogWarning("SocketConnected : " + socket);
            SocketListener.ActivateListener();
            AddEvents();
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
            Config.SocketConfig.SocketListenEvents.ForEach(evt => socket.On(evt, onListen));
        }
        #endregion
    }
}

[Serializable]
public struct AccesToken
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
}

[Serializable]
public struct ColorRequest
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
    [JsonProperty("color")]
    public string Color { get; set; }
    [JsonProperty("race")]
    public string CharRace { get; set; }
}
[Serializable]
public struct ThrowDart
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
    [JsonProperty("score")]
    public string Score { get; set; }
    [JsonProperty("roomName")]
    public string RoomName { get; set; }
    [JsonProperty("dartPoint")]
    public string DartPoint { get; set; }
}
