using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using DevCommons.Utility;
using ArenaZ.Screens;
using ArenaZ.SettingsManagement;
using System.Collections.Generic;

namespace ArenaZ.LevelMangement
{
    public class LevelSelection : Singleton<LevelSelection>
    {
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button shootingRangeButton;
        [SerializeField] private List<Button> unavailabeLevelButtons = new List<Button>();
        [SerializeField] private Button backButton;
        [SerializeField] private Button comingSoonCloseButton;
        [SerializeField] private Button depositButton;

        [Header("Text Fields")]
        [Space(5)]
        [SerializeField] private Text userName;
        [SerializeField] private Text userCoinCount;
        [SerializeField] private Text userCupCount;

        [Header("GameObjects")]
        [Space(5)]
        [SerializeField] private GameObject comingSoonPopUp;

        [Header("User Image")]
        [SerializeField] private Image userFrame;
        [SerializeField] private Image userPic;

        private GameType gamePlayType;

        private void Start()
        {
            GettingButtonReferences();
            UIManager.Instance.setUserName += SetUserName;
            UIManager.Instance.showProfilePic += SetUserProfileImage;
            UIManager.Instance.setCoinAndCup += refreshValues;
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

        public void SetUserProfileImage(string race, string color)
        {
            ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), race);
            EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), color);

            SquareFrameData t_FrameData = DataHandler.Instance.GetSquareFrameData(t_Color);
            if (t_FrameData != null)
            {
                userFrame.sprite = t_FrameData.FramePic;
            }

            CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
            if (t_CharacterPicData != null)
            {
                userPic.sprite = t_CharacterPicData.ProfilePic;
            }
        }

        #region Button_References
        private void GettingButtonReferences()
        {
            settingsButton.onClick.AddListener(SettingButtonClicked);
            shootingRangeButton.onClick.AddListener(OnClickShootingRange);
            unavailabeLevelButtons.ForEach(x => x.onClick.AddListener(OnClickUnavailableLevel));
            comingSoonCloseButton.onClick.AddListener(OnClickComingSoonClose);
            backButton.onClick.AddListener(OnClickBack);
            depositButton.onClick.AddListener(openDepositCoinPanel);
        }

        private void ReleaseButtonReferences()
        {
            settingsButton.onClick.RemoveListener(SettingButtonClicked);
            shootingRangeButton.onClick.RemoveListener(OnClickShootingRange);
            unavailabeLevelButtons.ForEach(x => x.onClick.RemoveListener(OnClickUnavailableLevel));
            comingSoonCloseButton.onClick.RemoveListener(OnClickComingSoonClose);
            backButton.onClick.RemoveListener(OnClickBack);
            depositButton.onClick.RemoveListener(openDepositCoinPanel);
        }
        #endregion

        private void SettingButtonClicked()
        {
            Settings.Instance.FromMainMenu = false;
            //UIManager.Instance.HideScreenImmediately(Page.LogOutAlertOverlay.ToString());
            UIManager.Instance.ShowScreen(Page.SettingsPanel.ToString(), Hide.none);
            UIManager.Instance.HideScreenImmediately(Page.PlayerColorChooser.ToString());
            UIManager.Instance.HideScreenImmediately(Page.RegionPopup.ToString());
        }

        private void openDepositCoinPanel()
        {
            UIManager.Instance.ShowScreen(Page.WalletDepositRequestPanel.ToString());
        }

        public void OnSelectionGameplayType(GameType gameType)
        {
            gamePlayType = gameType;
        }

        public void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        #region UI_Functionalities

        private void OnClickShootingRange()
        {
            UIManager.Instance.HideScreenImmediately(Page.ComingSoonOverlay.ToString());
            if (gamePlayType == GameType.normal)
            {
                UIManager.Instance.HideScreen(Page.LevelSelectionPanel.ToString());
                UIManager.Instance.ShowScreen(Page.ShootingrangePanel.ToString(),Hide.none);
                GameManager.Instance.ShootingRangeScreen.Refresh();
            }
            //else if (gamePlayType == GameType.training)
            //{
            //    UIManager.Instance.HideScreen(Page.LevelSelectionPanel.ToString());
            //    UIManager.Instance.ShowScreen(Page.ShootingrangePanel.ToString(),Hide.none);
            //    GameManager.Instance.ShootingRangeScreen.Refresh();
            //}
        }

        private void OnClickUnavailableLevel()
        {
            UIManager.Instance.HideScreenImmediately(Page.ComingSoonOverlay.ToString());
            UIManager.Instance.ShowScreen(Page.ComingSoonOverlay.ToString());
        }

        private void OnClickComingSoonClose()
        {
            UIManager.Instance.HideScreen(Page.ComingSoonOverlay.ToString());
        }

        private void OnClickBack()
        {
            //UIManager.Instance.HideOpenScreen();
            UIManager.Instance.HideScreen(Page.LevelSelectionPanel.ToString());
            UIManager.Instance.ShowScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ShowScreen(Page.CharacterSelectionPanel.ToString());
            CharacterSelection.Instance.GetUnreadMail();
        }
        #endregion
    }
}
