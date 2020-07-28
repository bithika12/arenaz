using ArenaZ.Manager;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ.Screens
{
    public class UiPopup : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text messageText;

        [SerializeField] private Button proccedBtn;
        [SerializeField] private Button declinedBtn;

        public void Show(string title, string message, Action a_OnProcced, Action a_OnDeclined)
        {
            UIManager.Instance.ShowScreen(Page.PopupPanel.ToString(), Hide.none);

            titleText.text = title;
            messageText.text = message;

            proccedBtn.onClick.AddListener(() =>
            {
                a_OnProcced?.Invoke();
                OnReset();
            });
            declinedBtn.onClick.AddListener(() =>
            {
                a_OnDeclined?.Invoke();
                OnReset();
            });
        }

        private void OnReset()
        {
            proccedBtn.onClick.RemoveAllListeners();
            declinedBtn.onClick.RemoveAllListeners();
            UIManager.Instance.HideScreenImmediately(Page.PopupPanel.ToString());
        }
    }
}