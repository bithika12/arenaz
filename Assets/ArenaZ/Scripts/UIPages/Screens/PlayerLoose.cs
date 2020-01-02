using ArenaZ.Manager;
using RedApple;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ.Screens
{
	public class PlayerLoose : MonoBehaviour
	{
        [Header("Images")]
        [Space(5)]
        [SerializeField] private Image winnerProfileImage;

        [Header("Text")]
        [Space(5)]
        [SerializeField] private Text WinnerUserName;

        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button playAgainButton;

        private void Start()
        {
            GameManager.Instance.userName += SetUserName;
            GameManager.Instance.userImage += SetUserProfileImage;
        }

        private void GettingButtonReferences()
        {
            closeButton.onClick.AddListener(OnClickClose);
            playAgainButton.onClick.AddListener(onClickPlayAgain);
        }

        private void ReleaseButtonReferences()
        {
            closeButton.onClick.RemoveListener(OnClickClose);
            playAgainButton.onClick.RemoveListener(onClickPlayAgain);
        }

        public void SetUserProfileImage(string imageName)
        {
            winnerProfileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium);
        }

        public void SetUserName(string userName)
        {
            this.WinnerUserName.text = userName;
        }

        private void OnClickClose()
        {
            UIManager.Instance.HideScreen(Page.PlayerLoosePanel.ToString());
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ScreenShowNormal(Page.ShootingrangePanel.ToString());
        }

        private void onClickPlayAgain()
        {
            SocketManager.Instance.GameRequest();
        }

    }
}
