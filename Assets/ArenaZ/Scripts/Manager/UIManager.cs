using ArenaZ.Screens;
using System.Collections.Generic;
using UnityEngine;
using System;
using ArenaZ.Data;
using UnityEngine.U2D;
using RedApple;
using System.Text;
using System.Collections;
using System.Linq;

namespace ArenaZ.Manager
{
    /// <summary>
    /// Add this class to the Main Canvas from where all the ui screens will be accesible
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        //Private Variables
        private Dictionary<string, UIScreen> allPages = new Dictionary<string, UIScreen>();
        private Dictionary<string, TextScreen> textPages = new Dictionary<string, TextScreen>();
        private Stack<string> _openPages = new Stack<string>();
        private Stack<string> _closedPages = new Stack<string>();
        private string characterName = string.Empty;
        private string startColorName = ButtonType.DarkGreen.ToString();
        public string StartColorName { get { return startColorName; } private set { } }

        [Header("SpriteAtlas")][Space(5)]
        [SerializeField] private SpriteAtlas countryAtlas;

        [Header("Button Images")][Space(5)]
        [SerializeField]private ButtonImage[] allButtonImages = new ButtonImage[28];

        [Header("Profile Image")][Space(5)]
        [SerializeField] private ProfileImage[] ProfilePic = new ProfileImage[7];

        //Public Variables
        public Action<string> showProfilePic;
        public Action<string> setUserName;

        protected override void Awake()
        {
            AddAllTextScreensToDictionary();
            AddAllUIScreensToDictionary();
        }

        private void Start()
        {
            enableGameStartPages();
        }
        private void enableGameStartPages()
        {
            ShowScreen(Page.UIPanel.ToString());
            ShowScreen(Page.TopAndBottomBarPanel.ToString());
            ShowScreen(Page.AccountAccessDetailsPanel.ToString());
        }


        public Sprite GetCorrespondingCountrySprite(string spriteName)
        {
            return countryAtlas.GetSprite(spriteName);
        }

        public Sprite GetProfile(string charName,ProfilePicType type)
        {
            for (int i = 0; i < ProfilePic.Length; i++)
            {
                if (ProfilePic[i].profileImageName.Equals(charName) && type == ProfilePicType.Small)
                {
                    return ProfilePic[i].smallSprite;
                }
                else if(ProfilePic[i].profileImageName.Equals(charName) && type == ProfilePicType.Medium)
                {
                    return ProfilePic[i].mediumSprite;
                }
                else if (ProfilePic[i].profileImageName.Equals(charName) && type == ProfilePicType.rounded)
                {
                    return ProfilePic[i].roundSprite;
                }
            }
            return null;
        }

        public void ShowPopWithText(string screenName,string message,float duration)
        {
            StopAllCoroutines();
            StartCoroutine(allPages[screenName].ShowAndHidePopUpText(message, duration));
        }

        public void ShowScreen(string screenName, Hide type)
        {
            if (allPages.ContainsKey(screenName) && type == Hide.previous)
            {
                hidePreviousScreens();
            }
            ShowScreen(screenName);            
            if (!_openPages.Contains(screenName))
            {
                _openPages.Push(screenName);
            }            
        }

        private void hidePreviousScreens()
        {
            if (_openPages.Count > 0)
            {
                for (int i =_openPages.Count; i > 0; i--)
                { 
                    HideScreenImmediately(_openPages.Peek());
                    _openPages.Pop();
                }
            }
        }

        public void ClearOpenPagesContainer()
        {
            _openPages.Clear();
        }

        public void HideOpenScreen()
        {
            if(_openPages.Count > 0)
            {
                string screenName = _openPages.Peek();
                Debug.Log($"HideOpenScreen: {screenName}");
                HideScreenImmediately(_openPages.Peek());
                _openPages.Pop();
            }           
        }

        public void ShowScreen(string screenName)
        {
            Debug.Log($"ShowScreen: {screenName}");
            if (!allPages.ContainsKey(screenName) || allPages[screenName].gameObject.activeInHierarchy)
                return;
            if (allPages.ContainsKey(screenName))
                allPages[screenName].ShowGameObjWithAnim();
        }

        public void ShowCharacterName(string name)
        {
            if (characterName.Equals(name) || !textPages.ContainsKey(name))
            {
                return;
            }
            textPages[name].Show();
            if (textPages.ContainsKey(characterName))
            {
                textPages[characterName].Hide();
            }
            characterName = name;
        }

        public void HideScreen(string screenName)
        {
            Debug.Log($"HideScreen: {screenName}");
            if (!allPages.ContainsKey(screenName) || !allPages[screenName].gameObject.activeInHierarchy)
                return;
            if (allPages.ContainsKey(screenName))
                allPages[screenName].HideGameObjWithAnim();
        }

        public void HideScreenImmediately(string screenName)
        {
            Debug.Log($"HideScreenImmediately: {screenName}");
            if (!allPages.ContainsKey(screenName))
                return;
            if (allPages[screenName].gameObject.activeInHierarchy)
                allPages[screenName].Hide();
        }

        public void ShowScreenImmediately(string screenName)
        {
            if (!allPages.ContainsKey(screenName))
                return;
            if (!allPages[screenName].gameObject.activeInHierarchy)
                allPages[screenName].Show();
        }

        public void ToggleScreenWithAnim(string screenName)
        {
            if (!allPages.ContainsKey(screenName))
                return;
            if (!allPages[screenName].gameObject.activeSelf)
                allPages[screenName].ShowGameObjWithAnim();
            else
                allPages[screenName].HideGameObjWithAnim();
        }

        public void ToggleScreenImmediately(string screenName)
        {
            if (!allPages.ContainsKey(screenName))
                return;
            if (!allPages[screenName].gameObject.activeSelf)
                allPages[screenName].Show();
            else
                allPages[screenName].Hide();
        }

        public void SetComponent<T>(string screenName, bool value)
        {
            if (allPages.ContainsKey(screenName))
                allPages[screenName].EnableDisableComponent<T>(value);
        }

        public ButtonImage ButtonImageType(string type)
        {
            for (int i = 0; i < allButtonImages.Length; i++)
            {
                if (allButtonImages[i].buttonType.ToString() == type)
                {
                    return allButtonImages[i];
                }
            }
            return default;
        }


        private bool AddAllUIScreensToDictionary()
        {
            allPages.Clear();
            UIScreen[] allScreens = Resources.FindObjectsOfTypeAll(typeof(UIScreen)) as UIScreen[]; //FindObjectsOfType<UIScreen>().ToList();
            List<UIScreen> screens = allScreens.ToList();
            screens.ForEach(x => allPages.Add(x.name, x));
            DeactivateAllUI();
            return true;
        }

        private void DeactivateAllUI()
        {
            foreach(KeyValuePair<string, UIScreen> child in allPages)
            {
                child.Value.Hide();
            }
        }

        private void AddAllTextScreensToDictionary()
        {
            textPages.Clear();
            foreach (TextScreen screen in FindObjectsOfType<TextScreen>())
            {
                textPages.Add(screen.name, screen);
            }
            DeactivateAllTextPages();
        }

        private void DeactivateAllTextPages()
        {
            foreach (KeyValuePair<string, TextScreen> child in textPages)
            {
                child.Value.Hide();
            }
        }

        private byte[] getByteFromString(string anyString)
        {
            return Encoding.ASCII.GetBytes(anyString);
        }

        public void SaveDetails(string filename, string data)
        {
            byte[] dataBytes = getByteFromString(data);
            DataSaveAndLoad dataSaveAndLoad = new DataSaveAndLoad(filename, filename, dataBytes);
            Debug.Log("Saving...  " + filename);
            dataSaveAndLoad.SaveToDisk(dataBytes);
        }

        public string LoadDetails(string fileName)
        {
            DataSaveAndLoad fileManagement = new DataSaveAndLoad(fileName, fileName);
            fileManagement.LoadDataFromStorage();
            byte[] details = fileManagement.LoadedData;
            Debug.Log("Fetched Data :::::: " + Encoding.ASCII.GetString(details));
            return Encoding.ASCII.GetString(details);
        }

        public void DeleteDetails(string fileName)
        {
            DataSaveAndLoad fileManagement = new DataSaveAndLoad(fileName, fileName);
            fileManagement.DeleteFile();
        }
    }

    [Serializable]
    public struct ButtonImage
    {
        public ButtonType buttonType;
        public Sprite normalSprite;
        public Sprite pressedSprite;
        public Sprite disabledSprite;      
    }

    [Serializable]
    public struct ProfileImage
    {
        [HideInInspector]
        public ProfilePicType profilePicType;
        public string profileImageName;
        public Sprite smallSprite;
        public Sprite mediumSprite;
        public Sprite roundSprite;
    }
}
