using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using ArenaZ.Manager;

namespace ArenaZ
{
    public class GameLoading : MonoBehaviour
    {
        public Transform loadingImage;

        public void WaitingForOtherPlayer()
        {
            UIManager.Instance.ShowScreen(Page.GameLoadingPanel.ToString(), Hide.none);
            loadingImage.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental);
        }

        public void HideLoadingScreen()
        {
            loadingImage.DOKill();
            loadingImage.eulerAngles = Vector3.zero;
            UIManager.Instance.HideScreenImmediately(Page.GameLoadingPanel.ToString());
        }
    }
}