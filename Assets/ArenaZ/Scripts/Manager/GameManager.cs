using ArenaZ.Behaviour;
using ArenaZ.ShootingObject;
using UnityEngine;
using RedApple;
using RedApple.Api;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using ArenaZ.GameMode;
using System.Collections;
using ArenaZ.Screens;
using DevCommons.Utility;
using DG.Tweening;
using System.Linq;
using RedApple.Api.Data;
using RedApple.Utils;
using Newtonsoft.Json;

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

        [Header("User")]
        [SerializeField] private Image userPic;
        [SerializeField] private Image userTimerImage;
        [SerializeField] private Text userName;

        [Header("Opponent")]
        [SerializeField] private Image opponentPic;
        [SerializeField] private Image opponentTimerImage;
        [SerializeField] private Text opponentName;

        [Header("Others")]
        [SerializeField] private int scorePopUpDuration;
        [SerializeField] private Text countdownTimerText;

        [Header("Training User")]
        [SerializeField] private Image trainingUserPic;
        [SerializeField] private Image trainingUserTimerImage;
        [SerializeField] private Text trainingUserName;

        [SerializeField] private CameraController cameraController;
        [SerializeField] private UiPopup popup;

        [SerializeField] private GameObject winPopup;
        [SerializeField] private GameObject loosePopup;
        [SerializeField] private Vector3 wonPopupOriginalScale = new Vector3();

        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private ScoreGraphic scoreGraphic;

        [SerializeField] private CountdownTimer countdownTimer;

        private AudioPlayer mainMenuBGAudioPlayer;
        private AudioPlayer gameplayBGAudioPlayer;

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

        [SerializeField] private AlarmClock alarmClock;

        [Header("Hit Particle")]
        [SerializeField] private GameObject hitParticleParent;
        [SerializeField] private ParticleSystem hitParticleSystem;

        [Header("Others")]
        [SerializeField] private float dartDragForce = 0.0f;

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

        public void StartTraining()
        {
            GetDartGameObj();
            onSwitchTurn(Player.None);
            cameraController.SetFocus(true);
            cameraController.SetCameraPosition(Player.Self);
            UIManager.Instance.HideScreen(Page.UIPanel.ToString());
            UIManager.Instance.ShowScreen(Page.GameplayPanel.ToString(), Hide.none);
            UIManager.Instance.ShowScreen(Page.TrainingUIPanel.ToString(), Hide.none);
            InitializeOnGameStartSequences();
            gameStatus = EGameStatus.Playing;
            PlayerType = Player.Self;

            setTrainingUserProfileImage(User.UserName, User.UserRace, User.UserColor);
            StartCoroutine(onNextTurnTraining());
        }

        public void StopTraining()
        {
            Debug.LogWarning("StopTraining");
            if (popup != null)
            {
                popup.Show("QUIT", "ARE YOU SURE?", onLeaveTraining,
                delegate { Debug.Log("Keep Training."); });
            }
        }

        private void onLeaveTraining()
        {
            Debug.LogWarning("LeaveTraining");
            cameraController.SetFocus(false);

            firstTime = true;
            genericTimer.StopTimer();

            gameStatus = EGameStatus.None;

            if (currentDart != null)
                Destroy(currentDart.gameObject);

            displayPopup(true);
        }

        private void setTrainingUserProfileImage(string name, string race, string color)
        {
            trainingUserName.text = name;
            ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), race);
            EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), color);

            CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
            if (t_CharacterPicData != null)
            {
                trainingUserPic.material.SetTexture("_BaseMap", (Texture)t_CharacterPicData.ProfilePic.texture);
            }
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
            SocketListener.Listen(SocketListenEvents.nextTurn.ToString(), onNextTurn);
            SocketListener.Listen(SocketListenEvents.gameThrow.ToString(), onOpponentDartThrow);
            SocketListener.Listen(SocketListenEvents.gameOver.ToString(), onGameOver);
            SocketListener.Listen(SocketListenEvents.gameTimer.ToString(), onGameTimerEvent);
            SocketListener.Listen(SocketListenEvents.dartTimer.ToString(), onDartTimerEvent);
        }

        private void setDartColor(string colorName, GameObject dartGameObj)
        {
            // dartGameObj.GetComponent<MeshRenderer>().material.color = 
        }

        public void InitializeOnGameStartSequences()
        {
            //if (currentDart != null)
            //    Destroy(currentDart);
            firstTime = true;
            clearFakeDarts();
            if (gamePlayMode == EGamePlayMode.Multiplayer)
                startGamplayBGMusic();
            else if (gamePlayMode == EGamePlayMode.Training)
            {
                stopMainMenuBGMusic();
                if (gameplayBGAudioPlayer != null)
                {
                    gameplayBGAudioPlayer.Destroy();
                    gameplayBGAudioPlayer = null;
                }
            }
            gameStartTime = Time.time;
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
            if (genericTimer != null)
            {
                if (playerTurn && PlayerType == Player.Self)
                {
                    if (gamePlayMode == EGamePlayMode.Multiplayer)
                        userTimerImage.fillAmount = genericTimer.RemainingTime / ConstantInteger.timerValue;
                    else if (gamePlayMode == EGamePlayMode.Training)
                        trainingUserTimerImage.fillAmount = genericTimer.RemainingTime / ConstantInteger.timerValue;
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
            trainingUserTimerImage.fillAmount = genericTimer.SlowTimeFactor;
            userTimerImage.fillAmount = genericTimer.SlowTimeFactor;
            opponentTimerImage.fillAmount = genericTimer.SlowTimeFactor;
            playerTurn = false;
        }

        public void LeaveRoom()
        {
            Debug.LogWarning("LeaveRoom");
            if (popup != null)
            {
                popup.Show("QUIT", "ARE YOU SURE?", onLeaveRoom,
                delegate { Debug.Log("Keep Playing."); });
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
            genericTimer.StopTimer();

            displayPopup(false);

            //if (currentDart != null)
            //    StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));

            onSwitchTurn(Player.None);
        }

        private void onGameOver(string data)
        {
            gameStatus = EGameStatus.None;
            firstTime = true;
            if (currentDart != null)
                Destroy(currentDart.gameObject);
            clearFakeDarts();
            stopGamplayBGMusic();

            cameraController.SetCameraPosition(Player.Self);
            genericTimer.StopTimer();
            Debug.Log($"OnGameOver : {data}" + "  User Id: " + User.UserId);
            var gameOverData = DataConverter.DeserializeObject<ApiResponseFormat<GameOverResponse>>(data);

            if(gameOverData.Result.FirstUserId == User.UserId)
            {
                DisplayAppropriateGameOverScreen(gameOverData.Result.FirstUserGameStatus);
            }
            else if (gameOverData.Result.SecondUserId == User.UserId)
            {
                DisplayAppropriateGameOverScreen(gameOverData.Result.SecondUserGameStatus);
            }

            //if (currentDart != null)
            //    StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));

            onSwitchTurn(Player.None);
        }

        private void DisplayAppropriateGameOverScreen(string a_GameOverStatus)
        {
            if (a_GameOverStatus == EGameOverStatus.Win.ToString())
            {
                displayPopup(true);
            }
            else if (a_GameOverStatus == EGameOverStatus.Lose.ToString())
            {
                displayPopup(false);
            }
            else if (a_GameOverStatus == EGameOverStatus.Draw.ToString())
            {
                onOpenWinloosePopUp(Page.DrawMatchPanel, User.UserName, User.UserRace, User.UserColor);
                PlayerDrawScreen.Refresh();
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

        private void displayPopup(bool a_Won)
        {
            if (a_Won)
            {
                loosePopup.transform.localScale = Vector3.zero;
                loosePopup.SetActive(false);

                winPopup.transform.localScale = Vector3.zero;
                winPopup.SetActive(true);

                Sequence t_Sequence = DOTween.Sequence();
                t_Sequence.Append(winPopup.transform.DOScale(wonPopupOriginalScale, 1.0f).SetEase(Ease.InBounce));
                t_Sequence.AppendInterval(1.0f);
                t_Sequence.AppendCallback(() => 
                {
                    winPopup.SetActive(false);
                    onOpenWinloosePopUp(Page.PlayerWinPanel, User.UserName, User.UserRace, User.UserColor);
                    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.Win).Clip, oneShot = true, volume = SettingData.SFXVolume });
                    
                    UIManager.Instance.HideScreenImmediately(Page.GameplayPanel.ToString());
                    UIManager.Instance.HideScreenImmediately(Page.GameplayUIPanel.ToString());
                    cameraController.SetFocus(false);

                    PlayerWinScreen.Refresh();
                    ShootingRangeScreen.Refresh();
                });
                t_Sequence.Play();
            }
            else
            {
                winPopup.transform.localScale = Vector3.zero;
                winPopup.SetActive(false);

                loosePopup.transform.localScale = Vector3.zero;
                loosePopup.SetActive(true);

                Sequence t_Sequence = DOTween.Sequence();
                t_Sequence.Append(loosePopup.transform.DOScale(wonPopupOriginalScale, 1.0f).SetEase(Ease.InBounce));
                t_Sequence.AppendInterval(1.0f);
                t_Sequence.AppendCallback(() => 
                {
                    loosePopup.SetActive(false);
                    onOpenWinloosePopUp(Page.PlayerLoosePanel, User.UserName, User.UserRace, User.UserColor);
                    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.Lose).Clip, oneShot = true, volume = SettingData.SFXVolume });

                    UIManager.Instance.HideScreenImmediately(Page.GameplayPanel.ToString());
                    UIManager.Instance.HideScreenImmediately(Page.GameplayUIPanel.ToString());
                    cameraController.SetFocus(false);

                    PlayerLooseScreen.Refresh();
                    ShootingRangeScreen.Refresh();
                });
                t_Sequence.Play();
            }
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
                    countdownTimer.StartCountdown(t_GameTimerData.Result.TotalGameTime, false, (t) => countdownTimerText.text = t, ()=>
                    {
                        countdownTimerText.DOKill();
                        countdownTimerText.color = Color.yellow;
                    });
                if (t_GameTimerData.Result.GameFinish == 1)
                    countdownTimerText.DOColor(Color.red, 1.0f).SetLoops(-1, LoopType.Yoyo);
            }
        }

        private void onDartTimerEvent(string a_Data)
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            Debug.Log($"DartTimer Data : {a_Data}");
        }

        private IEnumerator onNextTurnTraining()
        {
            yield return new WaitForSeconds(2.0f);

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

        private void onOponnetTimerComplete()
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            Debug.Log("On Opponent timer Complete");
            alarmClock.Show();
            resetTimerRelatedValues();
            genericTimer.StopTimer();
            StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));
        }

        private void onPlayerTimerComplete()
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            // Call Drat Throw Event WIth ZERO values
            Debug.Log("On Player Timer complete");
            alarmClock.Show();
            SocketManager.Instance.ThrowDartData(0, ConstantStrings.turnCancelled);
            StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));
            resetTimerRelatedValues();
            genericTimer.StopTimer();
        }

        private void onPlayerTimerCompleteTraining()
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            Debug.Log("On Player Timer complete");
            alarmClock.Show();
            StartCoroutine(onNextTurnTraining());
        }

        private void onOpponentDartThrow(string data)
        {
            if (gameStatus != EGameStatus.Playing)
                return;

            Debug.Log($"onOpponentDartThrow : {data}");
            var dartThrowData = DataConverter.DeserializeObject<ApiResponseFormat<DartThrow>>(data);
            if(dartThrowData.Result.UserId != User.UserId)
            {
                scoreHandler.SetRoundScoreData(Player.Opponent, dartThrowData.Result.RoundScore);
                scoreHandler.SetRemainingScoreData(Player.Opponent, dartThrowData.Result.RemainingScore);

                if (!dartThrowData.Result.DartPoint.Contains(ConstantStrings.turnCancelled))
                {
                    Debug.Log("Opponent Hit Score: " + int.Parse(dartThrowData.Result.PlayerScore));
                    if (int.Parse(dartThrowData.Result.PlayStatus) == 0)
                    {
                        int t_TotalValue = dartThrowData.Result.HitScore;
                        if (dartThrowData.Result.ScoreMultiplier > 1)
                            t_TotalValue = dartThrowData.Result.HitScore * dartThrowData.Result.ScoreMultiplier;
                        StartCoroutine(displayOpponentScoreWithDelay(dartThrowData.Result.HitScore, dartThrowData.Result.ScoreMultiplier, t_TotalValue, false));
                    }
                    else
                    {
                        StartCoroutine(displayOpponentScoreWithDelay(0, 0, dartThrowData.Result.RoundScore, true));
                    }
                    Vector3 opponenDartHitPoint = getValue(dartThrowData.Result.DartPoint);
                    DartThrow(opponenDartHitPoint, ConstantInteger.shootingAngle);
                }
                else
                {
                    StartCoroutine(displayOpponentScoreWithDelay(0, 0, 0, false));
                }
            }
            else if (dartThrowData.Result.UserId == User.UserId)
            {
                scoreHandler.SetRoundScoreData(Player.Self, dartThrowData.Result.RoundScore);
                scoreHandler.SetRemainingScoreData(Player.Self, dartThrowData.Result.RemainingScore);

                if (!dartThrowData.Result.DartPoint.Contains(ConstantStrings.turnCancelled))
                {
                    if (int.Parse(dartThrowData.Result.PlayStatus) == 0)
                    {
                        scoreGraphic.ShowScore(dartThrowData.Result.HitScore, dartThrowData.Result.ScoreMultiplier);
                        if (dartThrowData.Result.ScoreMultiplier > 1)
                        {
                            int t_TotalValue = dartThrowData.Result.HitScore * dartThrowData.Result.ScoreMultiplier;
                            scoreGraphic.ShowScore(t_TotalValue, 0);
                        }

                        postDartThrowAction(Player.Self, false);
                    }
                    else
                    {
                        scoreGraphic.ShowScore(dartThrowData.Result.RoundScore, 0, ScoreGraphic.EMoveTowards.None, true);
                        postDartThrowAction(Player.Self, true);
                    }
                }
                else
                {
                    scoreGraphic.ShowScore(0, 0);
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
                            scoreGraphic.ShowScore(scoreHandler.SelfScoreData.ActiveRoundScore, 0, ScoreGraphic.EMoveTowards.User, false, () => scoreHandler.UpdateScoreText(Player.Self));
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
                            scoreGraphic.ShowScore(scoreHandler.OpponentScoreData.ActiveRoundScore, 0, ScoreGraphic.EMoveTowards.Opponent, false, ()=> scoreHandler.UpdateScoreText(Player.Opponent));
                        }
                    }
                }
            }
        }

        private IEnumerator displayOpponentScoreWithDelay(int a_HitValue, int a_MultiplierValue, int a_TotalValue, bool a_IsBust)
        {
            if (gameStatus != EGameStatus.Playing)
                yield break;
            yield return new WaitForSeconds(1.0f);

            if (!a_IsBust)
            {
                scoreGraphic.ShowScore(a_HitValue, a_MultiplierValue);
                if (a_MultiplierValue > 1)
                    scoreGraphic.ShowScore(a_TotalValue, 0);

                postDartThrowAction(Player.Opponent, false);
            }
            else
            {
                scoreGraphic.ShowScore(a_TotalValue, 0, ScoreGraphic.EMoveTowards.None, true);
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
                    SocketManager.Instance.ThrowDartData(t_TotalValue, boardBody.HitPointScore, boardBody.ScoreMultiplier, touchBehaviour.LastTouchPosition);
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
                scoreGraphic.ShowScore(0, 0);
            }
            else
            {
                scoreGraphic.ShowScore(boardBody.HitPointScore, boardBody.ScoreMultiplier);
                if (boardBody.ScoreMultiplier > 1)
                {
                    int t_TotalValue = boardBody.HitPointScore * boardBody.ScoreMultiplier;
                    scoreGraphic.ShowScore(t_TotalValue, 0);
                }
            }

            PlayDartHitParticle(dartGameObj.transform.position);
            StartCoroutine(onNextTurnTraining());
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

        private void clearFakeDarts()
        {
            if (fakeDarts.Any())
            {
                fakeDarts.ForEach(x => Destroy(x));
                fakeDarts.Clear();
            }
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
        public void SocketStatus(SocketManager.ESocketState a_SocketState)
        {
            if (a_SocketState == SocketManager.ESocketState.Disconnected && gameStatus == EGameStatus.Playing)
            {
                float t_CurrentTime = Time.time;
                if (t_CurrentTime - gameStartTime <= 5.0f)
                {
                    onOpenWinloosePopUp(Page.DrawMatchPanel, User.UserName, User.UserRace, User.UserColor);
                    PlayerDrawScreen.Refresh();
                }
            }
        }

        public void SocketReconnected()
        {
            if (gameStatus == EGameStatus.None)
            {
                SocketManager.Instance.AddUser();
            }
        }
        #endregion

        private void OnApplicationPause(bool pause)
        {
            if (pause && gameStatus == EGameStatus.Playing)
                gameIsSuspended = true;
            if (!pause && gameStatus == EGameStatus.Playing && gameIsSuspended)
            {
                Debug.Log("GameResumed");
                gameIsSuspended = false;
                onLeaveRoom();
            }
        }
    }
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
