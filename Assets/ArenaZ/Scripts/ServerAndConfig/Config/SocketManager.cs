using UnityEngine;
using System;
using socket.io;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

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
            int t_RoomCoin = PlayerPrefs.GetInt(ConstantStrings.ROOM_VALUE, 10);
            GameRequest gameRequest = new GameRequest
            {
                AccessToken = User.UserAccessToken,
                RoomCoin = t_RoomCoin,
            };
            string gameReqJsonData = DataConverter.SerializeObject(gameRequest);
            Debug.Log("Game Request: " + gameReqJsonData);
            socket.EmitJson(SocketEmitEvents.gameRequest.ToString(), gameReqJsonData);
        }

        public void LeaveRoomRequest()
        {
            Debug.Log("RoomName: " + User.RoomName);
            LeaveRoomRequest leaveRoomRequest = new LeaveRoomRequest
            {
                AccessToken = User.UserAccessToken,
                RoomName = User.RoomName,
            };
            string leaveRoomReqJsonData = DataConverter.SerializeObject(leaveRoomRequest);
            Debug.Log("Leave Room Request: " + leaveRoomReqJsonData);
            socket.EmitJson(SocketEmitEvents.leave.ToString(), leaveRoomReqJsonData);
        }

        public void ColRequest()
        {
            ColorRequest colorRequest = new ColorRequest
            {
                AccessToken = User.UserAccessToken,
                Color = User.UserColor,
                CharRace = User.UserRace,
                DartName = User.DartName
            };
            string colorReqJsonData = DataConverter.SerializeObject(colorRequest);
            Debug.Log("Color Request");
            Debug.Log("Access Token: " + colorReqJsonData);
            socket.EmitJson(SocketEmitEvents.colorRequest.ToString(), colorReqJsonData);
        }

        public void ThrowDartData(int hitValue, Vector3 point)
        {
            string hitPoint = Regex.Replace(point.ToString(), @"\s+", "");
            //string hitPoint = Regex.Replace(point.ToString(), @"\s", "");
            Debug.Log("RoomName: " + User.RoomName);
            ThrowDart throwDart = new ThrowDart
            {
                AccessToken = User.UserAccessToken,
                Score = hitValue.ToString(),
                RoomName = User.RoomName,
                DartPoint = hitPoint
            };
            string throwDartData = DataConverter.SerializeObject(throwDart);
            Debug.Log("Throw Dart Data: " + throwDartData);
            socket.EmitJson(SocketEmitEvents.throwDart.ToString(), throwDartData);
        }

        public void ThrowDartData(int hitValue, string point)
        {
            Debug.Log("RoomName: " + User.RoomName);
            ThrowDart throwDart = new ThrowDart
            {
                AccessToken = User.UserAccessToken,
                Score = hitValue.ToString(),
                RoomName = User.RoomName,
                DartPoint = point
            };
            string throwDartData = DataConverter.SerializeObject(throwDart);
            Debug.Log("Throw Dart");
            Debug.LogError("Throw Dart Data: " + throwDartData);
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
public struct GameRequest
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
    [JsonProperty("roomCoin")]
    public int RoomCoin { get; set; }
}

[Serializable]
public struct LeaveRoomRequest
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
    [JsonProperty("roomName")]
    public string RoomName { get; set; }
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
    [JsonProperty("dartName")]
    public string DartName { get; set; }
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
