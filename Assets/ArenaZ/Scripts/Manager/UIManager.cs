using ArenaZ.Screens;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.U2D;
using RedApple;

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
        private string _openScreen = string.Empty;
        private string closeScreen = string.Empty;
        private string characterName = string.Empty;

        [Header("SpriteAtlas")][Space(5)]
        [SerializeField] private SpriteAtlas countryAtlas;

        [Header("Button Images")][Space(5)]
        [SerializeField]private ButtonImage[] allButtonImages = new ButtonImage[17];

        [Header("Profile Image")][Space(5)]
        [SerializeField] private ProfileImage[] ProfilePic = new ProfileImage[7];

        //Public Variables
        public Action<string> showProfilePic;
        public Action<string> setUserName;

        protected override void Awake()
        {
            AddAllTextScreensToDictionary();
            StartCoroutine(LogInCheck());
        }

        private void Start()
        {
            StartAnimations();          
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
            }
            return null;
        }

        private void StartAnimations()
        {
            ScreenShow(Page.UIPanel.ToString(), Hide.none);
            ScreenShow(Page.TopAndBottomBarPanel.ToString(), Hide.none);
            ScreenShow(Page.AccountAccesOverlay.ToString(), Hide.none);
        }

        public void ShowPopWithText(string screenName,string message,float duration)
        {
            StartCoroutine(allPages[screenName].ShowAndHidePopUpText(message, duration));
        }

        public void ScreenShow(string screenName,Hide type)
        {
            if (_openScreen.Equals(screenName) || !allPages.ContainsKey(screenName) || allPages[screenName].gameObject.activeInHierarchy)
            {
                Debug.Log("Return When Showing");
                return;
            }
            allPages[screenName].ShowGameObjWithAnim();
            if (_openScreen==string.Empty && type == Hide.previous)
            {
                _openScreen = screenName;
            }
            else if(allPages.ContainsKey(_openScreen) && type == Hide.previous)
            {
                allPages[_openScreen].Hide();
                _openScreen = screenName;
            }                           
            closeScreen = string.Empty;
        }

        public void ScreenShowNormal(string screenName)
        {
            if (_openScreen.Equals(screenName) || !allPages.ContainsKey(screenName))
            {
                return;
            }
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
            if (closeScreen.Equals(screenName) || !allPages.ContainsKey(screenName))
            {
                return;
            }
            allPages[screenName].HideGameObjWithAnim();
            closeScreen = screenName;
            _openScreen = string.Empty;
        }

        public void HideScreenNormalWithAnim(string screenName)
        {
            if (closeScreen.Equals(screenName) || !allPages.ContainsKey(screenName))
            {
                return;
            }
            allPages[screenName].HideGameObjWithAnim();
        }

        public void HideScreenImmediately(string screenName)
        {
            if (allPages.ContainsKey(screenName) && allPages[screenName].gameObject.activeInHierarchy)
            {
                allPages[screenName].Hide();
            }
        }

        public void SetComponent<T>(string screenName, bool value)
        {
            allPages[screenName].EnableDisableComponent<T>(value);
        }

        public ButtonImage ButtonImageType(ButtonType type)
        {
            for (int i = 0; i < allButtonImages.Length; i++)
            {
                if (allButtonImages[i].buttonType == type)
                {
                    return allButtonImages[i];
                }
            }
            return default;
        }


        private bool AddAllUIScreensToDictionary()
        {
            allPages.Clear();
            foreach (UIScreen screen in FindObjectsOfType<UIScreen>())
            {
                allPages.Add(screen.name, screen);
            }
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

        IEnumerator LogInCheck()
        {
            yield return new WaitUntil(()=> AddAllUIScreensToDictionary());
            if (PlayerPrefs.GetInt("AlreadyLoggedIn") == 0)
            {
                ScreenShow(Page.AccountAccessDetailsPanel.ToString(), Hide.none);
                PlayerPrefs.SetInt("Logout", 1);
            }            
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
    }
}
