using ArenaZ.Manager;
using DevCommons.Utility;
using RedApple;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ.Screens
{
	public class PlayerLoose : MonoBehaviour
	{
        [Header("User Image")]
        [SerializeField] private Image userFrame;
        [SerializeField] private Image userPic;

        [Header("Text")]
        [SerializeField] private Text WinnerUserName;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button playAgainButton;

        [Header("Others")]
        [SerializeField] private Text coinText;

        private void Start()
        {
            GameManager.Instance.setUserName += SetUserName;
            GameManager.Instance.setUserImage += SetUserProfileImage;
        }

        private void OnEnable()
        {
            GettingButtonReferences();
            DisplayCoinAmount();
        }

        private void OnDisable()
        {
            ReleaseButtonReferences();
        }

        private void GettingButtonReferences()
        {
            closeButton.onClick.AddListener(OnClickClose);
            playAgainButton.onClick.AddListener(OnClickClose);
        }

        private void ReleaseButtonReferences()
        {
            closeButton.onClick.RemoveListener(OnClickClose);
            playAgainButton.onClick.RemoveListener(OnClickClose);
        }

        private void DisplayCoinAmount()
        {
            int vlaue = PlayerPrefs.GetInt(ConstantStrings.ROOM_VALUE, 0);
            coinText.text = "-" + vlaue;
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

        private void OnClickClose()
        {
            UIManager.Instance.ClearOpenPagesContainer();
            //UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.HideScreen(Page.PlayerLoosePanel.ToString());
            //UIManager.Instance.ShowScreen(Page.LevelSelectionPanel.ToString(), Hide.none);
            UIManager.Instance.ShowScreen(Page.ShootingrangePanel.ToString(), Hide.none);
        }

        private void onClickPlayAgain()
        {
            SocketManager.Instance.GameRequest();
        }
    }
}
