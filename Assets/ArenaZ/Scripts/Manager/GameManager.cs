using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArenaZ.Behaviour;
using ArenaZ.GameMode;
using ArenaZ.Screens;
using ArenaZ.ShootingObject;
using DevCommons.Utility;
using DG.Tweening;
using Newtonsoft.Json;
using RedApple;
using RedApple.Api;
using RedApple.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Elendow.SpritedowAnimator;

namespace ArenaZ.Manager
{
    [RequireComponent(typeof(TouchBehaviour))]
    public class GameManager : Singleton<GameManager>, ISocketState
    {
        public enum EGameState
        {
            Idle,
            Playing,
        }

        public enum Player
        {
            Self,
            Opponent,
            None,
        }

        public enum EGameStatus
        {
            None = 0,
            Playing,
        }

        public enum EGamePlayMode
        {
            None = 0,
            Multiplayer,
            Training,
        }

        public enum EGameRejoin
        {
            No = 0,
            Yes,
        }

        [Header("User")]
        [SerializeField] private Image userPic;
        [SerializeField] private Image userTimerImage;
        [SerializeField] private Text userName;
        [SerializeField] private TextMeshProUGUI userScore;

        [Header("Opponent")]
        [SerializeField] private Image opponentPic;
        [SerializeField] private Image opponentTimerImage;
        [SerializeField] private Text opponentName;
        [SerializeField] private TextMeshProUGUI opponentScore;

        [Header("Others")]
        [SerializeField] private int scorePopUpDuration;
        [SerializeField] private Text countdownTimerText;

        [SerializeField] private CameraController cameraController;
        [SerializeField] private UiPopup popup;
        [SerializeField] private UiTrainingPopup trainingPopup;

        [SerializeField] private GameObject winPopup;
        [SerializeField] private GameObject loosePopup;
        [SerializeField] private GameObject winPopupBG;
        [SerializeField] private GameObject loosePopupBG;
        [SerializeField] private GameObject dartBoard;
        [SerializeField] private Vector3 winnerPopupOriginalScale = new Vector3();

        [SerializeField] private TrainingScoreHandler trainingScoreHandler;
        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private ScoreGraphic scoreGraphic;

        [SerializeField] private CountdownTimer countdownTimer;

        private AudioPlayer mainMenuBGAudioPlayer;
        private AudioPlayer gameplayBGAudioPlayer;
        private AudioPlayer countdownAudioPlayer;

        private bool playerTurn = false;
        private GameObject userDart;
        private GameObject opponentDart;
        private Dart currentDart;
        private TouchBehaviour touchBehaviour;        
        //private Dictionary<string, int> gameScore = new Dictionary<string, int>();
        private float timer = ConstantInteger.timerValue;

        private EGameStatus gameStatus = EGameStatus.None;
        private bool gameIsSuspended = false;

        private EGamePlayMode gamePlayMode = EGamePlayMode.None;
        private EGameRejoin gameRejoin = EGameRejoin.No;

        [SerializeField] private AlarmClock alarmClock;

        [Header("Hit Particle")]
        [SerializeField] private GameObject hitParticleParent;
        [SerializeField] private ParticleSystem hitParticleSystem;

        [Header("Others")]
        [SerializeField] private float dartDragForce = 0.0f;
        [SerializeField] private SpriteAnimator winAnimation;
        [SerializeField] private SpriteAnimator loseAnimation;
        [SerializeField] private SpriteAnimator boardAppearAnimation;

        private GeneralTimer genericTimer;

        public Action<string> setUserName;
        public Action<string, string> setUserImage;

        public Player PlayerType { get; set; }
        public CameraController CameraControllerRef { get => cameraController; }
        public bool GameIsSuspended { get => gameIsSuspended; }

        private Player lastPlayerType;
        private bool firstTime = false;

        private List<GameObject> fakeDarts = new List<GameObject>();

        private int userRoundCount = 0;
        private int opponentRoundCount = 0;

        private float gameStartTime = 0.0f;

        public PlayerWin PlayerWinScreen;
        public PlayerLoose PlayerLooseScreen;
        public PlayerDraw PlayerDrawScreen;
        public ShootingRange ShootingRangeScreen;
        public TrainingPlayerWin TrainingPlayerWinScreen;
        public OpponentSurrenderWindow OpponentSurrenderWindowScreen;
        public ReconnectCountdown ReconnectCountdownScreen;

        private bool haltProcess = false;
        private ThrowDartData throwDartData = null;

        public bool InternetConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override void Awake()
        {
            touchBehaviour = GetComponent<TouchBehaviour>();
        }

        private void Start()
        {
            SocketManager.Instance.iSocketStateSubscribers.Add(this);
            cameraController.SetCameraPosition(Player.Self);

            Input.multiTouchEnabled = false;
            listenSocketEvents();
            touchBehaviour.OnDartMove += DartMove;
            touchBehaviour.OnDartThrow += DartThrow;
            //UIManager.Instance.showProfilePic += SetUserProfileImage;
            ShootingRange.Instance.setUsersData += setAllUserData;
            genericTimer = new GeneralTimer(this, ConstantInteger.timerValue);

            boardAppearAnimation.gameObject.SetActive(false);
            winAnimation.onFinish.AddListener(onCompleteWinAnimation);
            loseAnimation.onFinish.AddListener(onCompleteLoseAnimation);
            boardAppearAnimation.onFinish.AddListener(onCompleteBoardAnim);

            startMainMenuBGMusic();
        }

        public void SetGameplayMode(EGamePlayMode a_GamePlayMode)
        {
            gamePlayMode = a_GamePlayMode;
        }

        public EGamePlayMode GetGameplayMode()
        {
            return gamePlayMode;
        }

        public void PlayBoardAnimation()
        {           
            dartBoard.SetActive(false);
            boardAppearAnimation.gameObject.SetActive(true);
            boardAppearAnimation.Play();
        }

        private void onCompleteBoardAnim()
        {
            dartBoard.SetActive(true);
            boardAppearAnimation.gameObject.SetActive(false);
        }

        public void StartTraining()
        {
            countdownTimerText.text = string.Empty;
            resetTimerImages();
            GetDartGameObj();

            onSwitchTurn(Player.None);
            cameraController.SetFocus(true);
            cameraController.SetCameraPosition(Player.Self);

            PlayBoardAnimation();

            UIManager.Instance.HideScreen(Page.UIPanel.ToString());
            UIManager.Instance.ShowScreen(Page.GameplayPanel.ToString(), Hide.none);
            UIManager.Instance.ShowScreen(Page.GameplayUIPanel.ToString(), Hide.none);

            setUserProfileImage(User.UserName, User.UserRace, User.UserColor);

            trainingScoreHandler.Initialize();
            clearOpponentData();

            if (trainingPopup != null)
            {
                trainingPopup.Show("Shooting Range", "Each round you have 3 tries to score as high as you can in order to lower your points down to 0. If you hit more than your remaining points, it’s a “Bust” and that round will not count. In order to decrease your points as fast as possible you can aim at the inner rings to receive double or triple points.",
                () =>
                {
                    InitializeOnGameStartSequences();
                    gameStatus = EGameStatus.Playing;
                    PlayerType = Player.Self;

                    StartCoroutine(onNextTurnTraining(2.0f));
                });
            }
        }

        public void StopTraining()
        {
            if (gamePlayMode == EGamePlayMode.Multiplayer)
                return;

            Debug.LogWarning("StopTraining");
            if (popup != null)
            {
                setCurrentDartActive(false);
                popup.Show("QUIT", "ARE YOU SURE?", OnLeaveTraining,
                delegate
                {
                    Debug.Log("Keep Training.");
                    setCurrentDartActive(true);
                });
            }
        }

        public void OnLeaveTraining()
        {
            Debug.LogWarning("LeaveTraining");
            cameraController.SetFocus(false);

            firstTime = true;
            genericTimer.StopTimer();
            genericTimer.ResetTimer();

            gameStatus = EGameStatus.None;

            if (currentDart != null)
                Destroy(currentDart.gameObject);

            resetTimerImages();
            displayPopup(true);
        }

        private void setCurrentDartActive(bool a_State)
        {
            if (currentDart != null)
                currentDart.gameObject.SetActive(a_State);
        }

        private void OnDestroy()
        {
            SocketManager.Instance.iSocketStateSubscribers.Remove(this);
        }

        private void getDartMaterialColorCode(string colorName)
        {

        }

        private void listenSocketEvents()
        {
            SocketListener.Listen(SocketListenEvents.userConnected.ToString(), onUserConnected);
            SocketListener.Listen(SocketListenEvents.nextTurn.ToString(), onNextTurn);
            SocketListener.Listen(SocketListenEvents.gameThrow.ToString(), onOpponentDartThrow);
            SocketListener.Listen(SocketListenEvents.gameOver.ToString(), onGameOver);
            SocketListener.Listen(SocketListenEvents.gameTimer.ToString(), onGameTimerEvent);
            SocketListener.Listen(SocketListenEvents.dartTimer.ToString(), onDartTimerEvent);
            SocketListener.Listen(SocketListenEvents.rejoinSuccess.ToString(), onRejoinSuccess);
            SocketListener.Listen(SocketListenEvents.rejoinFailure.ToString(), onRejoinFailure);
            SocketListener.Listen(SocketListenEvents.temporaryDisconnect.ToString(), onTemporaryDisconnect);
            SocketListener.Listen(SocketListenEvents.opponentReconnect.ToString(), onOpponentReconnect);
        }

        private void setDartColor(string colorName, GameObject dartGameObj)
        {
            // dartGameObj.GetComponent<MeshRenderer>().material.color = 
        }

        public void InitializeOnGameStartSequences()
        {
            genericTimer.IsTimerPaused = false;
            haltProcess = false;
            //if (currentDart != null)
            //    Destroy(currentDart);
            firstTime = true;
            clearFakeDarts();
            startGamplayBGMusic();
            //if (gamePlayMode == EGamePlayMode.Multiplayer)
            //    startGamplayBGMusic();
            //else if (gamePlayMode == EGamePlayMode.Training)
            //{
            //    stopMainMenuBGMusic();
            //    if (gameplayBGAudioPlayer != null)
            //    {
            //        gameplayBGAudioPlayer.Destroy();
            //        gameplayBGAudioPlayer = null;
            //    }
            //}
            gameStartTime = Time.time;
            resetTimerImages();
            gameRejoin = EGameRejoin.No;
        }

        private void resetTimerImages()
        {
            userTimerImage.fillAmount = 1.0f;
            opponentTimerImage.fillAmount = 1.0f;
        }

        private void setAllUserData(List<UserJoin> usersData)
        {
            for (int i = 0; i < usersData.Count; i++)
            {
                if (usersData[i].UserId.Equals(User.UserId))
                {
                    setUserProfileImage(usersData[i].UserName, usersData[i].RaceName, usersData[i].ColorName);
                }
                else if (!usersData[i].UserId.Equals(User.UserId))
                {
                    setOpponentProfileImage(usersData[i].UserName, usersData[i].RaceName, usersData[i].ColorName);
                }
            }
        }

        private void setUserProfileImage(string name, string race, string color)
        {
            userName.text = name;
            ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), race);
            EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), color);

            CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
            if (t_CharacterPicData != null)
            {
                userPic.material.SetTexture("_BaseMap", (Texture)t_CharacterPicData.ProfilePic.texture);
            }
        }

        private void setOpponentProfileImage(string name, string race, string color)
        {
            opponentName.text = name;
            ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), race);
            EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), color);

            CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
            if (t_CharacterPicData != null)
            {
                opponentPic.material.SetTexture("_BaseMap", (Texture)t_CharacterPicData.ProfilePic.texture);
            }
        }

        private void clearUserData()
        {
            userName.text = string.Empty;
            userScore.text = string.Empty;
            userTimerImage.fillAmount = 1.0f;
            userPic.material.SetTexture("_BaseMap", null);
        }

        private void clearOpponentData()
        {
            opponentName.text = string.Empty;
            opponentScore.text = string.Empty;
            opponentTimerImage.fillAmount = 1.0f;
            opponentPic.material.SetTexture("_BaseMap", null);
        }

        private void onSwitchTurn(Player a_PlayerType)
        {
            switch (a_PlayerType)
            {
                case Player.Self:
                    userPic.material.SetInt("_Glow", 1);
                    opponentPic.material.SetInt("_Glow", 0);
                    break;
                case Player.Opponent:
                    userPic.material.SetInt("_Glow", 0);
                    opponentPic.material.SetInt("_Glow", 1);
                    break;
                case Player.None:
                    userPic.material.SetInt("_Glow", 0);
                    opponentPic.material.SetInt("_Glow", 0);
                    break;
            }
        }

        public void GetDartGameObj()
        {
            string userDartPath = GameResources.dartPrefabFolderPath + "/" + User.DartName;
            string opponentDartPath = GameResources.dartPrefabFolderPath + "/" + Opponent.dartName;
            userDart = Resources.Load<GameObject>(userDartPath);
            opponentDart = Resources.Load<GameObject>(opponentDartPath);
        }

        private void Update()
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            if (genericTimer != null && !haltProcess)
            {
                if (playerTurn && PlayerType == Player.Self)
                {
                    userTimerImage.fillAmount = genericTimer.RemainingTime / ConstantInteger.timerValue;
                }
                if (playerTurn && PlayerType == Player.Opponent)
                {
                    opponentTimerImage.fillAmount = genericTimer.RemainingTime / ConstantInteger.timerValue;
                }
            }
        }

        private void resetTimerRelatedValues()
        {
            timer = ConstantInteger.timerValue;
            userTimerImage.fillAmount = genericTimer.SlowTimeFactor;
            opponentTimerImage.fillAmount = genericTimer.SlowTimeFactor;
            playerTurn = false;
        }

        public void LeaveRoom()
        {
            if (gamePlayMode == EGamePlayMode.Training)
                return;

            Debug.LogWarning("LeaveRoom");
            if (popup != null)
            {
                setCurrentDartActive(false);
                popup.Show("QUIT", "ARE YOU SURE?", onLeaveRoom,
                delegate
                {
                    Debug.Log("Keep Playing.");
                    setCurrentDartActive(true);
                });
            }
        }

        private void onLeaveRoom()
        {
            gameStatus = EGameStatus.None;
            firstTime = true;
            if (currentDart != null)
                Destroy(currentDart.gameObject);
            clearFakeDarts();
            stopGamplayBGMusic();

            Debug.Log("---------------Leaving Room---------------");
            SocketManager.Instance.LeaveRoomRequest();

            cameraController.SetCameraPosition(Player.Self);
            countdownTimer.StopCountdown();
            stopCountdownMusic();
            genericTimer.StopTimer();
            genericTimer.ResetTimer();
            ShootingRangeScreen.Refresh();
            onSwitchTurn(Player.None);

            RestManager.GetLastGameHistory(User.UserEmailId, OnGetLastGameHistorySuccess, OnRequestFailure);

            //float t_CurrentTime = Time.time;
            //if (t_CurrentTime - gameStartTime <= 5.0f)
            //{
            //    onOpenWinloosePopUp(Page.DrawMatchPanel, User.UserName, User.UserRace, User.UserColor);
            //    PlayerDrawScreen.Refresh();
            //}
            //else
            //{
            //    displayPopup(false);
            //}
        }

        private void OnGetLastGameHistorySuccess(LastGameHistory a_GameHistoryMatchDetails)
        {
            Debug.Log($"------------------------------------LGHD: {JsonConvert.SerializeObject(a_GameHistoryMatchDetails)}");
            if (a_GameHistoryMatchDetails != null)
            {
                if (a_GameHistoryMatchDetails.gameHistoryUserDatas.GameResult == 0)
                {
                    // Lose
                    DisplayAppropriateGameOverScreen("Lose", a_GameHistoryMatchDetails.gameHistoryUserDatas.CupNumber, a_GameHistoryMatchDetails.gameHistoryUserDatas.TotalCupWin, 1);
                }
                else if (a_GameHistoryMatchDetails.gameHistoryUserDatas.GameResult == 1)
                {
                    // Win
                    DisplayAppropriateGameOverScreen("Win", a_GameHistoryMatchDetails.gameHistoryUserDatas.CupNumber, a_GameHistoryMatchDetails.gameHistoryUserDatas.TotalCupWin, 1);
                }
                else if (a_GameHistoryMatchDetails.gameHistoryUserDatas.GameResult == 2)
                {
                    // Draw
                    DisplayAppropriateGameOverScreen("Draw", a_GameHistoryMatchDetails.gameHistoryUserDatas.CupNumber, a_GameHistoryMatchDetails.gameHistoryUserDatas.TotalCupWin, 1);
                }
            }
        }

        private void OnRequestFailure(RestUtil.RestCallError obj)
        {
            Debug.Log("Get Data Error: " + obj.Description);
        }

        private void onGameOver(string data)
        {
            StopRecconectCountdown();
            gameStatus = EGameStatus.None;
            firstTime = true;
            if (currentDart != null)
                Destroy(currentDart.gameObject);
            clearFakeDarts();
            stopGamplayBGMusic();

            cameraController.SetCameraPosition(Player.Self);
            countdownTimer.StopCountdown();
            stopCountdownMusic();
            genericTimer.StopTimer();
            genericTimer.ResetTimer();
            ShootingRangeScreen.Refresh();
            Debug.Log($"OnGameOver : {data}" + "  User Id: " + User.UserId);
            var gameOverData = DataConverter.DeserializeObject<ApiResponseFormat<GameOverResponse>>(data);

            if (gameOverData.Result.FirstUserId == User.UserId)
            {
                DisplayAppropriateGameOverScreen(gameOverData.Result.FirstUserGameStatus, gameOverData.Result.FirstUserCupNumber, gameOverData.Result.FirstUserTotalCup, gameOverData.Result.CompleteStatus);
            }
            else if (gameOverData.Result.SecondUserId == User.UserId)
            {
                DisplayAppropriateGameOverScreen(gameOverData.Result.SecondUserGameStatus, gameOverData.Result.SecondUserCupNumber, gameOverData.Result.SecondUserTotalCup, gameOverData.Result.CompleteStatus);
            }
            onSwitchTurn(Player.None);
        }

        private void DisplayAppropriateGameOverScreen(string a_GameOverStatus, int a_MatchCup, int a_TotalCup, int a_CompleteStatus)
        {
            if (a_GameOverStatus == EGameOverStatus.Win.ToString())
            {
                PlayerWinScreen.Refresh(a_MatchCup, a_TotalCup);
                if (a_CompleteStatus == 1)
                    displayPopup(true);
                else
                {
                    UIManager.Instance.ShowScreen(Page.SurrenderedPopupPanel.ToString());
                    OpponentSurrenderWindowScreen.UpdateInfo(opponentName.text, () => displayPopup(true));
                }
            }
            else if (a_GameOverStatus == EGameOverStatus.Lose.ToString())
            {
                PlayerLooseScreen.Refresh(a_MatchCup, a_TotalCup);
                displayPopup(false);
            }
            else if (a_GameOverStatus == EGameOverStatus.Draw.ToString())
            {
                PlayerDrawScreen.Refresh(a_TotalCup);
                onOpenWinloosePopUp(Page.DrawMatchPanel, User.UserName, User.UserRace, User.UserColor);
            }
        }

        private void startMainMenuBGMusic()
        {
            mainMenuBGAudioPlayer = AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.MainMenu).Clip, loop = true, volume = SettingData.BGMVolume });
        }

        private void stopMainMenuBGMusic()
        {
            if (mainMenuBGAudioPlayer != null)
            {
                mainMenuBGAudioPlayer.Destroy();
                mainMenuBGAudioPlayer = null;
            }
        }

        private void startGamplayBGMusic()
        {
            stopMainMenuBGMusic();
            gameplayBGAudioPlayer = AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.GameplayBg).Clip, loop = true, volume = SettingData.BGMVolume });
        }

        private void stopGamplayBGMusic()
        {
            if (gameplayBGAudioPlayer != null)
            {
                gameplayBGAudioPlayer.Destroy();
                gameplayBGAudioPlayer = null;
            }
            startMainMenuBGMusic();
        }

        private void startCountdownMusic()
        {
            countdownAudioPlayer = AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.Countdown).Clip, loop = true, volume = SettingData.SFXVolume });
        }

        private void stopCountdownMusic()
        {
            if (countdownAudioPlayer != null)
            {
                countdownAudioPlayer.Destroy();
                countdownAudioPlayer = null;
            }
        }

        public void MainMenuBGMusicVolume(float a_Volume)
        {
            if (mainMenuBGAudioPlayer != null)
                mainMenuBGAudioPlayer.GetAudioSource().volume = a_Volume;
        }

        public void GameplayBGMusicVolume(float a_Volume)
        {
            if (gameplayBGAudioPlayer != null)
                gameplayBGAudioPlayer.GetAudioSource().volume = a_Volume;
        }

        private void onOpenWinloosePopUp(Page popUpName, string userName, string race, string color)
        {
            Debug.Log($"OnEndGame PageName: {popUpName.ToString()}");
            cameraController.SetFocus(false);
            //UIManager.Instance.ShowUiPanel(true);
            UIManager.Instance.ShowScreen(Page.UIPanel.ToString());
            if (gamePlayMode == EGamePlayMode.Training)
            {
                UIManager.Instance.HideScreenImmediately(Page.TopAndBottomBarPanel.ToString());
                UIManager.Instance.HideScreenImmediately(Page.CharacterSelectionPanel.ToString());
            }
            //UIManager.Instance.ShowScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ShowScreen(popUpName.ToString(),Hide.previous);
            UIManager.Instance.ClearOpenPagesContainer();
            setUserName?.Invoke(userName);
            setUserImage?.Invoke(race, color);

            gameStatus = EGameStatus.None;
            clearFakeDarts();
            stopGamplayBGMusic();
            if (currentDart != null)
                Destroy(currentDart.gameObject);
        }

        private void onCompleteWinAnimation()
        {
            winPopupBG.SetActive(false);
            if (gamePlayMode == EGamePlayMode.Multiplayer)
                onOpenWinloosePopUp(Page.PlayerWinPanel, User.UserName, User.UserRace, User.UserColor);
            else if (gamePlayMode == EGamePlayMode.Training)
                onOpenWinloosePopUp(Page.TrainingPlayerWinPanel, User.UserName, User.UserRace, User.UserColor);
            //AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.GameWin).Clip, oneShot = true, volume = SettingData.SFXVolume });

            UIManager.Instance.HideScreenImmediately(Page.GameplayPanel.ToString());
            UIManager.Instance.HideScreenImmediately(Page.GameplayUIPanel.ToString());
            cameraController.SetFocus(false);

            if (gamePlayMode == EGamePlayMode.Multiplayer)
            {
                PlayerWinScreen.Refresh();
                ShootingRangeScreen.Refresh();
            }
            else if (gamePlayMode == EGamePlayMode.Training)
            {
                TrainingPlayerWinScreen.Refresh();
            }
        }

        private void onCompleteLoseAnimation()
        {
            loosePopupBG.SetActive(false);
            onOpenWinloosePopUp(Page.PlayerLoosePanel, User.UserName, User.UserRace, User.UserColor);
            //AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.GameLose).Clip, oneShot = true, volume = SettingData.SFXVolume });

            UIManager.Instance.HideScreenImmediately(Page.GameplayPanel.ToString());
            UIManager.Instance.HideScreenImmediately(Page.GameplayUIPanel.ToString());
            cameraController.SetFocus(false);

            PlayerLooseScreen.Refresh();
            ShootingRangeScreen.Refresh();
        }

        private void displayPopup(bool a_Won)
        {
            if (a_Won)
            {
                //loosePopup.transform.localScale = Vector3.zero;
                loosePopupBG.SetActive(false);

                //winPopup.transform.localScale = Vector3.zero;
                winPopupBG.SetActive(true);
                winAnimation.Play();
                AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.GameWin).Clip, oneShot = true, volume = SettingData.SFXVolume });
                //Sequence t_Sequence = DOTween.Sequence();
                //t_Sequence.Append(winPopup.transform.DOScale(winnerPopupOriginalScale, 1.0f).SetEase(Ease.InBounce));
                //t_Sequence.AppendInterval(1.0f);
                //t_Sequence.AppendCallback(() =>
                //{
                //    winPopupBG.SetActive(false);
                //    if (gamePlayMode == EGamePlayMode.Multiplayer)
                //        onOpenWinloosePopUp(Page.PlayerWinPanel, User.UserName, User.UserRace, User.UserColor);
                //    else if (gamePlayMode == EGamePlayMode.Training)
                //        onOpenWinloosePopUp(Page.TrainingPlayerWinPanel, User.UserName, User.UserRace, User.UserColor);
                //    //AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.GameWin).Clip, oneShot = true, volume = SettingData.SFXVolume });

                //    UIManager.Instance.HideScreenImmediately(Page.GameplayPanel.ToString());
                //    UIManager.Instance.HideScreenImmediately(Page.GameplayUIPanel.ToString());
                //    cameraController.SetFocus(false);

                //    if (gamePlayMode == EGamePlayMode.Multiplayer)
                //    {
                //        PlayerWinScreen.Refresh();
                //        ShootingRangeScreen.Refresh();
                //    }
                //    else if (gamePlayMode == EGamePlayMode.Training)
                //    {
                //        TrainingPlayerWinScreen.Refresh();
                //    }
                //});
                //t_Sequence.Play();
            }
            else
            {
                //winPopup.transform.localScale = Vector3.zero;
                winPopupBG.SetActive(false);

                //loosePopup.transform.localScale = Vector3.zero;
                loosePopupBG.SetActive(true);
                loseAnimation.Play();
                AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.GameLose).Clip, oneShot = true, volume = SettingData.SFXVolume });
                //Sequence t_Sequence = DOTween.Sequence();
                //t_Sequence.Append(loosePopup.transform.DOScale(winnerPopupOriginalScale, 1.0f).SetEase(Ease.InBounce));
                //t_Sequence.AppendInterval(1.0f);
                //t_Sequence.AppendCallback(() =>
                //{
                //    loosePopupBG.SetActive(false);
                //    onOpenWinloosePopUp(Page.PlayerLoosePanel, User.UserName, User.UserRace, User.UserColor);
                //    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.GameLose).Clip, oneShot = true, volume = SettingData.SFXVolume });

                //    UIManager.Instance.HideScreenImmediately(Page.GameplayPanel.ToString());
                //    UIManager.Instance.HideScreenImmediately(Page.GameplayUIPanel.ToString());
                //    cameraController.SetFocus(false);

                //    PlayerLooseScreen.Refresh();
                //    ShootingRangeScreen.Refresh();
                //});
                //t_Sequence.Play();
            }
            resetTimerImages();
        }

        private void onGameTimerEvent(string a_Data)
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            Debug.Log($"GameTimer Data : {a_Data}");
            var t_GameTimerData = DataConverter.DeserializeObject<ApiResponseFormat<GameTimerData>>(a_Data);
            if (t_GameTimerData != null && t_GameTimerData.Status == 1)
            {
                if (t_GameTimerData.Result.TotalGameTime > 0 && t_GameTimerData.Result.GameFinish == 0)
                {
                    countdownTimer.StartCountdown(t_GameTimerData.Result.TotalGameTime, false, (t) => countdownTimerText.text = t, () =>
                    {
                        countdownTimerText.DOKill();
                        countdownTimerText.color = Color.yellow;
                    });
                }
                if (t_GameTimerData.Result.GameFinish == 1)
                {
                    countdownTimerText.DOColor(Color.red, 1.0f).SetLoops(-1, DG.Tweening.LoopType.Yoyo);
                    startCountdownMusic();
                }
            }
        }

        private void onDartTimerEvent(string a_Data)
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            Debug.Log($"DartTimer Data : {a_Data}");
        }

        public void OnTrainingTurnComplete()
        {
            StartCoroutine(OnTrainingTurnCompleteTask());
        }

        private IEnumerator OnTrainingTurnCompleteTask()
        {
            yield return new WaitForSeconds(2.0f);
            clearFakeDarts();
        }

        private IEnumerator onNextTurnTraining(float a_Time)
        {
            yield return new WaitForSeconds(a_Time);
            if (gameStatus != EGameStatus.Playing)
                yield break;

            if (currentDart != null)
                Destroy(currentDart.gameObject);

            resetTimerRelatedValues();
            genericTimer.StopTimer();

            playerTurn = true;
            InstantiateDart(userDart);
            
            touchBehaviour.IsShooted = false;
            genericTimer.StartTimer(onPlayerTimerCompleteTraining);
        }

        private void onNextTurn(string data)
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            resetTimerRelatedValues();
            Debug.Log($"onNextTurn : {data}" + "  User Id: " + User.UserId);
            var nextTurnData = DataConverter.DeserializeObject<ApiResponseFormat<NextTurn>>(data);
            playerTurn = true;
            if (nextTurnData.Result.UserId == User.UserId)
            {
                cameraController.SetCameraPosition(Player.Self);
                InstantiateDart(userDart);
                PlayerType = Player.Self;
                touchBehaviour.IsShooted = false;
                genericTimer.StartTimer(onPlayerTimerComplete);
                onSwitchTurn(PlayerType);
            }
            else
            {
                cameraController.SetCameraPosition(Player.Opponent);
                InstantiateDart(opponentDart);
                PlayerType = Player.Opponent;
                touchBehaviour.IsShooted = true;
                genericTimer.StartTimer(onOponnetTimerComplete);
                onSwitchTurn(PlayerType);
            }

            if (firstTime)
            {
                firstTime = false;
                lastPlayerType = PlayerType;

                if (PlayerType == Player.Self)
                    userRoundCount = 3;
                else if (PlayerType == Player.Opponent)
                    opponentRoundCount = 3;
            }
            else
            {
                if (lastPlayerType != PlayerType)
                {
                    if (PlayerType == Player.Self)
                        userRoundCount = 3;
                    else if (PlayerType == Player.Opponent)
                        opponentRoundCount = 3;

                    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.WindowChange).Clip, oneShot = true, volume = SettingData.SFXVolume });
                    lastPlayerType = PlayerType;
                    clearFakeDarts();
                }
            }
        }

        private void ThrowDartAfterRejoin(bool firstDartOfTurn)
        {
            resetTimerRelatedValues();
            playerTurn = true;
            cameraController.SetCameraPosition(Player.Self);
            InstantiateDart(userDart);
            PlayerType = Player.Self;
            touchBehaviour.IsShooted = false;
            genericTimer.StartTimer(onPlayerTimerComplete);
            onSwitchTurn(PlayerType);

            if (firstDartOfTurn)
            {
                userRoundCount = 3;
                AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.WindowChange).Clip, oneShot = true, volume = SettingData.SFXVolume });
                lastPlayerType = PlayerType;
                clearFakeDarts();
            }
        }

        private void onOponnetTimerComplete()
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            Debug.Log("On Opponent timer Complete");

            if (currentDart != null)
                Destroy(currentDart.gameObject);

            alarmClock.Show();
            resetTimerRelatedValues();
            genericTimer.StopTimer();
        }

        private void onPlayerTimerComplete()
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            // Call Drat Throw Event WIth ZERO values
            Debug.Log("On Player Timer complete");

            if (currentDart != null)
                Destroy(currentDart.gameObject);

            alarmClock.Show();

            if (SocketManager.socket.IsConnected)
                SocketManager.Instance.ThrowDartData(0, ConstantStrings.turnCancelled);
            else
                throwDartData = new ThrowDartData() { TurnCanceled = true };

            resetTimerRelatedValues();
            genericTimer.StopTimer();
        }

        private void onPlayerTimerCompleteTraining()
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            Debug.Log("On Player Timer complete");

            if (currentDart != null)
                Destroy(currentDart.gameObject);

            trainingScoreHandler.AddHitPoint(0, 0);
            alarmClock.Show();
            StartCoroutine(onNextTurnTraining(2.5f));
        }

        private void onOpponentDartThrow(string data)
        {
            if (gameStatus != EGameStatus.Playing)
                return;
            throwDartData = null;
            Debug.Log($"onOpponentDartThrow : {data}");
            var dartThrowData = DataConverter.DeserializeObject<ApiResponseFormat<DartThrow>>(data);
            if(dartThrowData.Result.UserId != User.UserId)
            {
                if (!dartThrowData.Result.DartPoint.Contains(ConstantStrings.turnCancelled))
                {
                    Debug.Log("Opponent Hit Score: " + int.Parse(dartThrowData.Result.PlayerScore));
                    if (int.Parse(dartThrowData.Result.PlayStatus) == 0)
                    {
                        int t_TotalValue = dartThrowData.Result.HitScore;
                        if (dartThrowData.Result.ScoreMultiplier > 1)
                            t_TotalValue = dartThrowData.Result.HitScore * dartThrowData.Result.ScoreMultiplier;
                        StartCoroutine(displayOpponentScoreWithDelay(dartThrowData.Result.HitScore, dartThrowData.Result.ScoreMultiplier, t_TotalValue, dartThrowData.Result.RemainingScore, false));
                    }
                    else
                    {
                        StartCoroutine(displayOpponentScoreWithDelay(0, 0, dartThrowData.Result.RoundScore, dartThrowData.Result.RemainingScore, true));
                    }
                    Vector3 opponenDartHitPoint = getValue(dartThrowData.Result.DartPoint);
                    DartThrow(opponenDartHitPoint, ConstantInteger.shootingAngle);
                }
                else
                {
                    StartCoroutine(displayOpponentScoreWithDelay(0, 0, 0, dartThrowData.Result.RemainingScore, false));
                }
            }
            else if (dartThrowData.Result.UserId == User.UserId)
            {
                if (!dartThrowData.Result.DartPoint.Contains(ConstantStrings.turnCancelled))
                {
                    if (int.Parse(dartThrowData.Result.PlayStatus) == 0)
                    {
                        if (dartThrowData.Result.ScoreMultiplier > 1)
                        {
                            scoreGraphic.ShowScore(dartThrowData.Result.HitScore, dartThrowData.Result.ScoreMultiplier);
                            int t_TotalValue = dartThrowData.Result.HitScore * dartThrowData.Result.ScoreMultiplier;
                            //scoreGraphic.ShowScore(t_TotalValue, 0);
                            scoreGraphic.ShowScore(t_TotalValue, 0, ScoreGraphic.EMoveTowards.User, false, () => scoreHandler.UpdateScoreText(Player.Self, dartThrowData.Result.RemainingScore, true));
                        }
                        else
                            scoreGraphic.ShowScore(dartThrowData.Result.HitScore, 0, ScoreGraphic.EMoveTowards.User, false, () => scoreHandler.UpdateScoreText(Player.Self, dartThrowData.Result.RemainingScore, true));

                        postDartThrowAction(Player.Self, false);
                    }
                    else
                    {
                        //scoreGraphic.ShowScore(dartThrowData.Result.RoundScore, 0, ScoreGraphic.EMoveTowards.None, true);
                        int t_TotalValue = dartThrowData.Result.HitScore;
                        if (dartThrowData.Result.ScoreMultiplier > 1)
                            t_TotalValue = dartThrowData.Result.HitScore * dartThrowData.Result.ScoreMultiplier;
                        scoreGraphic.ShowScore(t_TotalValue, 0, ScoreGraphic.EMoveTowards.None, true, () => scoreHandler.UpdateScoreText(Player.Self, dartThrowData.Result.RemainingScore, true));
                        postDartThrowAction(Player.Self, true);
                    }
                }
                else
                {
                    //scoreGraphic.ShowScore(0, 0);
                    scoreGraphic.ShowScore(0, 0, ScoreGraphic.EMoveTowards.User, false, () => scoreHandler.UpdateScoreText(Player.Self, dartThrowData.Result.RemainingScore, true));
                    postDartThrowAction(Player.Self, false);
                }
            }
        }

        private void postDartThrowAction(Player a_Player, bool a_IsBust)
        {
            if (a_Player == Player.Self)
            {
                if (userRoundCount > 0)
                {
                    userRoundCount--;
                    if (userRoundCount == 0)
                    {
                        if (!a_IsBust)
                        {
                            AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.AudienceCheering).Clip, oneShot = true, volume = SettingData.SFXVolume });
                            //scoreGraphic.ShowScore(scoreHandler.SelfScoreData.ActiveRoundScore, 0, ScoreGraphic.EMoveTowards.User, false, () => scoreHandler.UpdateScoreText(Player.Self));
                        }
                    }
                }
            }
            else if (a_Player == Player.Opponent)
            {
                if (opponentRoundCount > 0)
                {
                    opponentRoundCount--;
                    if (opponentRoundCount == 0)
                    {
                        if (!a_IsBust)
                        {
                            AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.AudienceCheering).Clip, oneShot = true, volume = SettingData.SFXVolume });
                            //scoreGraphic.ShowScore(scoreHandler.OpponentScoreData.ActiveRoundScore, 0, ScoreGraphic.EMoveTowards.Opponent, false, ()=> scoreHandler.UpdateScoreText(Player.Opponent));
                        }
                    }
                }
            }
        }

        private IEnumerator displayOpponentScoreWithDelay(int a_HitValue, int a_MultiplierValue, int a_TotalValue, int a_RemainingScore, bool a_IsBust)
        {
            if (gameStatus != EGameStatus.Playing)
                yield break;
            yield return new WaitForSeconds(1.0f);

            if (!a_IsBust)
            {
                if (a_MultiplierValue > 1)
                {
                    scoreGraphic.ShowScore(a_HitValue, a_MultiplierValue);
                    scoreGraphic.ShowScore(a_TotalValue, 0, ScoreGraphic.EMoveTowards.Opponent, false, () => scoreHandler.UpdateScoreText(Player.Opponent, a_RemainingScore, true));
                }
                else
                    scoreGraphic.ShowScore(a_HitValue, 0, ScoreGraphic.EMoveTowards.Opponent, false, () => scoreHandler.UpdateScoreText(Player.Opponent, a_RemainingScore, true));

                postDartThrowAction(Player.Opponent, false);
            }
            else
            {
                scoreGraphic.ShowScore(a_TotalValue, 0, ScoreGraphic.EMoveTowards.None, true, () => scoreHandler.UpdateScoreText(Player.Opponent, a_RemainingScore, true));
                postDartThrowAction(Player.Opponent, true);
            }
        }

        public void OnCompletionDartHit(GameObject dartGameObj)
        {
            if (gameStatus != EGameStatus.Playing)
                return;
            if (gamePlayMode == EGamePlayMode.Training)
                onCompletionDartHitTraining(dartGameObj);
            else if (gamePlayMode == EGamePlayMode.Multiplayer)
            {
                Debug.Log("OnCompletionDartHit");
                StartCoroutine(destroyDartAfterACertainTime(1, dartGameObj));
                InstantiateFakeDart(dartGameObj);
                if (PlayerType == Player.Self)
                {
                    Debug.Log("Player...");

                    BoardBodyPart boardBody = touchBehaviour.DartHitGameObj.GetComponent<BoardBodyPart>();
                    if (boardBody.name.Contains("Outside"))
                        AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.DartMiss).Clip, oneShot = true, volume = SettingData.SFXVolume });

                    int t_TotalValue = boardBody.HitPointScore;
                    if (boardBody.ScoreMultiplier > 1)
                        t_TotalValue = boardBody.HitPointScore * boardBody.ScoreMultiplier;

                    if (SocketManager.socket.IsConnected)
                        SocketManager.Instance.ThrowDartData(t_TotalValue, boardBody.HitPointScore, boardBody.ScoreMultiplier, touchBehaviour.LastTouchPosition);
                    else
                        throwDartData = new ThrowDartData() { TotalValue = t_TotalValue, HitPointScore = boardBody.HitPointScore, ScoreMultiplier = boardBody.ScoreMultiplier, HitPosition = touchBehaviour.LastTouchPosition, TurnCanceled = false }; 
                }

                PlayDartHitParticle(dartGameObj.transform.position);
            }
        }

        private void onCompletionDartHitTraining(GameObject dartGameObj)
        {
            Debug.Log("onCompletionDartHitTraining");
            InstantiateFakeDart(dartGameObj);

            BoardBodyPart boardBody = touchBehaviour.DartHitGameObj.GetComponent<BoardBodyPart>();
            if (boardBody.name.Contains("Outside"))
            {
                AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.DartMiss).Clip, oneShot = true, volume = SettingData.SFXVolume });
                trainingScoreHandler.AddHitPoint(0, 0);
            }
            else
            {
                trainingScoreHandler.AddHitPoint(boardBody.HitPointScore, boardBody.ScoreMultiplier);
            }

            PlayDartHitParticle(dartGameObj.transform.position);
            StartCoroutine(onNextTurnTraining(2.0f));
        }

        private void InstantiateFakeDart(GameObject dartGameObj)
        {
            Debug.Log("Instantiate FakeDart");
            GameObject t_Go = Instantiate(dartGameObj, dartGameObj.transform.position, dartGameObj.transform.rotation);
            t_Go.transform.DOKill();
            Dart t_Dart = t_Go.GetComponent<Dart>();
            if (t_Dart != null)
                Destroy(t_Dart);
            fakeDarts.Add(t_Go);
        }

        private void InstantiateFakeDart(GameObject dartGameObj, Vector3 position)
        {
            Debug.Log("Instantiate FakeDart");
            GameObject t_Go = Instantiate(dartGameObj, position, dartGameObj.transform.rotation);
            t_Go.transform.DOKill();
            Dart t_Dart = t_Go.GetComponent<Dart>();
            if (t_Dart != null)
                Destroy(t_Dart);
            fakeDarts.Add(t_Go);
        }

        private void clearFakeDarts()
        {
            if (fakeDarts.Any())
            {
                fakeDarts.ForEach(x => Destroy(x));
                fakeDarts.Clear();
            }
        }

        private void onUserConnected(string obj)
        {
            if (gameRejoin == EGameRejoin.Yes)
            {
                SocketManager.Instance.UpdateOldSocketId();
                SocketManager.Instance.ReJoinRequest();
            }
        }

        private void doReJoinRequest()
        {
            if (SocketManager.Instance.SocketIdsMatching())
            {
                SocketManager.Instance.ReJoinRequest();
            }
            else
            {
                gameRejoin = EGameRejoin.Yes;
                SocketManager.Instance.AddUser();
            }
        }

        private void onRejoinSuccess(string a_Data)
        {
            gameRejoin = EGameRejoin.No;
            var t_ApiResponseFormatData = DataConverter.DeserializeObject<ApiResponseFormat<RejoinSuccessResponseData>>(a_Data);
            RejoinSuccessResponseData t_RejoinSuccessResponseData = t_ApiResponseFormatData.Result;
            countdownTimer.UpdateTime(t_RejoinSuccessResponseData.GameRemainingTime);

            if (t_RejoinSuccessResponseData.CurrentUserTurn == User.UserId)
            {
                if (t_RejoinSuccessResponseData.TurnStatus == 1)
                {
                    if (throwDartData != null)
                    {
                        if (throwDartData.TurnCanceled)
                            SocketManager.Instance.ThrowDartData(0, ConstantStrings.turnCancelled);
                        else
                            SocketManager.Instance.ThrowDartData(throwDartData.TotalValue, throwDartData.HitPointScore, throwDartData.ScoreMultiplier, throwDartData.HitPosition);
                    }
                    else
                    {
                        ThrowDartAfterRejoin(false);
                    }
                }
                else
                {
                    scoreHandler.UpdateScoreText(Player.Opponent, int.Parse(t_RejoinSuccessResponseData.SecondUserLastDetails.Last().TotalScore), false);
                    ThrowDartAfterRejoin(true);
                }
            }
            else
            {
                if (t_RejoinSuccessResponseData.SecondUserLastDetails.Any())
                {
                    t_RejoinSuccessResponseData.SecondUserLastDetails.ForEach(x => InstantiateFakeDart(opponentDart, getValue(x.DartPoint)));
                    opponentRoundCount = 3 - t_RejoinSuccessResponseData.SecondUserLastDetails.Count;
                    if (t_RejoinSuccessResponseData.SecondUserLastDetails.Count == 3)
                    {
                        scoreHandler.UpdateScoreText(Player.Opponent, int.Parse(t_RejoinSuccessResponseData.SecondUserLastDetails.Last().TotalScore), false);
                    }
                    else
                    {
                        scoreHandler.UpdateScoreText(Player.Opponent, int.Parse(t_RejoinSuccessResponseData.SecondUserLastDetails.Last().TotalScore), false);
                    }
                }
            }
        }

        private void onRejoinFailure(string a_Data)
        {
            gameRejoin = EGameRejoin.No;
            var t_ApiResponseFormatData = DataConverter.DeserializeObject<ApiResponseFormat<RejoinFailureResponseData>>(a_Data);
            RejoinFailureResponseData t_RejoinFailureResponseData = t_ApiResponseFormatData.Result;
            RejoinResponseData t_RejoinResponseData = t_RejoinFailureResponseData.UserLastDetails.Where(x => x.UserId == User.UserId).FirstOrDefault();

            // TODO Gameover
            gameStatus = EGameStatus.None;
            firstTime = true;
            if (currentDart != null)
                Destroy(currentDart.gameObject);
            clearFakeDarts();
            stopGamplayBGMusic();

            cameraController.SetCameraPosition(Player.Self);
            countdownTimer.StopCountdown();
            stopCountdownMusic();
            genericTimer.StopTimer();
            genericTimer.ResetTimer();

            PlayerLooseScreen.Refresh(t_RejoinResponseData.CupNumber, t_RejoinResponseData.TotalCupWin);

            displayPopup(false);
            onSwitchTurn(Player.None);
        }

        private void onTemporaryDisconnect(string a_Data)
        {
            var t_ApiResponseFormatData = DataConverter.DeserializeObject<ApiResponseFormat<TemporarilyDisconnectData>>(a_Data);
            TemporarilyDisconnectData t_TemporarilyDisconnectData = t_ApiResponseFormatData.Result;
            if (t_TemporarilyDisconnectData.UserId != User.UserId)
            {
                genericTimer.IsTimerPaused = true;
                haltProcess = true;
                UIManager.Instance.ShowScreen(Page.ReconnectCountdownPanel.ToString());
                //ReconnectCountdownScreen.StartCountdown();
            }
        }

        private void onOpponentReconnect(string a_Data)
        {
            var t_ApiResponseFormatData = DataConverter.DeserializeObject<ApiResponseFormat<OpponentReconnectData>>(a_Data);
            OpponentReconnectData t_OpponentReconnectData = t_ApiResponseFormatData.Result;
            if (t_OpponentReconnectData.UserId != User.UserId)
            {
                genericTimer.IsTimerPaused = false;
                StopRecconectCountdown();
            }
        }

        private void StopRecconectCountdown()
        {
            //ReconnectCountdownScreen.StopCountdown();
            UIManager.Instance.HideScreenImmediately(Page.ReconnectCountdownPanel.ToString());
            haltProcess = false;
        }

        private Vector3 getValue(string vectorValue)
        {
            vectorValue = vectorValue.Substring(1, vectorValue.Length - 2);
            string[] splittedString = vectorValue.Split(',');
            Vector3 newVector = new Vector3(float.Parse(splittedString[0]), float.Parse(splittedString[1]), float.Parse(splittedString[2]));
            return newVector;
        }

        public void ResetScore()
        {
            if (currentDart != null)
                Destroy(currentDart.gameObject);

            gameStatus = EGameStatus.Playing;
            countdownTimerText.text = "00:00";
            countdownTimerText.color = Color.yellow;

            userRoundCount = 0;
            opponentRoundCount = 0;

            scoreHandler.Initialize(ScoreData.requiredScore);
        }

        Vector3 newPosition = new Vector3(0, 0, 0);
        Vector3 currentPosition = new Vector3(0, 0, 0);
        private void DartMove(Vector3 dartPosition)
        {
            if (currentDart != null)
            {
                newPosition = dartPosition;
                currentPosition = Vector3.MoveTowards(currentPosition, newPosition, dartDragForce * Time.deltaTime);
                currentDart.transform.position = currentPosition;
            }
        }

        public void InstantiateDart(GameObject dartGameObj)
        {
            if (currentDart != null)
                Destroy(currentDart.gameObject);

            currentDart = Instantiate(dartGameObj, Vector3.zero, Quaternion.identity).GetComponent<Dart>();
            //currentDart.transform.localScale = Vector3.zero;
            //currentDart.transform.DOScale(Vector3.one, 1.0f).SetEase(Ease.InBounce);
            currentDart.DoFlash();
            Debug.Log("Current Dart: " + currentDart.name);
        }

        private IEnumerator destroyDartAfterACertainTime(float seconds,GameObject dartObj)
        {
            Debug.Log("Destroying... "+dartObj.name);
            yield return new WaitForSeconds(seconds);
            Destroy(dartObj);
            Debug.Log("Destroyed Game Obj");
        }

        private void DartThrow(Vector3 hitPoint, float angle)
        {
            resetTimerRelatedValues();
            genericTimer.StopTimer();
            if (currentDart != null)
            {
                AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.DartHit).Clip, oneShot = true, volume = SettingData.SFXVolume });
                currentDart.StopFlash();
                currentDart.TweenthroughPoints(hitPoint);
            }
            Debug.Log("Throw Dart");
        }

        private void PlayDartHitParticle(Vector3 a_Position)
        {
            hitParticleSystem.Stop();
            hitParticleParent.transform.position = new Vector3(a_Position.x, a_Position.y, hitParticleParent.transform.position.z);
            hitParticleSystem.Play();
        }

        public void PlayButtonClickSound()
        {
            AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.ButtonClick).Clip, oneShot = true, volume = SettingData.SFXVolume });
        }


        
        #region ISocketState
        public void SocketStatus(SocketManager.ESocketStatus a_SocketState)
        {
            if (gamePlayMode == EGamePlayMode.Multiplayer)
            {
                if (gameStatus == EGameStatus.Playing)
                {
                    if (a_SocketState == SocketManager.ESocketStatus.Disconnected)
                    {
                        UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                        //CheckForInternetOnGameplay();
                    }
                }
            }
        }

        private void CheckForInternetOnGameplay()
        {
            Debug.LogWarning("--------CheckForInternetOnGameplay--------");
            float t_CurrentTime = Time.time;
            if (t_CurrentTime - gameStartTime <= 5.0f)
            {
                onOpenWinloosePopUp(Page.DrawMatchPanel, User.UserName, User.UserRace, User.UserColor);
                PlayerDrawScreen.Refresh();
                ShootingRangeScreen.Refresh();
            }
            else
            {
                displayPopup(false);
                ShootingRangeScreen.Refresh();
            }
        }

        public void SocketReconnected()
        {
            if (gameStatus == EGameStatus.None)
            {
                SocketManager.Instance.AddUser();
            }
            else if (gamePlayMode == EGamePlayMode.Multiplayer && gameStatus == EGameStatus.Playing)
            {
                doReJoinRequest();
            }
        }
        #endregion

        private void OnApplicationPause(bool pause)
        {
            if (gamePlayMode != EGamePlayMode.Multiplayer)
                return;

            if (pause && gameStatus == EGameStatus.Playing)
            {
                gameIsSuspended = true;
            }
            if (!pause && gameStatus == EGameStatus.Playing && gameIsSuspended)
            {
                Debug.Log("GameResumed");
                gameIsSuspended = false;
                doReJoinRequest();
                //onLeaveRoom();
            }
        }
    }
}

[System.Serializable]
public class ThrowDartData
{
    public int TotalValue { get; set; } = 0;
    public int HitPointScore { get; set; } = 0;
    public int ScoreMultiplier { get; set; } = 0;
    public Vector3 HitPosition { get; set; } = new Vector3(0, 0, 0);
    public bool TurnCanceled { get; set; } = false;
}

[System.Serializable]
public class GameTimerData
{
    [JsonProperty("totalGameTime")]
    public int TotalGameTime = 0;
    [JsonProperty("gameFinish")]
    public int GameFinish = 0;
}

[System.Serializable]
public class DartTimerData
{
    [JsonProperty("dartFinish")]
    public int DartFinish = 0;
}

[System.Serializable]
public class RejoinSuccessResponseData
{
    [JsonProperty("lastTurnTime")]
    public int LastTurnTime;
    [JsonProperty("gameRemainingTime")]
    public int GameRemainingTime;
    [JsonProperty("currentUserTurn")]
    public string CurrentUserTurn;
    [JsonProperty("turnStatus")]
    public int TurnStatus;

    [JsonProperty("firstUserLastDetails")]
    public List<RejoinResponseData> FirstUserLastDetails = new List<RejoinResponseData>();
    [JsonProperty("secondUserLastDetails")]
    public List<RejoinResponseData> SecondUserLastDetails = new List<RejoinResponseData>();
}

[System.Serializable]
public class RejoinFailureResponseData
{
    [JsonProperty("userLastDetails")]
    public List<RejoinResponseData> UserLastDetails = new List<RejoinResponseData>();
}

[System.Serializable]
public class RejoinResponseData
{
    [JsonProperty("_id")]
    public string Id;
    [JsonProperty("name")]
    public string RoomName;
    [JsonProperty("status")]
    public string Status;
    [JsonProperty("userId")]
    public string UserId;
    [JsonProperty("total")]
    public string TotalScore;
    [JsonProperty("score")]
    public string Score;
    [JsonProperty("isWin")]
    public int IsWin;
    [JsonProperty("turn")]
    public int Turn;
    [JsonProperty("dartPoint")]
    public string DartPoint;
    [JsonProperty("cupNumber")]
    public int CupNumber;
    [JsonProperty("totalCupWin")]
    public int TotalCupWin;
    [JsonProperty("roomCoin")]
    public int RoomCoin;
    [JsonProperty("scoreMultiplier")]
    public string ScoreMultiplier;
    [JsonProperty("hitScore")]
    public string HitScore;
    [JsonProperty("userName")]
    public string UserName;
    [JsonProperty("colorName")]
    public string ColorName;
    [JsonProperty("raceName")]
    public string RaceName;
    [JsonProperty("dartName")]
    public string DartName;
    [JsonProperty("firstName")]
    public string FirstName;
    [JsonProperty("lastName")]
    public string LastName;
}

[System.Serializable]
public class TemporarilyDisconnectData
{
    [JsonProperty("userId")]
    public string UserId { get; set; } = "";
    [JsonProperty("userGameStatus")]
    public string UserGameStatus { get; set; } = "";
    [JsonProperty("roomName")]
    public string RoomName { get; set; } = "";
}

[System.Serializable]
public class OpponentReconnectData
{
    [JsonProperty("userId")]
    public string UserId;
}