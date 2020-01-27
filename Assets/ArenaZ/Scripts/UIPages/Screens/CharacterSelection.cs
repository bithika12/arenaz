using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ArenaZ.Manager;
using UnityEngine.UI.Extensions;
using ArenaZ.LevelMangement;
using RedApple;
using System;
using System.Text.RegularExpressions;
using DevCommons.Utility;

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
        public readonly string[] raceNames = { Race.Canines.ToString(), Race.Kepler.ToString(), Race.Cyborg.ToString(), Race.CyborgSecond.ToString(), Race.Human.ToString(), Race.Ebot.ToString(), Race.KeplerSecond.ToString(),Race.KeplerFemale.ToString(),Race.KeplerFemaleSecond.ToString(),Race.HumanSecond.ToString() };
        //Public Fields
        public static Action<string> setDart;

        public CharacterData SelfCharacterData { get; set; }

        private void Start()
        {           
            GettingButtonReferences();
            ShowFirstText();

            horizontalScrollSnap.OnSelectionPageChangedEvent.AddListener(PageChecker);
            UIManager.Instance.setUserName += SetUserName;
            PlayerColorChooser.setColorAfterChooseColor += SetColorOnCharacter;

            Invoke("LateStart", 1.5f);
        }

        private void LateStart()
        {
            string colorName = FileHandler.ReadFromPlayerPrefs(PlayerprefsValue.SelectedColor.ToString(), UIManager.Instance.StartColorName);
            Debug.Log("--------------ColorName: " + colorName);
            SetColorOnCharacter(colorName);

            string characterIdStr = FileHandler.ReadFromPlayerPrefs(PlayerprefsValue.SelectedCharacter.ToString(), "0");
            int characterId = int.Parse(characterIdStr);
            Debug.Log("--------------CharacterId: " + characterId);
            UIManager.Instance.ShowCharacterName(raceNames[characterId]);
            horizontalScrollSnap.GoToScreen(characterId);
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
            User.UserColor = colorName;
            FileHandler.SaveToPlayerPrefs(PlayerprefsValue.SelectedColor.ToString(), colorName);
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
            User.DartName = ConstantStrings.dart + GetTruncatedString(horizontalScrollSnap.CurrentPageObject().name);
            User.UserRace = raceNames[horizontalScrollSnap._currentPage];
            FileHandler.SaveToPlayerPrefs(PlayerprefsValue.SelectedCharacter.ToString(), horizontalScrollSnap._currentPage.ToString());
            Debug.Log("DartName: " + User.DartName + ", UserRace: " + User.UserRace);
            setDart?.Invoke(User.DartName);
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
            UIManager.Instance.ShowScreen(Page.LevelSelectionPanel.ToString(),Hide.none);
        }

        public void OnclickRanking()
        {
            UIManager.Instance.ShowScreen(Page.LeaderBoardPanel.ToString(),Hide.previous);
        }
        #endregion
    }
}

public class CharacterData
{
    public CharacterData()
    {
    }

    public CharacterData(int a_CharacterId, string a_ColorName)
    {
        CharacterId = a_CharacterId;
        ColorName = a_ColorName;
    }

    public int CharacterId { get; set; }
    public string ColorName { get; set; }
}
