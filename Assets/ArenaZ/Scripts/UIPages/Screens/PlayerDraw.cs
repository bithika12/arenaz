using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using DevCommons.Utility;

namespace ArenaZ.Screens 
{
	public class PlayerDraw : MonoBehaviour
	{
        [Header("User Image")]
        [SerializeField] private Image userFrame;
        [SerializeField] private Image userPic;

        [Header("Text")]
        [SerializeField] private Text WinnerUserName;

        [Header("Button")]
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
            playAgainButton.onClick.AddListener(onClickClose);
        }

        private void ReleaseButtonReferences()
        {
            closeButton.onClick.RemoveListener(onClickClose);
            playAgainButton.onClick.RemoveListener(onClickClose);
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

        public void SetUserName(string userName)
        {
            this.WinnerUserName.text = userName;
        }

        private void onClickClose()
        {
            UIManager.Instance.ClearOpenPagesContainer();
            //UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
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
