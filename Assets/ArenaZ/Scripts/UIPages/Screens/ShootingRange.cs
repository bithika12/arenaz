using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;

namespace ArenaZ.GameMode
{
    public class ShootingRange : Singleton<ShootingRange>
    {
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button shootingRangeBackButton;
        [SerializeField] private Button coinButton10;
        [SerializeField] private Button coinButton50;
        [SerializeField] private Button coinButton100;
        [SerializeField] private Button coinButton250;
        [SerializeField] private Button coinButton500;

        [Header("Images")]
        [Space(5)]
        [SerializeField] private Image profileImage;

        [Header("Text")]
        [Space(5)]
        [SerializeField] private Text userName;

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

        #region ButtonReferences
        private void GettingButtonReferences()
        {
            shootingRangeBackButton.onClick.AddListener(OnClickShootingRangeBack);
            coinButton10.onClick.AddListener(OnClickStartGameWithCoinValue);
            coinButton50.onClick.AddListener(OnClickStartGameWithCoinValue);
            coinButton100.onClick.AddListener(OnClickStartGameWithCoinValue);
            coinButton250.onClick.AddListener(OnClickStartGameWithCoinValue);
            coinButton500.onClick.AddListener(OnClickStartGameWithCoinValue);
        }

        private void ReleaseButtonReferences()
        {
            shootingRangeBackButton.onClick.RemoveListener(OnClickShootingRangeBack);
            coinButton10.onClick.RemoveListener(OnClickStartGameWithCoinValue);
            coinButton50.onClick.RemoveListener(OnClickStartGameWithCoinValue);
            coinButton100.onClick.RemoveListener(OnClickStartGameWithCoinValue);
            coinButton250.onClick.RemoveListener(OnClickStartGameWithCoinValue);
            coinButton500.onClick.RemoveListener(OnClickStartGameWithCoinValue);
        }
        #endregion

        public void SetProfileImage(string imageName)
        {
            profileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium);
        }

        public void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        private void OnClickShootingRangeBack()
        {
            UIManager.Instance.ScreenShow(Page.LevelSelectionPanel.ToString(), Hide.none);
            UIManager.Instance.HideScreen(Page.ShootingrangePanel.ToString());
        }

        private void OnClickStartGameWithCoinValue()
        {
            UIManager.Instance.ScreenShow(Page.PlayerMatchPanel.ToString(), Hide.none);
            SocketManager.Instance.GameRequest();
            PlayerMatch.Instance.LoadScene();
        }
    }
}
