using ArenaZ.Behaviour;
using ArenaZ.ShootingObject;
using UnityEngine;
using RedApple;
using System;
using RedApple.Api;

namespace ArenaZ.Manager
{
    [RequireComponent(typeof(TouchBehaviour))]
    public class GameManager : Singleton<GameManager>
    {
        // Private Variables
        [SerializeField] private bool projectileMove;       
        private TouchBehaviour touchBehaviour;
        private Dart currentDart;
        private const int throwCount = 3;

        private enum Player
        {
            mySelf,
            opponent
        }

        private Player PlayerType;

        // Public Variables
        public Action<int> dartScore;

        protected override void Awake()
        {
            currentDart = GameObject.FindGameObjectWithTag("Player").GetComponent<Dart>();
            touchBehaviour = GetComponent<TouchBehaviour>();
        }

        private void Start()
        {
            listenSocketEvents();
            touchBehaviour.OnDartMove += DartMove;
            touchBehaviour.OnDartThrow += DartThrow;
        }

        private void listenSocketEvents()
        {
            SocketListener.Listen(SocketListenEvents.nextTurn.ToString(),onNextTurn);
            SocketListener.Listen(SocketListenEvents.gameThrow.ToString(), onOpponentDartThrow);
        }

        private void onNextTurn(string data)
        {
            Debug.Log($"Next Turn : {data}");
            var nextTurnData = DataConverter.DeserializeObject<ApiResponseFormat<NextTurn>>(data);
            if(nextTurnData.Result.UserId == User.userId)
            {
                PlayerType = Player.mySelf;
            }
            else
            {
                PlayerType = Player.opponent;
            }
        }

        private void onOpponentDartThrow(string data)
        {
            Debug.Log($"Dart Throw : {data}");
            var opponentDartThrowdata = DataConverter.DeserializeObject<ApiResponseFormat<DartThrow>>(data);
            Vector3 opponenDartHitPoint = getValue(opponentDartThrowdata.Result.DartPoint);
            Debug.Log("Opponent Dart Hit point:  " + opponenDartHitPoint);
           // DartThrow(opponenDartHitPoint, 0);
        }

        private Vector3 getValue(string vector)
        {
            if (!vector.StartsWith("(") && vector.EndsWith(")"))
            {
                return Vector3.zero;
            }
            else
            {
                // vector = vector.Substring(1, vector.Length - 2);
                char[] splitChar = { '(', ',', ')' };
                Vector3 newVector = new Vector3();
                string[] splittedString = vector.Split(splitChar);
                newVector.x = float.Parse(splittedString[0]);
                newVector.y = float.Parse(splittedString[1]);
                newVector.z = float.Parse(splittedString[2]);
                return newVector;
            }
        }

        private void DartMove(Vector3 dartPosition)
        {
            currentDart.transform.position = dartPosition;
        }

        public void OnCompletionDartHit()
        {
            if (PlayerType == Player.mySelf)
            {
                BoardBodyPart boardBody = touchBehaviour.DartHitGameObj.GetComponent<BoardBodyPart>();
                SocketManager.Instance.ThrowDartData(boardBody.HitPointScore, touchBehaviour.DartHitPoint);
                Debug.Log("Hit Point Value:  " + boardBody.HitPointScore + " " + touchBehaviour.DartHitPoint);
            }
        }

        private void DartThrow(Vector3 hitPoint, float angle)
        {
            if (!projectileMove)
            {
                currentDart.TweenthroughPoints(hitPoint);
            }
            else
            {
                currentDart.MoveInProjectilePathWithPhysics(hitPoint, angle);
            }
        }
    }
}
