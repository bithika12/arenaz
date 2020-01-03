using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using UnityEngine.UI.Extensions;
using ArenaZ.LevelMangement;
using RedApple;

namespace ArenaZ.Screens
{
    public class CharacterSelection : Singleton<CharacterSelection>
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
        [SerializeField]private HorizontalScrollSnap horizontalScrollSnap;
        public readonly string[] names = { Race.Canines.ToString(), Race.Kepler.ToString(), Race.Cyborg.ToString(), Race.CyborgSecond.ToString(), Race.Human.ToString(), Race.Ebot.ToString(), Race.KeplerSecond.ToString() };
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
            arenaButton.onClick.AddListener(()=> OnClickArena(GameType.normal));
            trainingButton.onClick.AddListener(()=> OnClickArena(GameType.training));
            rankingButton.onClick.AddListener(OnclickRanking);
        }

        private void ReleaseButtonReferences()
        {
            arenaButton.onClick.RemoveListener(()=> OnClickArena(GameType.normal));
            trainingButton.onClick.RemoveListener(()=> OnClickArena(GameType.training));
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
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == PlayerPrefs.GetString(PlayerprefsValue.CharacterName.ToString()))
                {
                    horizontalScrollSnap.ChangePage(i);
                }
            }
          //  horizontalScrollSnap.ChangePage(PlayerPrefs.GetInt(userName, 0));
        }

        public void SetProfilePicOnClick()
        {
            UIManager.Instance.showProfilePic?.Invoke(PlayerPrefs.GetString(PlayerprefsValue.CharacterName.ToString()));
        }

        private void ShowFirstText()
        {
            UIManager.Instance.ShowCharacterName(Race.Canines.ToString());
        }

        public void PageChecker(int pageNo)
        {
            UIManager.Instance.ShowCharacterName(names[pageNo]);
        }

        private void OnClickArena(GameType type)
        {
            enterArenaMode();
            User.userRace = names[horizontalScrollSnap._currentPage];
            UIManager.Instance.showProfilePic?.Invoke(names[horizontalScrollSnap._currentPage]);
            LevelSelection.Instance.OnSelectionGameplayType(type);
            PlayerPrefs.SetString(PlayerprefsValue.CharacterName.ToString(), names[horizontalScrollSnap._currentPage]);
           // PlayerPrefs.SetInt(User.userName, horizontalScrollSnap._currentPage);
            SocketManager.Instance.ColRequest();
        }


        private void enterArenaMode()
        {
            UIManager.Instance.HideScreen(Page.CharacterSelectionPanel.ToString());
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ScreenShow(Page.LevelSelectionPanel.ToString(), Hide.none);
        }

        public void OnclickRanking()
        {
            UIManager.Instance.ScreenShow(Page.LeaderBoardPanel.ToString(), Hide.none);
        }
        #endregion
    }
}
