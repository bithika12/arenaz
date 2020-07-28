using System.Collections;
using System.Collections.Generic;
using ArenaZ.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ
{
    public class InternetConnectionLostWindow : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseScreen);
        }

        private void CloseScreen()
        {
            UIManager.Instance.HideScreen(Page.InternetConnectionLostPanel.ToString());
        }
    }
}
