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

namespace ArenaZ.Manager
{
    [RequireComponent(typeof(TouchBehaviour))]
    public class GameManager : Singleton<GameManager>
    {
        public enum Player
        {
            Self,
            Opponent
        }

        // Private Variables
        [SerializeField] private bool projectileMove;
        [SerializeField] private int scorePopUpDuration;
        [SerializeField] private TextMeshProUGUI selfScoreText;
        [SerializeField] private TextMeshProUGUI opponentScoreText;
        [SerializeField] private Image userImage;
        [SerializeField] private Image opponentImage;
        [SerializeField] private Image userTimerImage;
        [SerializeField] private Image opponentTimerImage;
        [SerializeField] private Text userName;
        [SerializeField] private Text opponentName;

        [SerializeField] private CameraController cameraController;

        private bool playerTurn = false;
        private GameObject userDart;
        private GameObject opponentDart;
        private Dart currentDart;
        private TouchBehaviour touchBehaviour;        
        //private Dictionary<string, int> gameScore = new Dictionary<string, int>();
        private float timer = ConstantInteger.timerValue;

        private GameSoreData selfGameScoreData;
        private GameSoreData opponentGameScoreData;

        public Action<string> setUserName;
        public Action<string> setUserImage;

        public Player PlayerType { get; set; }

        private GeneralTimer genericTimer;

        // Public Variables      

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

        private void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        private void SetUserProfileImage(string imageName)
        {
            userImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.rounded);
        }

        private void SetOpponentProfileImage(string imageName)
        {
            opponentImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.rounded);
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
            SocketManager.Instance.LeaveRoomRequest();
        }

        private void onGameOver(string data)
        {
            cameraController.SetCameraPosition(Player.Self);
            genericTimer.StopTimer();
            Debug.Log($"OnGameOver : {data}" + "  User Id: " + User.UserId);
            var gameOverData = DataConverter.DeserializeObject<ApiResponseFormat<GameOver>>(data);
            if(gameOverData.Result.UserId == User.UserId)
            {
                onOpenWinloosePopUp(Page.PlayerWinPanel, User.UserName, User.UserRace);
            }
            else 
            {
                onOpenWinloosePopUp(Page.PlayerLoosePanel, User.UserName, User.UserRace);
            }
            if (currentDart != null)
                StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));
        }

        private void onOpenWinloosePopUp(Page popUpName,string userName,string userImage)
        {
            UIManager.Instance.ShowScreen(Page.UIPanel.ToString());
            //UIManager.Instance.ShowScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ShowScreen(popUpName.ToString(),Hide.previous);
            UIManager.Instance.ClearOpenPagesContainer();
            setUserName?.Invoke(userName);
            setUserImage?.Invoke(userImage);
        }

        private void onNextTurn(string data)
        {
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
            }
            else
            {
                cameraController.SetCameraPosition(Player.Opponent);
                InstantiateDart(opponentDart);
                PlayerType = Player.Opponent;
                touchBehaviour.IsShooted = true;
                genericTimer.StartTimer(onOponnetTimerComplete);
            }
        }

        private void onOponnetTimerComplete()
        {
            Debug.Log("On Opponent timer Complete");
            resetTimerRelatedValues();
            genericTimer.StopTimer();
            StartCoroutine(destroyDartAfterACertainTime(0, currentDart.gameObject));
        }

        private void OnPlayerTimerComplete()
        {
            // Call Drat Throw Event WIth ZERO values
            Debug.Log("On Player Timer complete");
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
                        storeCalculatedgameScore(PlayerType, int.Parse(dartThrowData.Result.PlayerScore));
                    }
                    Vector3 opponenDartHitPoint = getValue(dartThrowData.Result.DartPoint);
                    DartThrow(opponenDartHitPoint, ConstantInteger.shootingAngle);
                    //Debug.Log("Opponent Dart Hit point:  " + opponenDartHitPoint + " " + " IsBust " + dartThrowData.Result.PlayStatus + " Score " + opponentRemainingScore);
                }
            }
        }

        public void OnCompletionDartHit(GameObject dartGameObj)
        {
            Debug.Log("On Completion Dart Hit");
            StartCoroutine(destroyDartAfterACertainTime(1, dartGameObj));
            if (PlayerType == Player.Self)
            {
                Debug.Log("Player...");
                BoardBodyPart boardBody = touchBehaviour.DartHitGameObj.GetComponent<BoardBodyPart>();
                int hitScore = boardBody.HitPointScore * boardBody.ScoreMultiplier;
                Debug.Log("Player Hit Score:  " + hitScore);
                if (hitScore < selfGameScoreData.Score)
                    storeCalculatedgameScore(Player.Self, hitScore);
                SocketManager.Instance.ThrowDartData(hitScore, touchBehaviour.LastTouchPosition);
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
                selfScoreText.text = selfGameScoreData.Score.ToString();
            }
            else if (a_PlayerType == Player.Opponent)
            {
                opponentGameScoreData.Score -= a_HitValue;
                opponentScoreText.text = opponentGameScoreData.Score.ToString();
            }
            showScore(a_HitValue);
        }

        public void ResetScore()
        {
            selfScoreText.text = ScoreData.requiredScore.ToString();
            opponentScoreText.text = ScoreData.requiredScore.ToString();

            selfGameScoreData.Score = ScoreData.requiredScore;
            opponentGameScoreData.Score = ScoreData.requiredScore;
        }

        private void showScore(int score)
        {
            UIManager.Instance.ShowPopWithText(Page.HitScore.ToString(), score.ToString(), scorePopUpDuration);
        }

        private void DartMove(Vector3 dartPosition)
        {
            currentDart.transform.position = dartPosition;
        }

        public void InstantiateDart(GameObject dartGameObj)
        {
            currentDart = Instantiate(dartGameObj, Vector3.zero,Quaternion.identity).GetComponent<Dart>();
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
            currentDart.TweenthroughPoints(hitPoint);
            Debug.Log("Throw Dart");
        }
    }

    [System.Serializable]
    public class GameSoreData
    {
        public GameManager.Player PlayerType;
        public int Score;
    }
}
