using System;
using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Mail;
using ArenaZ.LeaderBoard;

namespace ArenaZ.Screens
{
    public class TopAndBottomBarScreen : RedAppleSingleton<TopAndBottomBarScreen>
    {
        [Header("Interactive Elements")]
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
            UIManager.Instance.ScreenShow(Page.InfoAndRulesForPlayerPanel.ToString(),Hide.previous);
        }

        private void SettingButtonClicked()
        {
            UIManager.Instance.HideScreenImmediately(Page.LeaderBoardPanel.ToString());
            UIManager.Instance.ScreenShow(Page.SettingsPanel.ToString(), Hide.previous);
            if (count < 1)
            {
                if (PlayerPrefs.GetInt(User.userName) != 0)
                {
                    CharacterSelection.Instance.SetProfilePicOnClick();
                }
                count++;
            }
        }

        private void MailButtonClicked()
        {
            UIManager.Instance.ScreenShow(Page.MailboxPanel.ToString(),Hide.previous);
            UIManager.Instance.HideScreenImmediately(Page.LeaderBoardPanel.ToString());
        }

        private void HistoryButtonClicked()
        {
            UIManager.Instance.ScreenShow(Page.PlayerMatchHistoryPanel.ToString(),Hide.previous);
            UIManager.Instance.HideScreenImmediately(Page.LeaderBoardPanel.ToString());
        }
    }
}
