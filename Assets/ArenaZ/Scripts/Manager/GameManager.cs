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

namespace ArenaZ.Manager
{
    [RequireComponent(typeof(TouchBehaviour))]
    public class GameManager : Singleton<GameManager>
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

        // Private Variables

        [Header("User")]
        [SerializeField] private Image userPic;
        [SerializeField] private Image userTimerImage;
        [SerializeField] private Text userName;
        [SerializeField] private TextMeshProUGUI userScoreText;

        [Header("Opponent")]
        [SerializeField] private Image opponentPic;
        [SerializeField] private Image opponentTimerImage;
        [SerializeField] private Text opponentName;
        [SerializeField] private TextMeshProUGUI opponentScoreText;

        [Header("Others")]
        [SerializeField] private int scorePopUpDuration;

        [SerializeField] private CameraController cameraController;
        [SerializeField] private UiPopup popup;

        [SerializeField] private GameObject winPopup;
        [SerializeField] private GameObject loosePopup;
        [SerializeField] private Vector3 wonPopupOriginalScale = new Vector3();

        private AudioPlayer mainMenuBGAudioPlayer;
        private AudioPlayer gameplayBGAudioPlayer;

        private bool playerTurn = false;
        private GameObject userDart;
        private GameObject opponentDart;
        private Dart currentDart;
        private TouchBehaviour touchBehaviour;        
        //private Dictionary<string, int> gameScore = new Dictionary<string, int>();
        private float timer = ConstantInteger.timerValue;

        private EGameState gameState = EGameState.Idle;

        private GameSoreData selfGameScoreData;
        private GameSoreData opponentGameScoreData;

        [SerializeField] private ScoreGraphic scoreGraphic;
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

        private Player lastPlayerType;
        private bool firstTime = false;
        private int throwCount;

        private int roundEndScore = 0;
        private List<GameObject> fakeDarts = new List<GameObject>();

        protected override void Awake()
        {
            touchBehaviour = GetComponent<TouchBehaviour>();
        }

        private void Start()
        {
            cameraController.SetCameraPosition(Player.Self);
            selfGameScoreData = new GameSoreData { PlayerType = Player.Self };
            opponentGameScoreData = new GameSoreData { PlayerType = Player.Opponent };

            Input.multiTouchEnabled = false;
            listenSocketEvents();
            touchBehaviour.OnDartMove += DartMove;
            touchBehaviour.OnDartThrow += DartThrow;
            UIManager.Instance.setUserName += SetUserName;
            UIManager.Instance.showProfilePic += SetUserProfileImage;
            ShootingRange.Instance.setOpponentName += SetOpponentName;
            ShootingRange.Instance.setOpponentImage += SetOpponentProfileImage;
            genericTimer = new GeneralTimer(this, ConstantInteger.timerValue);

            startMainMenuBGMusic();
        }

        private void getDartMaterialColorCode(string colorName)
        {

        }

        private void listenSocketEvents()
        {
            SocketListener.Listen(SocketListenEvents.nextTurn.ToString(), onNextTurn);
            SocketListener.Listen(SocketListenEvents.gameThrow.ToString(), onOpponentDartThrow);
            SocketListener.Listen(SocketListenEvents.gameOver.ToString(), onGameOver);
        }

        private void setDartColor(string colorName, GameObject dartGameObj)
        {
            // dartGameObj.GetComponent<MeshRenderer>().material.color = 
        }

        public void InitializeOnGameStartSequences()
        {
            Debug.Log("Playing For First Time");
            //if (currentDart != null)
            //    Destroy(currentDart);
            firstTime = true;
            ClearFakeDarts();
            startGamplayBGMusic();

            StartCoroutine(PastFiveSecond());
        }

        private IEnumerator PastFiveSecond()
        {
            yield return new WaitForSeconds(5.0f);
            gameState = EGameState.Playing;
        }

        private void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        public void SetUserProfileImage(string race, string color)
        {
            ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), race);
            EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), color);

            CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
            if (t_CharacterPicData != null)
            {
                userPic.material.SetTexture("_MainTex", t_CharacterPicData.ProfilePic.texture);
                //userPic.sprite = t_CharacterPicData.ProfilePic;
            }
        }

        public void SetOpponentProfileImage(string race, string color)
        {
            ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), race);
            EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), color);

            CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
            if (t_CharacterPicData != null)
            {
                opponentPic.material.SetTexture("_MainTex", t_CharacterPicData.ProfilePic.texture);
                //opponentPic.sprite = t_CharacterPicData.ProfilePic;
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

        private void SetOpponentName(string opponentName)
        {
            this.opponentName.text = opponentName;
        }

        public void GetDartGameObj()
        {
            Debug.Log("Spawn Dart At First");
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
            Debug.LogWarning("LeaveRoom");
            if (popup != null)
            {
                popup.Show("ALERT", "ARE YOU SURE?", onLeaveRoom,
                delegate { Debug.Log("Keep Playing."); });
            }
        }

        private void onLeaveRoom()
        {
            Debug.Log("---------------Leaving Room---------------");
            SocketManager.Instance.LeaveRoomRequest();

            cameraController.SetCameraPosition(Player.Self);
            genericTimer.StopTimer();

            displayPopup(false);

            if (currentDart != null)
                StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));

            gameState = EGameState.Idle;
            ClearFakeDarts();
            stopGamplayBGMusic();
            onSwitchTurn(Player.None);
        }

        private void onGameOver(string data)
        {
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

            if (currentDart != null)
                StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));

            gameState = EGameState.Idle;
            onSwitchTurn(Player.None);
            ClearFakeDarts();
            stopGamplayBGMusic();
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
            }
        }

        private void startMainMenuBGMusic()
        {
            mainMenuBGAudioPlayer = AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.MainMenu).Clip, loop = true });
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
            gameplayBGAudioPlayer = AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.GameplayBg).Clip, loop = true });
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
        }

        private void displayPopup(bool a_Won)
        {
            scoreGraphic.HideCrossAndBust();
            StartCoroutine(scoreGraphic.ClearScoreboard());
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
                    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.Win).Clip, oneShot = true });

                    UIManager.Instance.HideScreenImmediately(Page.GameplayPanel.ToString());
                    cameraController.SetFocus(false);
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
                    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.Lose).Clip, oneShot = true });

                    UIManager.Instance.HideScreenImmediately(Page.GameplayPanel.ToString());
                    cameraController.SetFocus(false);
                });
                t_Sequence.Play();
            }
        }

        private void onNextTurn(string data)
        {
            resetTimerRelatedValues();
            Debug.Log($"Next Turn : {data}"+"  User Id: "+User.UserId );
            var nextTurnData = DataConverter.DeserializeObject<ApiResponseFormat<NextTurn>>(data);
            playerTurn = true;
            if (nextTurnData.Result.UserId == User.UserId)
            {
                cameraController.SetCameraPosition(Player.Self);
                InstantiateDart(userDart);
                PlayerType = Player.Self;
                touchBehaviour.IsShooted = false;
                genericTimer.StartTimer(OnPlayerTimerComplete);
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
            }
            else
            {
                if (lastPlayerType != PlayerType)
                {
                    throwCount = 0;

                    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.AudienceCheering).Clip, oneShot = true });
                    updateAndShowTotalScore();

                    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.WindowChange).Clip, oneShot = true });
                    lastPlayerType = PlayerType;
                    ClearFakeDarts();
                }
            }
        }

        private void onOponnetTimerComplete()
        {
            Debug.Log("On Opponent timer Complete");
            alarmClock.Show();
            resetTimerRelatedValues();
            genericTimer.StopTimer();
            StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));
        }

        private void OnPlayerTimerComplete()
        {
            // Call Drat Throw Event WIth ZERO values
            Debug.Log("On Player Timer complete");
            alarmClock.Show();
            SocketManager.Instance.ThrowDartData(0, ConstantStrings.turnCancelled);
            StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));
            resetTimerRelatedValues();
            genericTimer.StopTimer();
        }

        private void onOpponentDartThrow(string data)
        {
            Debug.Log($"Dart Throw : {data}");
            var dartThrowData = DataConverter.DeserializeObject<ApiResponseFormat<DartThrow>>(data);
            if(dartThrowData.Result.UserId != User.UserId)
            {
                if (!dartThrowData.Result.DartPoint.Contains(ConstantStrings.turnCancelled))
                {
                    Debug.Log("Opponent Hit Score: " + int.Parse(dartThrowData.Result.PlayerScore));
                    if (int.Parse(dartThrowData.Result.PlayStatus) == 0)
                    {
                        int t_Score = int.Parse(dartThrowData.Result.PlayerScore);
                        storeCalculatedgameScore(Player.Opponent, t_Score);
                        if (t_Score > 0)
                            roundEndScore += t_Score;
                        else
                            roundEndScore = 0;
                        StartCoroutine(displayOpponentScoreWithDelay(dartThrowData.Result.HitScore, dartThrowData.Result.ScoreMultiplier, t_Score));
                    }
                    Vector3 opponenDartHitPoint = getValue(dartThrowData.Result.DartPoint);
                    DartThrow(opponenDartHitPoint, ConstantInteger.shootingAngle);
                    //Debug.Log("Opponent Dart Hit point:  " + opponenDartHitPoint + " " + " IsBust " + dartThrowData.Result.PlayStatus + " Score " + opponentRemainingScore);
                }
            }
        }

        private IEnumerator displayUserScoreWithDelay(int a_HitValue, int a_MultiplierValue, int a_Score)
        {
            scoreGraphic.ShowScore(a_HitValue, a_MultiplierValue, ScoreGraphic.EMoveTowards.None);
            yield return new WaitForSeconds(1.25f);
            if (a_MultiplierValue > 1)
                scoreGraphic.ShowScore(a_Score, 0, ScoreGraphic.EMoveTowards.None);
        }

        private IEnumerator displayOpponentScoreWithDelay(int a_HitValue, int a_MultiplierValue, int a_Score)
        {
            yield return new WaitForSeconds(1.5f);
            scoreGraphic.ShowScore(a_HitValue, a_MultiplierValue, ScoreGraphic.EMoveTowards.None);
            yield return new WaitForSeconds(1.25f);
            if (a_MultiplierValue > 1)
                scoreGraphic.ShowScore(a_Score, 0, ScoreGraphic.EMoveTowards.None);
        }

        public void OnCompletionDartHit(GameObject dartGameObj)
        {
            //throwCount++;
            //if (throwCount == 3)
            //{
            //    Debug.LogWarning("3 darts are thrown!");
            //    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.AudienceCheering).Clip, oneShot = true });
            //    updateAndShowTotalScore();
            //    //StartCoroutine(DisplayTotalScore(roundEndScore, PlayerType == Player.Self ? ScoreGraphic.EMoveTowards.User : PlayerType == Player.Opponent ? ScoreGraphic.EMoveTowards.Opponent : ScoreGraphic.EMoveTowards.None));
            //    throwCount = 0;
            //    //roundEndScore = 0;
            //}

            Debug.Log("On Completion Dart Hit");
            StartCoroutine(destroyDartAfterACertainTime(1, dartGameObj));
            InstantiateFakeDart(dartGameObj);
            if (PlayerType == Player.Self)
            {
                Debug.Log("Player...");

                BoardBodyPart boardBody = touchBehaviour.DartHitGameObj.GetComponent<BoardBodyPart>();
                if (boardBody.name.Contains("Outside"))
                {
                    AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.DartMiss).Clip, oneShot = true });
                }

                int hitScore = boardBody.HitPointScore;
                if (boardBody.ScoreMultiplier > 0)
                    hitScore = boardBody.HitPointScore * boardBody.ScoreMultiplier;

                Debug.Log($"HitPointScore: {boardBody.HitPointScore} ScoreMultiplier: {boardBody.ScoreMultiplier}");
                StartCoroutine(displayUserScoreWithDelay(boardBody.HitPointScore, boardBody.ScoreMultiplier, hitScore));
                Debug.Log("Player Hit Score:  " + hitScore);
                if (hitScore <= selfGameScoreData.Score)
                {
                    roundEndScore += hitScore;
                    Debug.Log("Score Current TS: " + roundEndScore);
                    storeCalculatedgameScore(Player.Self, hitScore);
                }
                else
                {
                    roundEndScore = 0;
                    scoreGraphic.ScoreDenied();
                }
                SocketManager.Instance.ThrowDartData(hitScore, boardBody.HitPointScore, boardBody.ScoreMultiplier, touchBehaviour.LastTouchPosition);
            }

            PlayDartHitParticle(dartGameObj.transform.position);
        }

        private void updateAndShowTotalScore()
        {
            //if (PlayerType == Player.Self)
            //    countDownScore(userScoreText, selfGameScoreData.Score);
            //else if (PlayerType == Player.Opponent)
            //    countDownScore(opponentScoreText, opponentGameScoreData.Score);

            StartCoroutine(DisplayTotalScore(roundEndScore, PlayerType == Player.Self ? ScoreGraphic.EMoveTowards.User : ScoreGraphic.EMoveTowards.Opponent));
            roundEndScore = 0;
        }

        private void countDownScore(TextMeshProUGUI a_TextMeshPro, int a_Score)
        {
            int t_CurrentScore = int.Parse(a_TextMeshPro.text);
            DOTween.To(() => t_CurrentScore, x => a_TextMeshPro.text = x.ToString(), a_Score, 0.5f).OnComplete(() => a_TextMeshPro.text = a_Score.ToString());
        }

        private IEnumerator DisplayTotalScore(int a_TotalScore, ScoreGraphic.EMoveTowards a_MoveTowards)
        {
            yield return new WaitForSeconds(3.0f);
            scoreGraphic.HideCrossAndBust();
            StartCoroutine(scoreGraphic.ClearScoreboard());
            Debug.Log("Score TotalScore: " + a_TotalScore);
            StartCoroutine(showTotalScore(a_TotalScore, a_MoveTowards));
        }

        private IEnumerator showTotalScore(int a_TotalScore, ScoreGraphic.EMoveTowards a_MoveTowards)
        {
            yield return new WaitForSeconds(2.0f);
            Debug.LogError($"GameManager: ScoreMT: {a_MoveTowards}");
            scoreGraphic.ShowScore(a_TotalScore, 0, a_MoveTowards);
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

        private void ClearFakeDarts()
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

        private void storeCalculatedgameScore(Player a_PlayerType, int a_HitValue)
        {
            if (a_PlayerType == Player.Self)
            {
                selfGameScoreData.Score -= a_HitValue;
                userScoreText.text = selfGameScoreData.Score.ToString();
            }
            else if (a_PlayerType == Player.Opponent)
            {
                opponentGameScoreData.Score -= a_HitValue;
                opponentScoreText.text = opponentGameScoreData.Score.ToString();
            }
            //StartCoroutine(showScore(a_HitValue));
            //showScore(a_HitValue);
        }

        public void ResetScore()
        {
            userScoreText.text = ScoreData.requiredScore.ToString();
            opponentScoreText.text = ScoreData.requiredScore.ToString();

            selfGameScoreData.Score = ScoreData.requiredScore;
            opponentGameScoreData.Score = ScoreData.requiredScore;
        }

        private IEnumerator showScore(int score)
        {
            yield return new WaitForSeconds(0.75f);
            UIManager.Instance.ShowPopWithText(Page.HitScore.ToString(), score.ToString(), scorePopUpDuration);
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
                AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.DartHit).Clip, oneShot = true });
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
            AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.ButtonClick).Clip, oneShot = true });
        }
    }

    [System.Serializable]
    public class GameSoreData
    {
        public GameManager.Player PlayerType;
        public int Score;
    }
}
