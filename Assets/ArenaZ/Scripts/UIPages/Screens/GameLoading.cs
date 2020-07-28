using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;

namespace ArenaZ
{
    public class GameLoading : MonoBehaviour
    {
        [SerializeField] private Button cancelBtn;

        public void WaitingForOtherPlayer()
        {
            UIManager.Instance.ShowScreen(Page.GameLoadingPanel.ToString(), Hide.none);
            cancelBtn.onClick.AddListener(() =>
            {
                SocketManager.Instance.GameRequestCancel();
                HideLoadingScreen();
            });
        }

        public void HideLoadingScreen()
        {
            UIManager.Instance.HideScreenImmediately(Page.GameLoadingPanel.ToString());
        }
    }
}