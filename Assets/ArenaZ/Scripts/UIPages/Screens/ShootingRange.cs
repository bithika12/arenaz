using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using RedApple.Api.Data;
using System;
using ArenaZ.Screens;
using DevCommons.Utility;

namespace ArenaZ.GameMode
{
    public class ShootingRange : Singleton<ShootingRange>
    {
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button shootingRangeBackButton;
        [SerializeField] private Button coinButton10;
        [SerializeField] private Button coinButton50;
        [SerializeField] private Button coinButton100;
        [SerializeField] private Button coinButton250;
        [SerializeField] private Button coinButton500;

        [Header("User Image")]
        [SerializeField] private Image userFrame;
        [SerializeField] private Image userPic;

        [Header("Images")]
        [SerializeField] private Image dartImage;

        [Header("Text")]
        [SerializeField] private Text userName;

        [SerializeField] private GameLoading gameLoading;

        public Action<string> setOpponentName;
        public Action<string, string> setOpponentImage;

        private void Start()
        {
            GettingButtonReferences();
            ListenSocketEvents();
            CharacterSelection.setDart += SetDartImage;
            UIManager.Instance.setUserName += SetUserName;
            UIManager.Instance.showProfilePic += SetUserProfileImage;
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();        
        }

        #region ButtonReferences
        private void GettingButtonReferences()
        {
            shootingRangeBackButton.onClick.AddListener(OnClickShootingRangeBack);
            coinButton10.onClick.AddListener(() => OnClickStartGameWithCoinValue(10));
            coinButton50.onClick.AddListener(() => OnClickStartGameWithCoinValue(50));
            coinButton100.onClick.AddListener(() => OnClickStartGameWithCoinValue(100));
            coinButton250.onClick.AddListener(() => OnClickStartGameWithCoinValue(250));
            coinButton500.onClick.AddListener(() => OnClickStartGameWithCoinValue(500));
        }

        private void ReleaseButtonReferences()
        {
            shootingRangeBackButton.onClick.RemoveAllListeners();
            coinButton10.onClick.RemoveAllListeners();
            coinButton50.onClick.RemoveAllListeners();
            coinButton100.onClick.RemoveAllListeners();
            coinButton250.onClick.RemoveAllListeners();
            coinButton500.onClick.RemoveAllListeners();
        }
        #endregion

        private void ListenSocketEvents()
        {
            SocketListener.Listen(SocketListenEvents.userJoined.ToString(), OnUserJoin);
            SocketListener.Listen(SocketListenEvents.noUser.ToString(), OnNoUser);
            SocketListener.Listen(SocketListenEvents.gameStart.ToString(), OnGameStart);
        }

        private void OnGameStart(string data)
        {
            gameLoading.HideLoadingScreen();
            UIManager.Instance.HideScreen(Page.PlayerWinPanel.ToString());
            UIManager.Instance.HideScreen(Page.PlayerLoosePanel.ToString());

            Debug.Log($"Game Start : {data}");
            var gameStartData = DataConverter.DeserializeObject<GamePlayDataFormat<UserJoin>>(data);
            User.RoomName = gameStartData.result.RoomName;
            Debug.Log($"User Join RoomName: {User.RoomName}");
            UserJoin[] users = gameStartData.result.Users;
            for (int i = 0; i < users.Length; i++)
            {
                ScoreData.requiredScore = users[i].Score;
                if (User.UserId != users[i].UserId)
                {
                    saveOpponentData(users[i]);
                    setOpponentName?.Invoke(users[i].UserName);
                    setOpponentImage?.Invoke(users[i].RaceName, users[i].ColorName);
                    UIManager.Instance.ShowScreen(Page.PlayerMatchPanel.ToString(),Hide.none);
                }
            }
            GameManager.Instance.ResetScore();
            GameManager.Instance.GetDartGameObj();
            PlayerMatch.Instance.LoadGameplay();
        }

        private void saveOpponentData(UserJoin joinedUser)
        {
            Opponent.opponentName = joinedUser.UserName;
            Opponent.opponentId = joinedUser.UserId;
            Opponent.opponentRace = joinedUser.RaceName;
            Opponent.opponentColor = joinedUser.ColorName;
            Opponent.dartName = joinedUser.DartName;
        }

        private void OnUserJoin(string data)
        {
            Debug.Log($"User Joined: {data}");
            var userJoinData = DataConverter.DeserializeObject<GamePlayDataFormat<UserJoin>>(data);
            Debug.Log($"User Joined RoomName: {userJoinData.result.RoomName}");
            User.RoomName = userJoinData.result.RoomName;
        }

        private void OnNoUser(string data)
        {
            Debug.Log($"No User: {data}");
            gameLoading.HideLoadingScreen();
        }

        public void SetDartImage(string dartName)
        {
            string path = GameResources.dartImageFolderPath + "/" + dartName + User.UserColor;
            Debug.Log($"2D Dart Path: {path}");
            dartImage.sprite = Resources.Load<Sprite>(path);
        }

        public void SetUserProfileImage(string race, string color)
        {
            ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), race);
            EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), color);

            SquareFrameData t_FrameData = DataHandler.Instance.GetSquareFrameData(t_Color);
            if (t_FrameData != null)
            {
                userFrame.sprite = t_FrameData.FramePic;
            }

            CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
            if (t_CharacterPicData != null)
            {
                userPic.sprite = t_CharacterPicData.ProfilePic;
            }
        }

        public void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        private void OnClickShootingRangeBack()
        {
            UIManager.Instance.HideOpenScreen();
        }

        private void OnClickStartGameWithCoinValue(int a_Value)
        {
            PlayerPrefs.SetInt(ConstantStrings.ROOM_VALUE, a_Value);
            gameLoading.WaitingForOtherPlayer();
            SocketManager.Instance.GameRequest();
        }
    }
}
