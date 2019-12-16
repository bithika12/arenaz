using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;

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

        [Header("GameObjects")]
        [Space(5)]
        [SerializeField] private GameObject comingSoonPopUp;

        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Image profileImage;
        private GameType gamePlayType;

        private void Start()
        {
            GettingButtonReferences();
            UIManager.Instance.setUserName += SetUserName;
            UIManager.Instance.showProfilePic += SetProfileImage;
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();           
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

        public void SetProfileImage(string imageName)
        {
            profileImage.sprite = UIManager.Instance.GetProfile(imageName,ProfilePicType.Small);
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
                UIManager.Instance.ScreenShow(Page.ShootingrangePanel.ToString(), Hide.none);
            }
            else
            {
                UIManager.Instance.ScreenShow(Page.ShootingrangePanel.ToString(), Hide.none);
            }
        }

        private void OnClickSpeedRaceAndBunkerDef()
        {
            UIManager.Instance.ScreenShow(Page.ComingSoonOverlay.ToString(), Hide.none);
        }

        private void OnClickComingSoonClose()
        {
            UIManager.Instance.HideScreen(Page.ComingSoonOverlay.ToString());
        }

        private void OnClickBack()
        {
            UIManager.Instance.ScreenShow(Page.CharacterSelectionPanel.ToString(), Hide.none);
            UIManager.Instance.ScreenShow(Page.TopAndBottomBarPanel.ToString(), Hide.none);
            UIManager.Instance.HideScreen(Page.LevelSelectionPanel.ToString());
        }
        #endregion
    }
}
