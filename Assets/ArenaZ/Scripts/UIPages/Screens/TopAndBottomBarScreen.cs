using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using System.Collections.Generic;
using System;

namespace ArenaZ.Screens
{
    public class TopAndBottomBarScreen : Singleton<TopAndBottomBarScreen>
    {
        [Header("Buttons")]
        [SerializeField] private Button historyButton;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button mailButton;
        [SerializeField] private Button settingButton;

        public int count = 0;

        private void Start()
        {
            GettingButtonReferences();
        }
        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }
        #region Button_References
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
        #endregion

        private void InfoButtonClicked()
        {
            UIManager.Instance.ShowScreen(Page.InfoAndRulesForPlayerPanel.ToString(),Hide.previous);
        }

        private void SettingButtonClicked()
        {
           // UIManager.Instance.HideScreenImmediately(Page.LogOutAlertOverlay.ToString());
            UIManager.Instance.ShowScreen(Page.SettingsPanel.ToString(), Hide.previous);
            UIManager.Instance.HideScreenImmediately(Page.PlayerColorChooser.ToString());
            if (PlayerPrefs.GetInt(PlayerPrefsValue.Logout.ToString())==0)
            {
                CharacterSelection.Instance.SetProfilePicOnClick();
            }
        }

        private void MailButtonClicked()
        {
            UIManager.Instance.ShowScreen(Page.MailboxPanel.ToString(), Hide.previous);
        }

        private void HistoryButtonClicked()
        {
            UIManager.Instance.ShowScreen(Page.PlayerMatchHistoryPanel.ToString(),Hide.previous);
        }
    }
}
