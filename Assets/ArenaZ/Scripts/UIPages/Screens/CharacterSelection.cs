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
using RedApple.Api.Data;
using Newtonsoft.Json;
using RedApple.Utils;
using System.Linq;
using ArenaZ.SettingsManagement;

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
        [SerializeField] private HorizontalScrollSnap horizontalScrollSnap;

        public readonly string[] raceNames = { ERace.Canines.ToString(), ERace.Kepler.ToString(), ERace.Cyborg.ToString(), ERace.CyborgSecond.ToString(), ERace.Human.ToString(), ERace.Ebot.ToString(), ERace.KeplerSecond.ToString(),ERace.KeplerFemale.ToString(),ERace.KeplerFemaleSecond.ToString(),ERace.HumanSecond.ToString() };
        //Public Fields
        public static Action<string> setDart;

        public CharacterData SelfCharacterData { get; set; }

        [Header("Others")]
        [Space(5)]
        [SerializeField] private Settings settings;
        [SerializeField] private PlayerColorChooser playerColorChooser;

        private void Start()
        {           
            GettingButtonReferences();
            ShowFirstText();

            horizontalScrollSnap.OnSelectionPageChangedEvent.AddListener(PageChecker);
            UIManager.Instance.setUserName += SetUserName;
            PlayerColorChooser.setColorAfterChooseColor += SetColorOnCharacter;

            Invoke("Initialize", 1.5f);
        }

        public void Initialize()
        {
            Debug.Log("Initialize CS");
            RestManager.GetUserSelection(User.UserEmailId, OnGetUserSelection, OnErrorGetSelectionData);

            string colorName = FileHandler.ReadFromPlayerPrefs(PlayerPrefsValue.SelectedColor.ToString(), UIManager.Instance.StartColorName);
            //Debug.Log("--------------ColorName: " + colorName);
            //SetColorOnCharacter(colorName);

            //string characterIdStr = FileHandler.ReadFromPlayerPrefs(PlayerPrefsValue.SelectedCharacter.ToString(), "0");
            //int characterId = int.Parse(characterIdStr);
            //Debug.Log("--------------CharacterId: " + characterId);
            //UIManager.Instance.ShowCharacterName(raceNames[characterId]);
            //horizontalScrollSnap.GoToScreen(characterId);
        }

        private void OnGetUserSelection(UserSelectionDetails userSelectionDetails)
        {
            Debug.Log($"------------------------------------USD: {JsonConvert.SerializeObject(userSelectionDetails)}");
            if (userSelectionDetails != null)
            {
                User.UserCountry = userSelectionDetails.CountryName;
                User.UserLanguage = userSelectionDetails.LanguageName;

                CountryPicData t_Data = DataHandler.Instance.GetCountryPicData(string.IsNullOrEmpty(User.UserCountry) ? "in" : User.UserCountry);
                if (t_Data != null)
                    settings.UpdateCountryDetails(t_Data.CountryId, t_Data.CountryPic);

                string colorName = string.IsNullOrEmpty(userSelectionDetails.ColorName) ? UIManager.Instance.StartColorName : userSelectionDetails.ColorName;
                SetColorOnCharacter(colorName);

                string characterIdStr = string.IsNullOrEmpty(userSelectionDetails.CharacterId) ? "0" : userSelectionDetails.CharacterId;
                int characterId = int.Parse(characterIdStr);
                UIManager.Instance.ShowCharacterName(raceNames[characterId]);
                horizontalScrollSnap.GoToScreen(characterId);
                playerColorChooser.SetSelectedColor(userSelectionDetails.ColorName);

                FileHandler.SaveToPlayerPrefs(PlayerPrefsValue.SelectedCharacter.ToString(), characterIdStr);
                FileHandler.SaveToPlayerPrefs(PlayerPrefsValue.SelectedRace.ToString(), raceNames[characterId]);
            }
            else
            {
                LoadDefaultData();
            }
        }

        private void OnErrorGetSelectionData(RestUtil.RestCallError obj)
        {
            Debug.Log("Get Data Error: " + obj.Description);
            LoadDefaultData();
        }

        private void LoadDefaultData()
        {
            SetColorOnCharacter(UIManager.Instance.StartColorName);

            UIManager.Instance.ShowCharacterName(raceNames[0]);
            horizontalScrollSnap.GoToScreen(0);

            FileHandler.SaveToPlayerPrefs(PlayerPrefsValue.SelectedCharacter.ToString(), "0");
            FileHandler.SaveToPlayerPrefs(PlayerPrefsValue.SelectedRace.ToString(), raceNames[0]);
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
            FileHandler.SaveToPlayerPrefs(PlayerPrefsValue.SelectedColor.ToString(), colorName);
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
                if (raceNames[i] == PlayerPrefs.GetString(PlayerPrefsValue.SelectedRace.ToString()))
                {
                    horizontalScrollSnap.GoToScreen(i);
                }
            }
          //  horizontalScrollSnap.ChangePage(PlayerPrefs.GetInt(userName, 0));
        }

        public void SetProfilePicOnClick()
        {
            string t_SelectedRace = FileHandler.ReadFromPlayerPrefs(PlayerPrefsValue.SelectedRace.ToString());
            string t_SelectedColor = FileHandler.ReadFromPlayerPrefs(PlayerPrefsValue.SelectedColor.ToString());

            if (!string.IsNullOrEmpty(t_SelectedRace) && !string.IsNullOrEmpty(t_SelectedColor))
                UIManager.Instance.showProfilePic?.Invoke(t_SelectedRace, t_SelectedColor);
            else
                UIManager.Instance.showProfilePic?.Invoke(ERace.Canines.ToString(), EColor.DarkBlue.ToString());
        }

        private void ShowFirstText()
        {
            UIManager.Instance.ShowCharacterName(ERace.Canines.ToString());
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
            Debug.Log($"DartName: {User.DartName}, UserRace: {User.UserRace}");

            FileHandler.SaveToPlayerPrefs(PlayerPrefsValue.SelectedCharacter.ToString(), horizontalScrollSnap._currentPage.ToString());
            FileHandler.SaveToPlayerPrefs(PlayerPrefsValue.SelectedRace.ToString(), raceNames[horizontalScrollSnap._currentPage]);

            SaveUserData();

            setDart?.Invoke(User.DartName);
            SetProfilePicOnClick();
            LevelSelection.Instance.OnSelectionGameplayType(type);
            SocketManager.Instance.ColRequest();

            switch (type)
            {
                case GameType.normal:
                    GameManager.Instance.SetGameplayMode(GameManager.EGamePlayMode.Multiplayer);
                    break;
                case GameType.training:
                    GameManager.Instance.SetGameplayMode(GameManager.EGamePlayMode.Training);
                    break;
            }
        }

        private void SaveUserData()
        {
            UserSelectionDetails userSelectionDetails = new UserSelectionDetails
            {
                ColorName = string.IsNullOrEmpty(User.UserColor) ? UIManager.Instance.StartColorName : User.UserColor,
                RaceName = string.IsNullOrEmpty(User.UserRace) ? ERace.Canines.ToString() : User.UserRace,
                CountryName = string.IsNullOrEmpty(User.UserCountry) ? "in" : User.UserCountry,
                LanguageName = "English",
                CharacterId = horizontalScrollSnap._currentPage.ToString(),
                DartName = User.DartName
            };
            RestManager.SaveUserSelection(User.UserEmailId, userSelectionDetails, delegate { Debug.Log("User selection data is saved."); }, OnErrorSetSelectionData);
        }

        private void OnErrorSetSelectionData(RestUtil.RestCallError obj)
        {
            Debug.Log("Set Data Error: " + obj.Description);
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
