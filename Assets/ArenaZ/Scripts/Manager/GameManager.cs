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
        }

        private void onNextTurn(string data)
        {
            var nextTurnData = DataConverter.DeserializeObject<ApiResponseFormat<NextTurn>>(data);
            Debug.Log("Next Turn Id: " + nextTurnData.Result.UserId);
        }

        private void DartMove(Vector3 dartPosition)
        {
            currentDart.transform.position = dartPosition;
        }

        public void OnCompletionDartHit()
        {
            BoardBodyPart boardBody = touchBehaviour.DartHitGameObj.GetComponent<BoardBodyPart>();
            Debug.Log("Hit Point Value:  " + boardBody.HitPointScore+" "+touchBehaviour.DartHitPoint);
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
