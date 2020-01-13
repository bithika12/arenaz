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
        // Private Variables
        [SerializeField] private bool projectileMove;
        [SerializeField] private int scorePopUpDuration;
        [SerializeField] private TextMeshProUGUI txtMeshPro;
        [SerializeField] private Image userImage;
        [SerializeField] private Image opponentImage;
        [SerializeField] private Image userTimerImage;
        [SerializeField] private Image opponentTimerImage;
        [SerializeField] private Text userName;
        [SerializeField] private Text opponentName;
        private bool playerTurn = false;
        private GameObject userDart;
        private GameObject opponentDart;
        private Dart currentDart;
        private TouchBehaviour touchBehaviour;        
        private Dictionary<string, int> gameScore = new Dictionary<string, int>();
        private int opponentRemainingScore = ConstantInteger.totalGameScore;
        private float timer = ConstantInteger.timerValue;

        public Action<string> setUserName;
        public Action<string> setUserImage;

        private enum Player
        {
            player,
            opponent
        }

        private Player PlayerType;
        private GeneralTimer genericTimer;

        // Public Variables      

        protected override void Awake()
        {
            touchBehaviour = GetComponent<TouchBehaviour>();
        }

        private void Start()
        {
            Input.multiTouchEnabled = false;
            listenSocketEvents();
            touchBehaviour.OnDartMove += DartMove;
            touchBehaviour.OnDartThrow += DartThrow;
            UIManager.Instance.setUserName += SetUserName;
            UIManager.Instance.showProfilePic += SetUserProfileImage;
            ShootingRange.Instance.setOpponentName += SetOpponentName;
            ShootingRange.Instance.setOpponentImage += SetOpponentProfileImage;
            genericTimer = new GeneralTimer(this,ConstantInteger.timerValue);
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
            string userDartPath = GameResources.dartPrefabFolderPath + "/" + User.dartName;
            string opponentDartPath = GameResources.dartPrefabFolderPath + "/" + Opponent.dartName;
            userDart = Resources.Load<GameObject>(userDartPath);
            opponentDart = Resources.Load<GameObject>(opponentDartPath);
        }

        private void Update()
        {
            if (genericTimer != null)
            {
                if (playerTurn && PlayerType == Player.player)
                {
                    userTimerImage.fillAmount = genericTimer.RemainingTime / ConstantInteger.timerValue;
                }
                if (playerTurn && PlayerType == Player.opponent)
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

        private void onGameOver(string data)
        {
            Debug.Log($"OnGameOver : {data}" + "  User Id: " + User.userId);
            var gameOverData = DataConverter.DeserializeObject<ApiResponseFormat<GameOver>>(data);
            if(gameOverData.Result.UserId == User.userId)
            {
                onOpenWinloosePopUp(Page.PlayerWinPanel, User.userName, User.userRace);
            }
            else 
            {
                onOpenWinloosePopUp(Page.PlayerLoosePanel, User.userName, User.userRace);
            }
        }

        private void onOpenWinloosePopUp(Page popUpName,string userName,string userImage)
        {
            UIManager.Instance.ShowScreen(Page.UIPanel.ToString());
            UIManager.Instance.ShowScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ShowScreen(popUpName.ToString(),Hide.previous);
            UIManager.Instance.ClearOpenPagesContainer();
            setUserName?.Invoke(userName);
            setUserImage?.Invoke(userImage);
        }

        private void onNextTurn(string data)
        {
            Debug.Log($"Next Turn : {data}"+"  User Id: "+User.userId );
            var nextTurnData = DataConverter.DeserializeObject<ApiResponseFormat<NextTurn>>(data);
            playerTurn = true;
            if (nextTurnData.Result.UserId == User.userId)
            {
                InstantiateDart(userDart);
                PlayerType = Player.player;
                touchBehaviour.IsShooted = false;
                genericTimer.StartTimer(OnPlayerTimerComplete);
            }
            else
            {
                InstantiateDart(opponentDart);
                PlayerType = Player.opponent;
                touchBehaviour.IsShooted = true;
                genericTimer.StartTimer(onOponnetTimerComplete);
            }
            updateScoreBoardInEveryTurn(PlayerType);
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
            if(dartThrowData.Result.UserId != User.userId)
            {
                if (!dartThrowData.Result.DartPoint.Contains(ConstantStrings.turnCancelled))
                {
                    Debug.Log("Opponent Hit Score: " + int.Parse(dartThrowData.Result.PlayerScore));
                    UIManager.Instance.ShowPopWithText(Page.HitScore.ToString(), int.Parse(dartThrowData.Result.PlayerScore).ToString(), scorePopUpDuration);
                    if (int.Parse(dartThrowData.Result.PlayStatus) == 0)
                    {
                        storeCalculatedgameScore(PlayerType.ToString(), int.Parse(dartThrowData.Result.PlayerScore));
                        showScore(int.Parse(dartThrowData.Result.PlayerScore));
                    }
                    Vector3 opponenDartHitPoint = getValue(dartThrowData.Result.DartPoint);
                    DartThrow(opponenDartHitPoint, ConstantInteger.shootingAngle);
                    Debug.Log("Opponent Dart Hit point:  " + opponenDartHitPoint + " " + " IsBust " + dartThrowData.Result.PlayStatus + " Score " + opponentRemainingScore);
                }
            }
        }

        public void OnCompletionDartHit(GameObject dartGameObj)
        {
            Debug.Log("On Completion Dart Hit");
            StartCoroutine(destroyDartAfterACertainTime(1, dartGameObj));
            if (PlayerType == Player.player)
            {
                Debug.Log("Player...");
                BoardBodyPart boardBody = touchBehaviour.DartHitGameObj.GetComponent<BoardBodyPart>();
                int hitScore = boardBody.HitPointScore * boardBody.ScoreMultiplier;
                Debug.Log("Player Hit Score:  " + hitScore);
                UIManager.Instance.ShowPopWithText(Page.HitScore.ToString(), hitScore.ToString(), scorePopUpDuration);
                if (hitScore < gameScore[PlayerType.ToString()])
                {
                    storeCalculatedgameScore(Player.player.ToString(), hitScore);
                    showScore(hitScore);
                }
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

        private void storeCalculatedgameScore(string key, int hitValue)
        {
            gameScore[key] -= hitValue;
            Debug.Log("Player : " + key + "Score: " + gameScore[key]);
        }

        private void showScore(int score)
        {
            int previousVal = int.Parse(txtMeshPro.text);
            txtMeshPro.text = (previousVal - score).ToString();
        }

        private void updateScoreBoardInEveryTurn(Player playerType)
        {
            if (!gameScore.ContainsKey(playerType.ToString()))
            {
                gameScore.Add(playerType.ToString(), ConstantInteger.totalGameScore);
            }
            txtMeshPro.text = gameScore[playerType.ToString()].ToString();
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
            if (!projectileMove)
            {
                resetTimerRelatedValues();
                genericTimer.StopTimer();
                currentDart.TweenthroughPoints(hitPoint);
                Debug.Log("Throw Dart");
            }
            else
            {
                resetTimerRelatedValues();
                genericTimer.StopTimer();
                currentDart.MoveInProjectilePathWithPhysics(hitPoint, angle);
            }
        }
    }
}
