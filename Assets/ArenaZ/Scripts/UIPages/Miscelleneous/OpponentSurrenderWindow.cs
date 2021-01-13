using System;
using System.Collections;
using System.Collections.Generic;
using ArenaZ.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ
{
    public class OpponentSurrenderWindow : MonoBehaviour
    {
        [SerializeField] private Text infoText;
        private Action callback;

        public void OnProcced()
        {
            UIManager.Instance.HideScreen(Page.SurrenderedPopupPanel.ToString());
            callback?.Invoke();
        }

        public void UpdateInfo(string a_OpponentName, Action a_Callback)
        {
            infoText.text = a_OpponentName + " has surrendered.";
            callback = a_Callback;
        }
    }
}
