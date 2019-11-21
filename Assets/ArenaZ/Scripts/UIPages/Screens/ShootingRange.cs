using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;
using ArenaZ.AccountAccess;

namespace ArenaZ.GameMode
{
    [RequireComponent(typeof(UIScreen))]
    public class ShootingRange : RedAppleSingleton<ShootingRange>
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
            profileImage.sprite = UIManager.Instance.GetCorrespondingProfileSprite(imageName, ProfilePic.Medium);
        }

        public void SetUserName()
        {
            userName.text = AccountAccessManager.Instance.UserName;
        }

        private void OnClickShootingRangeBack()
        {
            UIManager.Instance.ShowScreen(Page.LevelSelection.ToString(), Hide.none);
            UIManager.Instance.HideScreen(Page.Shootingrange.ToString());
        }

        private void OnClickStartGameWithCoinValue()
        {
            UIManager.Instance.ShowScreen(Page.PlayerMatch.ToString(), Hide.none);
        }
    }
}
