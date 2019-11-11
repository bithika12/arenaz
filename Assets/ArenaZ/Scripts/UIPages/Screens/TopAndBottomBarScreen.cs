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
            GettingButtonReferences();
        }
        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }

        private void GettingButtonReferences()
        {
            historyButton.onClick.AddListener(HistoryButtonClicked);
            mailButton.onClick.AddListener(MailButtonClicked);
            settingButton.onClick.AddListener(SettingButtonClicked);
            infoButton.onClick.AddListener(InfoButtonClicked);
        }

        private void ReleaseButtonReferences()
        {
            historyButton.onClick.RemoveListener(HistoryButtonClicked);
            mailButton.onClick.RemoveListener(MailButtonClicked);
            settingButton.onClick.RemoveListener(SettingButtonClicked);
            infoButton.onClick.RemoveListener(InfoButtonClicked);
        }

        private void InfoButtonClicked()
        {
            UIManager.Instance.ShowScreen(Page.InfoAndRulesForPlayer.ToString(),Hide.previous);
        }

        private void SettingButtonClicked()
        {
            UIManager.Instance.ShowScreen(Page.Settings.ToString(),Hide.previous);
        }

        private void MailButtonClicked()
        {
            UIManager.Instance.ShowScreen(Page.Mailbox.ToString(),Hide.previous);
        }

        private void HistoryButtonClicked()
        {
            UIManager.Instance.ShowScreen(Page.PlayerMatchHistory.ToString(),Hide.previous);
        }
    }
}
