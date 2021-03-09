using UnityEngine;
using System;
using socket.io;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DevCommons.Utility;
using System.Collections.Generic;
using ArenaZ.Manager;
using System.Collections;

namespace RedApple
{
    public interface ISocketState
    {
        void SocketStatus(SocketManager.ESocketStatus a_SocketState);
        void SocketReconnected();
    }

    public class SocketManager : Singleton<SocketManager>
    {
        public enum ESocketStatus
        {
            None,
            Connected,
            Disconnected,
        }

        public static Socket socket;
        public static event Action<string, string> onListen;
        public ESocketStatus SocketStatus { get; private set; } = ESocketStatus.None;

        public List<ISocketState> iSocketStateSubscribers = new List<ISocketState>();

        public string OldSocketId { get; private set; } = string.Empty;
        public string CurrentSocketId { get; private set; } = string.Empty;

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

        public void SetCurrentSocketId(string a_SocketId)
        {
            Debug.Log($"Socket Id: {a_SocketId}");
            CurrentSocketId = a_SocketId;

            if (string.IsNullOrEmpty(OldSocketId) || string.IsNullOrWhiteSpace(OldSocketId))
                OldSocketId = CurrentSocketId;
        }

        public void UpdateOldSocketId()
        {
            OldSocketId = CurrentSocketId;
        }

        public bool SocketIdsMatching()
        {
            return OldSocketId.Equals(CurrentSocketId);
        }

        #region Socket Emit
        public void AddUser()
        {
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
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
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
            int t_RoomCoin = PlayerPrefs.GetInt(ConstantStrings.ROOM_VALUE, 10);
            GameRequest gameRequest = new GameRequest
            {
                AccessToken = User.UserAccessToken,
                RoomCoin = t_RoomCoin,
                GameId = User.UserSelectedGame
            };
            string gameReqJsonData = DataConverter.SerializeObject(gameRequest);
            Debug.Log("Game Request: " + gameReqJsonData);
            socket.EmitJson(SocketEmitEvents.gameRequest.ToString(), gameReqJsonData);
        }

        public void GameRequestCancel()
        {
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
            Debug.Log("RoomName: " + User.RoomName);
            LeaveRoomRequest leaveRoomRequest = new LeaveRoomRequest
            {
                AccessToken = User.UserAccessToken,
                RoomName = User.RoomName,
            };
            string leaveRoomReqJsonData = DataConverter.SerializeObject(leaveRoomRequest);
            Debug.Log("Game Request Cancel: " + leaveRoomReqJsonData);
            socket.EmitJson(SocketEmitEvents.gameRequestCancel.ToString(), leaveRoomReqJsonData);
        }

        public void LeaveRoomRequest()
        {
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
            Debug.Log("Leave RoomName: " + User.RoomName);
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
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
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

        public void ThrowDartData(int socreValue, int hitValue, int multiplierValue, Vector3 point)
        {
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
            string hitPoint = Regex.Replace(point.ToString(), @"\s+", "");
            //string hitPoint = Regex.Replace(point.ToString(), @"\s", "");
            Debug.Log("RoomName: " + User.RoomName);
            ThrowDart throwDart = new ThrowDart
            {
                AccessToken = User.UserAccessToken,
                Score = socreValue.ToString(),
                HitScore = hitValue,
                ScoreMultiplier = multiplierValue,
                RoomName = User.RoomName,
                DartPoint = hitPoint
            };
            string throwDartData = DataConverter.SerializeObject(throwDart);
            Debug.Log("Throw Dart Data: " + throwDartData);
            socket.EmitJson(SocketEmitEvents.throwDart.ToString(), throwDartData);
        }

        public void ThrowDartData(int hitValue, string point)
        {
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
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

        public void ReJoinRequest()
        {
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
            // {"accessToken":"de103012-0cbe-4256-8222-4e5eb172f040","roomName":"RM1589509679450"}
            ReJoinData reJoinData = new ReJoinData
            {
                AccessToken = User.UserAccessToken,
                RoomName = User.RoomName
            };

            string reJoinRequestData = DataConverter.SerializeObject(reJoinData);
            Debug.LogError("ReJoin Data: " + reJoinRequestData);
            socket.EmitJson(SocketEmitEvents.reJoin.ToString(), reJoinRequestData);
        }
        #endregion

        #region Socket Callbacks
        private void OnConnected()
        {
            iSocketStateSubscribers.ForEach(x => x.SocketStatus(ESocketStatus.Connected));
            SocketStatus = ESocketStatus.Connected;
            Debug.LogWarning("SocketConnected : " + socket);
            SocketListener.ActivateListener();
            AddEvents();
        }

        private void OnReConnected(int val)
        {
            iSocketStateSubscribers.ForEach(x =>
            {
                x.SocketStatus(ESocketStatus.Connected);
                x.SocketReconnected();
            });
            SocketStatus = ESocketStatus.Connected;
            Debug.LogWarning("SocketReConnected : " + val);
        }

        private void OnConnectedError(Exception error)
        {
            iSocketStateSubscribers.ForEach(x => x.SocketStatus(ESocketStatus.Disconnected));
            SocketStatus = ESocketStatus.Disconnected;
            Debug.LogError("ConnectedError");
            //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
        }

        private void OnDisconnected()
        {
            iSocketStateSubscribers.ForEach(x => x.SocketStatus(ESocketStatus.Disconnected));
            SocketStatus = ESocketStatus.Disconnected;
            Debug.LogError("Disconnected");
            //Invoke(nameof(chectSocketConnection), 2.5f);
        }

        private void OnTimeout()
        {
            iSocketStateSubscribers.ForEach(x => x.SocketStatus(ESocketStatus.Disconnected));
            SocketStatus = ESocketStatus.Disconnected;
            Debug.LogError("Timeout");
            //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
        }

        private void AddEvents()
        {
            Debug.Log("Add Events");
            Config.SocketConfig.SocketListenEvents.ForEach(evt => socket.On(evt, onListen));
        }
        #endregion

        //private IEnumerator Emiting(string eventName, string messgae, bool jsonFormat = true)
        //{
        //    yield return new WaitUntil(() => socket != null && socket.IsConnected);
        //    if (jsonFormat)
        //    {
        //        Debug.Log($"{eventName} {messgae}");
        //        socket.EmitJson(eventName, messgae);
        //    }
        //    else
        //    {
        //        Debug.Log($"{eventName} {messgae}");
        //        socket.Emit(eventName, messgae);
        //    }
        //}
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
    [JsonProperty("gameId")]
    public string GameId { get; set; }
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
    [JsonProperty("hitScore")]
    public int HitScore { get; set; }
    [JsonProperty("scoreMultiplier")]
    public int ScoreMultiplier { get; set; }
}

[System.Serializable]
public class GameOverResponse
{
    [JsonProperty("firstUserId")]
    public string FirstUserId;
    [JsonProperty("firstUserGameStatus")]
    public string FirstUserGameStatus;
    [JsonProperty("secondUserId")]
    public string SecondUserId;
    [JsonProperty("secondUserGameStatus")]
    public string SecondUserGameStatus;
    [JsonProperty("roomName")]
    public string RoomName;
    [JsonProperty("firstUserCupNumber")]
    public int FirstUserCupNumber;
    [JsonProperty("secondUserCupNumber")]
    public int SecondUserCupNumber;
    [JsonProperty("firstUserCoinNumber")]
    public int FirstUserCoinNumber;
    [JsonProperty("secondUserCoinNumber")]
    public int SecondUserCoinNumber;
    [JsonProperty("firstUserTotalCup")]
    public int FirstUserTotalCup;
    [JsonProperty("secondUserTotalCup")]
    public int SecondUserTotalCup;
    [JsonProperty("completeStatus")]
    public int CompleteStatus;
}

[System.Serializable]
public class SocketUserData
{
    [JsonProperty("userId")]
    public string UserId;
}

[System.Serializable]
public class ReJoinData
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
    [JsonProperty("roomName")]
    public string RoomName { get; set; }
}
