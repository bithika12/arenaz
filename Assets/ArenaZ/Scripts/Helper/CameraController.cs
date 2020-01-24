using ArenaZ.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private CameraData selfCameraData = new CameraData();
        [SerializeField]
        private CameraData opponentCameraData = new CameraData();

        public void SetCameraPosition(GameManager.Player playerType)
        {
            if (playerType == GameManager.Player.Self)
            {
                transform.position = selfCameraData.CameraPosition;
                transform.eulerAngles = selfCameraData.CameraRotation;
                Debug.Log($"CameraData: {playerType.ToString()}");
            }
            else if (playerType == GameManager.Player.Opponent)
            {
                transform.position = opponentCameraData.CameraPosition;
                transform.eulerAngles = opponentCameraData.CameraRotation;
                Debug.Log($"CameraData: {playerType.ToString()}");
            }
        }
    }

    [System.Serializable]
    public class CameraData
    {
        public Vector3 CameraPosition = new Vector3(0, 0, 0);
        public Vector3 CameraRotation = new Vector3(0, 0, 0);
    }
}