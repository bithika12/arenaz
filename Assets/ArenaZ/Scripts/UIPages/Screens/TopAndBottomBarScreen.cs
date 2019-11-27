using System;
using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using UnityEngine.EventSystems;

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
          //  if(EventSystem.current.)
            UIManager.Instance.ScreenShowAndHide(Page.InfoAndRulesForPlayer.ToString(),Hide.previous);
        }

        private void SettingButtonClicked()
        {
            UIManager.Instance.ScreenShowAndHide(Page.Settings.ToString(),Hide.previous);
            Settings.Instance.SetCountrySpriteAndCountryNameOnButton();
            Settings.Instance.LogInLogOutButtonNameSet();
        }

        private void MailButtonClicked()
        {
            UIManager.Instance.ScreenShowAndHide(Page.Mailbox.ToString(),Hide.previous);
        }

        private void HistoryButtonClicked()
        {
            UIManager.Instance.ScreenShowAndHide(Page.PlayerMatchHistory.ToString(),Hide.previous);
        }
    }
}
