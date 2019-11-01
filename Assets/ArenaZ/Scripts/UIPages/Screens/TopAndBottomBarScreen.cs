using System;
using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;

namespace ArenaZ.Screens
{
    [RequireComponent(typeof(UIScreen))]
    public class TopAndBottomBarScreen : MonoBehaviour
    {
        [Header("Interactive Elements")]
        [SerializeField] private Button historyButton;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button mailButton;
        [SerializeField] private Button settingButton;

        private void Start()
        {
            historyButton.onClick.AddListener(HistoryButtonClicked);
            mailButton.onClick.AddListener(MailButtonClicked);
            settingButton.onClick.AddListener(SettingButtonClicked);
            infoButton.onClick.AddListener(InfoButtonClicked);
        }

        private void InfoButtonClicked()
        {
            
        }

        private void SettingButtonClicked()
        {
            
        }

        private void MailButtonClicked()
        {
            
        }

        private void HistoryButtonClicked()
        {
            UIManager.Instance.ShowScreen(Page.PlayerMatchHistory.ToString());
        }
    }
}
