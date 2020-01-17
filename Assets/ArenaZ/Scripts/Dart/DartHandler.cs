using ArenaZ.ShootingObject;
using RedApple;
using RedApple.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ.Handler
{
    public class DartHandler : MonoBehaviour
    {
        private bool playerTurn = false;

        private GameObject userDart;
        private GameObject opponentDart;

        private void listenSocketEvents()
        {
            SocketListener.Listen(SocketListenEvents.nextTurn.ToString(), onNextTurn);
            SocketListener.Listen(SocketListenEvents.gameThrow.ToString(), onOpponentDartThrow);
            SocketListener.Listen(SocketListenEvents.gameOver.ToString(), onGameOver);
        }

        public void GetDartGameObj()
        {
            Debug.Log("Spawn Dart At First");
            string userDartPath = GameResources.dartPrefabFolderPath + "/" + User.dartName;
            string opponentDartPath = GameResources.dartPrefabFolderPath + "/" + Opponent.dartName;
            userDart = Resources.Load<GameObject>(userDartPath);
            opponentDart = Resources.Load<GameObject>(opponentDartPath);
        }

        private void onNextTurn(string data)
        {
            Debug.Log($"Next Turn : {data}" + "  User Id: " + User.userId);
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

        public void InstantiateDart(GameObject dartGameObj)
        {
            currentDart = Instantiate(dartGameObj, Vector3.zero, Quaternion.identity).GetComponent<Dart>();
            Debug.Log("Current Dart: " + currentDart.name);
        }
    }
}
