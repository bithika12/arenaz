using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;

namespace ArenaZ.Screens 
{
	public class PlayerWin : MonoBehaviour
	{
        [Header("Images")]
        [Space(5)]
        [SerializeField] private Image winnerProfileImage;

        [Header("Text")]
        [Space(5)]
        [SerializeField] private Text WinnerUserName;

        [Header("Button")]
        [Space(5)]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button playAgainButton;

        private void Start()
        {
            GameManager.Instance.setUserName += SetUserName;
            GameManager.Instance.setUserImage += SetUserProfileImage;
        }

        private void OnEnable()
        {
            GettingButtonReferences();
        }

        private void OnDisable()
        {
            ReleaseButtonReferences();
        }

        private void GettingButtonReferences()
        {
            closeButton.onClick.AddListener(onClickClose);
            playAgainButton.onClick.AddListener(onClickPlayAgain);
        }

        private void ReleaseButtonReferences()
        {
            closeButton.onClick.RemoveListener(onClickClose);
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

        private void onClickClose()
        {
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.HideScreen(Page.PlayerWinPanel.ToString());
            UIManager.Instance.ShowScreen(Page.LevelSelectionPanel.ToString(), Hide.none);
            UIManager.Instance.ShowScreen(Page.ShootingrangePanel.ToString(), Hide.none);
        }

        private void onClickPlayAgain()
        {
            SocketManager.Instance.GameRequest();
        }

    }
}
