using ArenaZ.Manager;
using DevCommons.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ArenaZ
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private CameraData selfCameraData = new CameraData();
        [SerializeField]
        private CameraData opponentCameraData = new CameraData();

        [SerializeField]
        private Camera camera;
        private Tween tween;

        //private void Awake()
        //{
        //    FileHandler.DeleteSaveFile(ConstantStrings.USER_SAVE_FILE_KEY);
        //    FileHandler.ClearPlayerPrefs();
        //}

        //private void Update()
        //{
        //    if (Input.GetKey(KeyCode.O))
        //    {
        //        SetCameraPosition(GameManager.Player.Opponent);
        //    }
        //    else if (Input.GetKey(KeyCode.S))
        //    {
        //        SetCameraPosition(GameManager.Player.Self);
        //    }

        //    if (Input.GetKey(KeyCode.F))
        //    {
        //        SetFocus(true);
        //    }
        //    else if (Input.GetKey(KeyCode.U))
        //    {
        //        SetFocus(false);
        //    }
        //}

        public void SetFocus(bool a_Set)
        {
            camera.DOFieldOfView(40.0f, 2.0f).SetEase(Ease.InSine);
            float t_DistanceFOV = 160;
            float t_OrgFOV = 40;

            if (a_Set)
            {
                camera.fieldOfView = t_DistanceFOV;
                camera.DOFieldOfView(t_OrgFOV, 2.0f).SetEase(Ease.InSine);
            }
            else
            {
                tween.Kill();
                tween = null;
                camera.fieldOfView = t_OrgFOV;
            }
        }

        public void SetCameraPosition(GameManager.Player a_PlayerType)
        {
            if (a_PlayerType == GameManager.Player.Self)
            {
                transform.DOMove(selfCameraData.CameraPosition, 1.5f).SetEase(Ease.InSine);
                transform.DORotate(selfCameraData.CameraRotation, 1.5f).SetEase(Ease.InSine);

                //transform.position = selfCameraData.CameraPosition;
                //transform.eulerAngles = selfCameraData.CameraRotation;
                Debug.Log($"CameraData: {a_PlayerType.ToString()}");
            }
            else if (a_PlayerType == GameManager.Player.Opponent)
            {
                transform.DOMove(opponentCameraData.CameraPosition, 1.5f).SetEase(Ease.InSine);
                transform.DORotate(opponentCameraData.CameraRotation, 1.5f).SetEase(Ease.InSine);

                //transform.position = opponentCameraData.CameraPosition;
                //transform.eulerAngles = opponentCameraData.CameraRotation;
                Debug.Log($"CameraData: {a_PlayerType.ToString()}");
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