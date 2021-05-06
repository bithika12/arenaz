using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using RedApple.Api.Data;
using System;
using ArenaZ.Screens;
using DevCommons.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using DG.Tweening;
using Coffee.UIExtensions;
using System.Linq;
using System.Collections;
using RedApple.Utils;

namespace ArenaZ.GameMode
{
    public class ShootingRange : Singleton<ShootingRange>
    {
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button shootingRangeBackButton;
        [SerializeField] private Button notEnoughCoinCloseBtn;
        [SerializeField] private Button coinButton10;
        [SerializeField] private Button coinButton50;
        [SerializeField] private Button coinButton100;
        [SerializeField] private Button coinButton250;
        [SerializeField] private Button coinButton500;
        [SerializeField] private Button depositButton;

        [Header("User Image")]
        [SerializeField] private Image userFrame;
        [SerializeField] private Image userPic;

        [Header("Images")]
        [SerializeField] private Image dartImage;

        [Header("Text")]
        [SerializeField] private Text userName;
        [SerializeField] private Text userCoinCount;
        [SerializeField] private Text userCupCount;

        [Header("UI Effects")]
        [SerializeField] private List<UIShiny> uiEffects = new List<UIShiny>();
        [SerializeField] private List<Transform> uiButtons = new List<Transform>();

        [SerializeField] private GameLoading gameLoading;
        public Action<List<UserJoin>> setUsersData;
        public Action<string, string, string> setUserImage;
        public Action<string, string, string> setOpponentImage;
        public Action<string, string> setCupCount;

        private void Start()
        {
            //Debug.LogError("Shooting Range Start");
            GettingButtonReferences();
            ListenSocketEvents();
            CharacterSelection.setDart += SetDartImage;
            UIManager.Instance.setUserName += SetUserName;
            UIManager.Instance.showProfilePic += SetUserProfileImage;
            UIManager.Instance.setCoinAndCup += refreshData;
        }

        private void OnEnable()
        {
            dartImage.transform.DOLocalMoveY(-630.0f, 1.0f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
            InvokeRepeating("PlayShinyEffect", 1.0f, UnityEngine.Random.Range(3.0f, 5.0f));
            InvokeRepeating("ScaleSize", 4.0f, UnityEngine.Random.Range(4.0f, 6.0f));
        }

        private void OnDisable()
        {
            dartImage.transform.DOKill();
            CancelInvoke("PlayShinyEffect");
            CancelInvoke("ScaleSize");
        }

        private void OnClickNotEnoughCoinClose()
        {
            UIManager.Instance.HideScreen(Page.NotEnoughCoinOverlay.ToString());
        }

        public void Refresh()
        {
            RestManager.GetUserSelection(User.UserEmailId, OnGetUserSelection, OnRequestFailure);
        }

        private void refreshData(string coin, string cup)
        {
            if (userCoinCount != null)
                userCoinCount.text = coin;
            if (userCupCount != null)
                userCupCount.text = cup;
        }

        private void OnGetUserSelection(UserSelectionDetails userSelectionDetails)
        {
            Debug.Log($"------------------------------------USD: {JsonConvert.SerializeObject(userSelectionDetails)}");
            if (userSelectionDetails != null)
            {
                if (userCoinCount != null)
                    userCoinCount.text = userSelectionDetails.UserCoin.ToString();
                if (userCupCount != null)
                    userCupCount.text = userSelectionDetails.UserCup.ToString();

                Debug.Log($"Before: {User.UserCoin} - {userSelectionDetails.UserCoin}");
                User.UserCoin = userSelectionDetails.UserCoin;
                Debug.Log($"After: {User.UserCoin} - {userSelectionDetails.UserCoin}");
                User.UserCup = userSelectionDetails.UserCup;
                UIManager.Instance.setCoinAndCup?.Invoke(User.UserCoin.ToString(), User.UserCup.ToString());
            }
        }

        private void OnRequestFailure(RestUtil.RestCallError obj)
        {
            Debug.Log("Get Data Error: " + obj.Description);
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();        
        }

        #region ButtonReferences
        private void GettingButtonReferences()
        {
            shootingRangeBackButton.onClick.AddListener(OnClickShootingRangeBack);
            notEnoughCoinCloseBtn.onClick.AddListener(OnClickNotEnoughCoinClose);

            coinButton10.onClick.AddListener(() => OnClickStartGameWithCoinValue(10));
            coinButton50.onClick.AddListener(() => OnClickStartGameWithCoinValue(50));
            coinButton100.onClick.AddListener(() => OnClickStartGameWithCoinValue(100));
            coinButton250.onClick.AddListener(() => OnClickStartGameWithCoinValue(250));
            coinButton500.onClick.AddListener(() => OnClickStartGameWithCoinValue(500));
            depositButton.onClick.AddListener(openDepositCoinPanel);
        }

        private void ReleaseButtonReferences()
        {
            shootingRangeBackButton.onClick.RemoveAllListeners();
            notEnoughCoinCloseBtn.onClick.RemoveAllListeners();

            coinButton10.onClick.RemoveAllListeners();
            coinButton50.onClick.RemoveAllListeners();
            coinButton100.onClick.RemoveAllListeners();
            coinButton250.onClick.RemoveAllListeners();
            coinButton500.onClick.RemoveAllListeners();
            depositButton.onClick.RemoveListener(openDepositCoinPanel);
        }
        #endregion

        private void ListenSocketEvents()
        {
            SocketListener.Listen(SocketListenEvents.connectedRoom.ToString(), OnConnectedRoom);
            SocketListener.Listen(SocketListenEvents.noUser.ToString(), OnNoUser);
            SocketListener.Listen(SocketListenEvents.gameStart.ToString(), OnGameStart);
        }

        private void openDepositCoinPanel()
        {
            UIManager.Instance.ShowScreen(Page.WalletDepositRequestPanel.ToString());
        }
        
        private void PlayShinyEffect()
        {
            int t_RandIndex = UnityEngine.Random.Range(0, uiEffects.Count);
            float t_EffectVal = 0;
            Tween tween = DOTween.To(() => t_EffectVal, x =>
            {
                t_EffectVal = x;
                uiEffects[t_RandIndex].effectFactor = t_EffectVal;
            }, 1, 2.0f).SetEase(Ease.InSine).OnComplete(() =>
            {
                uiEffects[t_RandIndex].effectFactor = 0;
            });
        }

        private void ScaleSize()
        {
            int t_RandIndex = UnityEngine.Random.Range(0, uiButtons.Count);
            uiButtons[t_RandIndex].DOScale(1.1f, 0.5f).SetEase(Ease.InSine).SetLoops(6, LoopType.Yoyo);
        }

        private void OnGameStart(string data)
        {
            gameLoading.HideLoadingScreen();
            UIManager.Instance.HideScreen(Page.PlayerWinPanel.ToString());
            UIManager.Instance.HideScreen(Page.PlayerLoosePanel.ToString());

            Debug.Log($"Game Start : {data}");
            var gameStartData = DataConverter.DeserializeObject<GamePlayDataFormat<UserJoin>>(data);
            Debug.LogWarning($"----UserId: {User.UserId}");
            Debug.LogWarning($"----OnJoinRoom: {JsonConvert.SerializeObject(gameStartData)}");
            User.RoomName = gameStartData.result.RoomName;
            Debug.Log($"User Join RoomName: {User.RoomName}");
            UserJoin[] users = gameStartData.result.Users;

            string selfCup = "";
            string opponentCup = "";
            setUsersData?.Invoke(users.ToList());
            for (int i = 0; i < users.Length; i++)
            {
                ScoreData.requiredScore = users[i].Total;
                

                if (User.UserId.Equals(users[i].UserId))
                {
                    setUserImage?.Invoke(users[i].UserName, users[i].RaceName, users[i].ColorName);
                    selfCup = users[i].TotalCupWin.ToString();
                }
                else
                {
                    opponentCup = users[i].TotalCupWin.ToString();
                    saveOpponentData(users[i]);
                    setOpponentImage?.Invoke(users[i].UserName, users[i].RaceName, users[i].ColorName);
                    UIManager.Instance.ShowScreen(Page.PlayerMatchPanel.ToString(), Hide.none);
                }
            }
            setCupCount?.Invoke(selfCup, opponentCup);
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

        private void OnConnectedRoom(string data)
        {
            Debug.Log($"Connected Room: {data}");
            var t_Data = DataConverter.DeserializeObject<GamePlayDataFormat<UserConnectedDetails>>(data);
            Debug.Log($"ConnectedRoom RoomName: {t_Data.result.RoomName}");
            User.RoomName = t_Data.result.RoomName;
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
            //UIManager.Instance.HideOpenScreen();
            UIManager.Instance.HideScreen(Page.ShootingrangePanel.ToString());
            UIManager.Instance.ShowScreen(Page.LevelSelectionPanel.ToString());
        }

        private void OnClickStartGameWithCoinValue(int a_Value)
        {
            if (GameManager.Instance.GetGameplayMode() == GameManager.EGamePlayMode.Multiplayer)
            {
                if (!GameManager.Instance.InternetConnection())
                {
                    //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                    return;
                }

                UIManager.Instance.HideScreenImmediately(Page.NotEnoughCoinOverlay.ToString());
                if (a_Value <= User.UserCoin)
                {
                    PlayerPrefs.SetInt(ConstantStrings.ROOM_VALUE, a_Value);
                    gameLoading.WaitingForOtherPlayer();
                    SocketManager.Instance.GameRequest();
                }
                else
                    UIManager.Instance.ShowScreen(Page.NotEnoughCoinOverlay.ToString());
            }
            //else if (GameManager.Instance.GetGameplayMode() == GameManager.EGamePlayMode.Training)
            //{
            //    //PlayerPrefs.SetInt(ConstantStrings.ROOM_VALUE, a_Value);
            //    GameManager.Instance.StartTraining();
            //}
        }

        //private IEnumerator LeaveAndJoinRoomRequest()
        //{
        //    float t_Delay = 0.0f;
        //    if (GameManager.Instance.GameStatus == GameManager.EGameStatus.Playing)
        //    {
        //        t_Delay = 1.0f;
        //        Debug.Log("---------------Leaving Room---------------");
        //        SocketManager.Instance.LeaveRoomRequest();
        //    }
        //    yield return new WaitForSeconds(t_Delay);
        //    SocketManager.Instance.GameRequest();
        //}
    }
}
