using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using UnityEngine.UI.Extensions;
using ArenaZ.LevelMangement;

namespace ArenaZ.Screens
{
    public class CharacterSelection : RedAppleSingleton<CharacterSelection>
    {
        //Private Fields
        [Header("Buttons")]
        [Space(5)]
        [SerializeField] private Button arenaButton;
        [SerializeField] private Button trainingButton;
        [SerializeField] private Button rankingButton;

        [Header("Text Fields")]
        [Space(5)]
        [SerializeField] private Text userName;

        [Header("Scroll Snap")]
        [Space(5)]
        [SerializeField] private HorizontalScrollSnap horizontalScrollSnap;
        public readonly string[] names = { Page.Canines.ToString(), Page.Kepler.ToString(), Page.Cyborg.ToString(), Page.CyborgSecond.ToString(), Page.Human.ToString(), Page.Ebot.ToString(), Page.KeplerSecond.ToString() };
        //Public Fields

        private void Start()
        {
            GettingButtonReferences();
            ShowFirstText();
            horizontalScrollSnap.OnSelectionPageChangedEvent.AddListener(PageChecker);
            UIManager.Instance.setUserName += SetUserName;
        }
        //horizontalScrollSnap.OnSelectionPageChangedEvent.RemoveListener(PageChecker);
        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }
        #region Button_References
        private void GettingButtonReferences()
        {
            arenaButton.onClick.AddListener(OnClickArena);
            trainingButton.onClick.AddListener(OnClickArena);
            rankingButton.onClick.AddListener(OnclickRanking);
        }

        private void ReleaseButtonReferences()
        {
            arenaButton.onClick.RemoveListener(OnClickArena);
            trainingButton.onClick.RemoveListener(OnClickArena);
            rankingButton.onClick.RemoveListener(OnclickRanking);
        }
        #endregion

        public void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        #region UI_Functionalities
        public void ResetCharacterScroller(string userName)
        {
            horizontalScrollSnap.ChangePage(PlayerPrefs.GetInt(userName, 0));
        }

        public void SetProfilePicOnClick()
        {
            UIManager.Instance.showProfilePic?.Invoke(names[horizontalScrollSnap._currentPage]);
        }

        private void ShowFirstText()
        {
            UIManager.Instance.ShowCharacterName(Page.Canines.ToString());
        }

        public void PageChecker(int pageNo)
        {
            UIManager.Instance.ShowCharacterName(names[pageNo]);
        }

        private void OnClickArena()
        {
            UIManager.Instance.HideScreen(Page.CharacterSelectionPanel.ToString());
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ScreenShow(Page.LevelSelectionPanel.ToString(), Hide.none);
            UIManager.Instance.showProfilePic?.Invoke(names[horizontalScrollSnap._currentPage]);
            LevelSelection.Instance.OnSelectionGameplayType(GameType.normal);
            PlayerPrefs.SetInt(User.userName, horizontalScrollSnap._currentPage);
        }

        private void OnClickTraining()
        {
            UIManager.Instance.HideScreen(Page.CharacterSelectionPanel.ToString());
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ScreenShow(Page.LevelSelectionPanel.ToString(), Hide.none);
            UIManager.Instance.showProfilePic?.Invoke(names[horizontalScrollSnap._currentPage]);
            LevelSelection.Instance.OnSelectionGameplayType(GameType.training);
            PlayerPrefs.SetInt(User.userName, horizontalScrollSnap._currentPage);
        }

        public void OnclickRanking()
        {
            UIManager.Instance.ScreenShow(Page.LeaderBoardPanel.ToString(), Hide.none);
        }
        #endregion
    }
}
