using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ArenaZ.Manager;
using UnityEngine.UI.Extensions;
using ArenaZ.LevelMangement;
using RedApple;
using System;
using System.Text.RegularExpressions;

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
        public readonly string[] raceNames = { Race.Canines.ToString(), Race.Kepler.ToString(), Race.Cyborg.ToString(), Race.CyborgSecond.ToString(), Race.Human.ToString(), Race.Ebot.ToString(), Race.KeplerSecond.ToString(),Race.KeplerWoman.ToString() };
        //Public Fields
        public static Action<string> setDart;

        private void Start()
        {           
            GettingButtonReferences();
            ShowFirstText();
            SetColorOnCharacter(UIManager.Instance.StartColorName);
            horizontalScrollSnap.OnSelectionPageChangedEvent.AddListener(PageChecker);
            UIManager.Instance.setUserName += SetUserName;
            PlayerColorChooser.setColorAfterChooseColor += SetColorOnCharacter;
        }

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

        public void SetColorOnCharacter(string colorName)
        {
            User.userColor = colorName;
            GameObject[] characters = horizontalScrollSnap.ChildObjects;
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].GetComponent<Character>().SetCharacterImage(colorName);
            }
        }

        private void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        #region UI_Functionalities
        public void ResetCharacterScroller(string userName)
        {
            for (int i = 0; i < raceNames.Length; i++)
            {
                if (raceNames[i] == PlayerPrefs.GetString(PlayerprefsValue.CharacterName.ToString()))
                {
                    horizontalScrollSnap.GoToScreen(i);
                }
            }
          //  horizontalScrollSnap.ChangePage(PlayerPrefs.GetInt(userName, 0));
        }

        public void SetProfilePicOnClick()
        {
            if (PlayerPrefs.GetString(PlayerprefsValue.CharacterName.ToString()) != string.Empty)
            {
                UIManager.Instance.showProfilePic?.Invoke(PlayerPrefs.GetString(PlayerprefsValue.CharacterName.ToString()));
            }
            else
            {
                UIManager.Instance.showProfilePic?.Invoke(Race.Canines.ToString());
            }
        }

        private void ShowFirstText()
        {
            UIManager.Instance.ShowCharacterName(Race.Canines.ToString());
        }

        public void PageChecker(int pageNo)
        {
            UIManager.Instance.ShowCharacterName(raceNames[pageNo]);
        }

        private void OnClickArena(GameType type)
        {
            enterArenaMode();
            User.dartName = ConstantStrings.dart + GetTruncatedString(horizontalScrollSnap.CurrentPageObject().name);
            User.userRace = raceNames[horizontalScrollSnap._currentPage];
            Debug.Log("DartName:  " + User.dartName);
            setDart?.Invoke(User.dartName);
            UIManager.Instance.showProfilePic?.Invoke(raceNames[horizontalScrollSnap._currentPage]);
            LevelSelection.Instance.OnSelectionGameplayType(type);
            PlayerPrefs.SetString(PlayerprefsValue.CharacterName.ToString(), raceNames[horizontalScrollSnap._currentPage]);
            SocketManager.Instance.ColRequest();
        }

        private string GetTruncatedString(string characterString)
        {
            string[] splittedString = Regex.Split(characterString, @"(?<!^)(?=[A-Z])");
            return splittedString[1];
        }

        private void enterArenaMode()
        {
            UIManager.Instance.HideScreen(Page.CharacterSelectionPanel.ToString());
            UIManager.Instance.HideScreen(Page.TopAndBottomBarPanel.ToString());
            UIManager.Instance.ShowScreen(Page.LevelSelectionPanel.ToString());
        }

        public void OnclickRanking()
        {
            UIManager.Instance.ShowScreen(Page.LeaderBoardPanel.ToString(),Hide.previous);
        }
        #endregion
    }
}
