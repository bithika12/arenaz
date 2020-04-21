using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using RedApple;
using DevCommons.Utility;
using RedApple.Api.Data;
using Newtonsoft.Json;
using RedApple.Utils;

namespace ArenaZ.Screens 
{
	public class PlayerDraw : MonoBehaviour
	{
        [Header("User Image")]
        [SerializeField] private Image userFrame;
        [SerializeField] private Image userPic;

        [Header("Text")]
        [SerializeField] private Text WinnerUserName;
        [SerializeField] private Text userCoinCount;
        [SerializeField] private Text userCupCount;

        [Header("Button")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button playAgainButton;

        private void Start()
        {
            GameManager.Instance.setUserName += SetUserName;
            GameManager.Instance.setUserImage += SetUserProfileImage;
            UIManager.Instance.setCoinAndCup += refreshValues;
        }

        private void refreshValues(string coinCount, string cupCount)
        {
            if (userCoinCount != null)
                userCoinCount.text = coinCount;
            if (userCupCount != null)
                userCupCount.text = cupCount;
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


        public void Refresh()
        {
            if (GameManager.Instance.GetGameplayMode() == GameManager.EGamePlayMode.Multiplayer)
            {
                RestManager.GetUserSelection(User.UserEmailId, OnGetUserSelection, OnRequestFailure);
            }
            else
            {
                if (userCoinCount != null)
                    userCoinCount.text = "0";
                if (userCupCount != null)
                    userCupCount.text = "0";
            }
        }

        private void OnGetUserSelection(UserSelectionDetails userSelectionDetails)
        {
            Debug.Log($"------------------------------------USD: {JsonConvert.SerializeObject(userSelectionDetails)}");
            if (userSelectionDetails != null)
            {
                if (userCoinCount != null)
                    userCoinCount.text = userSelectionDetails.UserCoin.ToString();
                if (userCupCount != null)
                    userCupCount.text = userSelectionDetails.UserCup.ToString();
            }
        }

        private void OnRequestFailure(RestUtil.RestCallError obj)
        {
            Debug.Log("Get Data Error: " + obj.Description);
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
            UIManager.Instance.HideScreen(Page.DrawMatchPanel.ToString());
            //UIManager.Instance.ShowScreen(Page.LevelSelectionPanel.ToString(), Hide.none);
            UIManager.Instance.ShowScreen(Page.ShootingrangePanel.ToString(), Hide.none);
        }

        private void onClickPlayAgain()
        {
            SocketManager.Instance.GameRequest();
        }
    }
}
