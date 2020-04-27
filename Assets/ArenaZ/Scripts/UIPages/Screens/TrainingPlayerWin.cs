using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using DevCommons.Utility;
using RedApple.Api.Data;
using Newtonsoft.Json;
using RedApple.Utils;
using System.Linq;

namespace ArenaZ.Screens 
{
	public class TrainingPlayerWin : MonoBehaviour
	{
        [Header("User Image")]
        [SerializeField] private Image userFrame;
        [SerializeField] private Image userPic;
        [SerializeField] private Text userCupText;
        [SerializeField] private Text userWonCupText;

        [Header("Text")]
        [SerializeField] private Text WinnerUserName;

        [Header("Button")]
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
                userWonCupText.text = "0";
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
                userWonCupText.text = "+" + a_GameHistoryMatchDetails.gameHistoryUserDatas.CupNumber.ToString();
            }
        }

        private void OnRequestFailure(RestUtil.RestCallError obj)
        {
            Debug.Log("Get Data Error: " + obj.Description);
        }

        private void GettingButtonReferences()
        {
            closeButton.onClick.AddListener(onClickClose);
            playAgainButton.onClick.AddListener(onClickPlayNow);
        }

        private void ReleaseButtonReferences()
        {
            closeButton.onClick.RemoveListener(onClickClose);
            playAgainButton.onClick.RemoveListener(onClickPlayNow);
        }

        private void DisplayCoinAmount()
        {
            if (GameManager.Instance.GetGameplayMode() == GameManager.EGamePlayMode.Multiplayer)
            {
                int vlaue = PlayerPrefs.GetInt(ConstantStrings.ROOM_VALUE, 0);
                coinText.text = "+" + (vlaue * 2);
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

        private void onClickPlayNow()
        {
            GameManager.Instance.SetGameplayMode(GameManager.EGamePlayMode.Multiplayer);
            UIManager.Instance.ClearOpenPagesContainer();
            UIManager.Instance.HideScreen(Page.TrainingPlayerWinPanel.ToString());
            CharacterSelection.Instance.OnClickArena(GameType.normal);
        }

        private void onClickClose()
        {
            GameManager.Instance.SetGameplayMode(GameManager.EGamePlayMode.None);
            UIManager.Instance.ClearOpenPagesContainer();
            UIManager.Instance.HideScreen(Page.TrainingPlayerWinPanel.ToString());
            UIManager.Instance.ShowScreen(Page.CharacterSelectionPanel.ToString(), Hide.none);
            UIManager.Instance.ShowScreen(Page.TopAndBottomBarPanel.ToString(), Hide.none);
        }
    }
}
