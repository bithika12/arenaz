using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using DevCommons.Utility;

namespace ArenaZ.LevelMangement
{
    public class LevelSelection : Singleton<LevelSelection>
    {
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button shootingRangeButton;
        [SerializeField] private Button speedRaceButton;
        [SerializeField] private Button bunkerDefenseButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button comingSoonCloseButton;

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
            shootingRangeButton.onClick.AddListener(OnClickShootingRange);
            speedRaceButton.onClick.AddListener(OnClickSpeedRaceAndBunkerDef);
            bunkerDefenseButton.onClick.AddListener(OnClickSpeedRaceAndBunkerDef);
            comingSoonCloseButton.onClick.AddListener(OnClickComingSoonClose);
            backButton.onClick.AddListener(OnClickBack);
        }

        private void ReleaseButtonReferences()
        {
            shootingRangeButton.onClick.RemoveListener(OnClickShootingRange);
            speedRaceButton.onClick.RemoveListener(OnClickSpeedRaceAndBunkerDef);
            bunkerDefenseButton.onClick.RemoveListener(OnClickSpeedRaceAndBunkerDef);
            comingSoonCloseButton.onClick.RemoveListener(OnClickComingSoonClose);
            backButton.onClick.RemoveListener(OnClickBack);
        }
        #endregion

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
            if (gamePlayType == GameType.normal)
            {
                UIManager.Instance.HideScreen(Page.LevelSelectionPanel.ToString());
                UIManager.Instance.ShowScreen(Page.ShootingrangePanel.ToString(),Hide.none);
            }
            else
            {
                UIManager.Instance.HideScreen(Page.LevelSelectionPanel.ToString());
                UIManager.Instance.ShowScreen(Page.ShootingrangePanel.ToString(),Hide.none);
            }
        }

        private void OnClickSpeedRaceAndBunkerDef()
        {
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
        }
        #endregion
    }
}
