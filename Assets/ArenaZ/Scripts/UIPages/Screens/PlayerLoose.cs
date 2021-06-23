using ArenaZ.Manager;
using DevCommons.Utility;
using RedApple;
using UnityEngine;
using UnityEngine.UI;
using RedApple.Api.Data;
using Newtonsoft.Json;
using RedApple.Utils;
using System.Linq;

namespace ArenaZ.Screens
{
	public class PlayerLoose : MonoBehaviour
	{
        [Header("User Image")]
        [SerializeField] private Image userFrame;
        [SerializeField] private Image userPic;
        [SerializeField] private Text userCupText;
        [SerializeField] private Text userLostCupText;

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
            Refresh();
        }

        private void OnDisable()
        {
            ReleaseButtonReferences();
        }

        public void Refresh(int a_LostCup, int a_TotalCup)
        {
            DisplayCoinAmount();
            userLostCupText.text = "-" + a_LostCup.ToString();
            userCupText.text = a_TotalCup.ToString();
        }

        public void Refresh()
        {
            DisplayCoinAmount();

            if (GameManager.Instance.GetGameplayMode() == GameManager.EGamePlayMode.Multiplayer)
            {
                RestManager.GetUserSelection(User.UserEmailId, OnGetUserSelection, OnRequestFailure);
                RestManager.GetLastGameHistory(User.UserEmailId, OnGetLastGameHistorySuccess, OnRequestFailure);
            }
            else
            {
                userCupText.text = "0";
                userLostCupText.text = "0";
            }
        }

        private void OnGetUserSelection(UserSelectionDetails userSelectionDetails)
        {
            Debug.Log($"------------------------------------USD: {JsonConvert.SerializeObject(userSelectionDetails)}");
            if (userSelectionDetails != null)
            {
                userCupText.text = userSelectionDetails.UserCup.ToString();
            }
        }

        private void OnGetLastGameHistorySuccess(LastGameHistory a_GameHistoryMatchDetails)
        {
            Debug.Log($"------------------------------------LGHD: {JsonConvert.SerializeObject(a_GameHistoryMatchDetails)}");
            if (a_GameHistoryMatchDetails != null)
            {
                userLostCupText.text = "-" + a_GameHistoryMatchDetails.gameHistoryUserDatas.CupNumber.ToString();
            }
        }

        private void OnRequestFailure(RestUtil.RestCallError obj)
        {
            Debug.Log("Get Data Error: " + obj.Description);
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
            if (GameManager.Instance.GetGameplayMode() == GameManager.EGamePlayMode.Multiplayer)
            {
                int vlaue = PlayerPrefs.GetInt(ConstantStrings.ROOM_VALUE, 0);
                coinText.text = "-" + vlaue;
            }
            else
                coinText.text = "0";
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
            User.ShowFreeTokenPanel = 1;
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
