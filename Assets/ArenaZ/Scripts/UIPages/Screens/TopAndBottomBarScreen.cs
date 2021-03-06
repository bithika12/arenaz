using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using System.Collections.Generic;
using System;
<<<<<<< HEAD
using ArenaZ.GameMode;
=======
>>>>>>> parent of 1e8790f... Minor Changes
using ArenaZ.SettingsManagement;

namespace ArenaZ.Screens
{
    public class TopAndBottomBarScreen : Singleton<TopAndBottomBarScreen>
    {
        [Header("Top Buttons")]
        [SerializeField] private Button topSettingsBtn;

        [Header("Bottom Buttons")]
        [SerializeField] private Button historyButton;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button mailButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button shareButton;

        [SerializeField] private Button depositButton;
        [SerializeField] private Button withdrawButton;
        [SerializeField] private Button walletHistoryButton;

        [SerializeField] private Text userCoinCount;
        [SerializeField] private Text userCupCount;

        [SerializeField] private WalletHistory walletHistory;
<<<<<<< HEAD
        [SerializeField] private ShootingRange shootingRangeRef;
=======
>>>>>>> parent of 1e8790f... Minor Changes

        private ShareFile shareFile;
        public int count = 0;

        private void Start()
        {
            GettingButtonReferences();
            UIManager.Instance.setCoinAndCup += refreshValues;
            shareFile = GetComponent<ShareFile>();
        }

        private void refreshValues(string coinCount, string cupCount)
        {
            if (userCoinCount != null)
                userCoinCount.text = coinCount;
            if (userCupCount != null)
                userCupCount.text = cupCount;
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }

        #region Button_References
        private void GettingButtonReferences()
        {
            topSettingsBtn.onClick.AddListener(SettingButtonClicked);
            historyButton.onClick.AddListener(HistoryButtonClicked);
            mailButton.onClick.AddListener(MailButtonClicked);
            settingButton.onClick.AddListener(SettingButtonClicked);
            infoButton.onClick.AddListener(InfoButtonClicked);
            shareButton.onClick.AddListener(onClickShare);

            depositButton.onClick.AddListener(openDepositCoinPanel);
            withdrawButton.onClick.AddListener(openWithdrawCoinPanel);
            walletHistoryButton.onClick.AddListener(walletHistoryPanel);
        }

        private void ReleaseButtonReferences()
        {
            topSettingsBtn.onClick.RemoveListener(SettingButtonClicked);
            historyButton.onClick.RemoveListener(HistoryButtonClicked);
            mailButton.onClick.RemoveListener(MailButtonClicked);
            settingButton.onClick.RemoveListener(SettingButtonClicked);
            infoButton.onClick.RemoveListener(InfoButtonClicked);
            shareButton.onClick.RemoveListener(onClickShare);

            depositButton.onClick.RemoveListener(openDepositCoinPanel);
            withdrawButton.onClick.RemoveListener(openWithdrawCoinPanel);
            walletHistoryButton.onClick.RemoveListener(walletHistoryPanel);
        }
        #endregion

        private void InfoButtonClicked()
        {
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ShowScreen(Page.InfoAndRulesForPlayerPanel.ToString(),Hide.previous);
        }

        private void SettingButtonClicked()
        {
            Settings.Instance.FromMainMenu = true;
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            //UIManager.Instance.HideScreenImmediately(Page.LogOutAlertOverlay.ToString());
            UIManager.Instance.ShowScreen(Page.SettingsPanel.ToString(), Hide.previous);
            UIManager.Instance.HideScreenImmediately(Page.PlayerColorChooser.ToString());
            UIManager.Instance.HideScreenImmediately(Page.RegionPopup.ToString());
            if (PlayerPrefs.GetInt(PlayerPrefsValue.Logout.ToString())==0)
            {
                CharacterSelection.Instance.SetProfilePicOnClick();
            }
        }

        private void MailButtonClicked()
        {
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ShowScreen(Page.MailboxPanel.ToString(), Hide.previous);
        }

        private void HistoryButtonClicked()
        {
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ShowScreen(Page.PlayerMatchHistoryPanel.ToString(),Hide.previous);
        }

        private void openDepositCoinPanel()
        {
<<<<<<< HEAD
            shootingRangeRef.Refresh();
=======
>>>>>>> parent of 1e8790f... Minor Changes
            UIManager.Instance.ShowScreen(Page.WalletDepositRequestPanel.ToString());
        }

        private void openWithdrawCoinPanel()
        {
<<<<<<< HEAD
            shootingRangeRef.Refresh();
=======
>>>>>>> parent of 1e8790f... Minor Changes
            UIManager.Instance.ShowScreen(Page.WalletWithdrawPanel.ToString());
        }

        private void walletHistoryPanel()
        {
            UIManager.Instance.ShowScreen(Page.WalletHistoryPanel.ToString());
            walletHistory.Initialize();
        }

        private void onClickShare()
        {
            shareFile.Share();
        }
    }
}
