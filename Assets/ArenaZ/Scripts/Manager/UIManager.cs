using ArenaZ.Screens;
using ArenaZ.AccountAccess;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.U2D;

namespace ArenaZ.Manager
{
    /// <summary>
    /// Add this class to the Main Canvas from where all the ui screens will be accesible
    /// </summary>
    public class UIManager : RedAppleSingleton<UIManager>
    {
        //Public Variables
        //Private Variables
        private Dictionary<string, UIScreen> allPages = new Dictionary<string, UIScreen>();
        private string _openScreen = string.Empty;
        private string closeScreen = string.Empty;
        private string characterName = string.Empty;

        [Header("SpriteAtlas")][Space(5)]
        [SerializeField] private SpriteAtlas countryAtlas;

        [Header("Button Images")][Space(5)]
        [SerializeField]private ImageType[] allButtonImages = new ImageType[17];

        [Header("Profile Image")][Space(5)]
        [SerializeField] private ProfileImageType[] smallProfilePic = new ProfileImageType[6];
        [SerializeField] private ProfileImageType[] mediumProfilePic = new ProfileImageType[6];

        private void Start()
        {
            StartCoroutine(LogInCheck());
            StartAnimations();
        }
        public Sprite GetCorrespondingCountrySprite()
        {
            return countryAtlas.GetSprite(AccountAccessManager.Instance.CountryId.ToLower());
        }

        public Sprite GetCorrespondingProfileSprite(string charName,ProfilePic type)
        {
            ProfileImageType[] pics;
            if (type == ProfilePic.Small)
            {
                pics = smallProfilePic;
            }
            else
            {
                pics = mediumProfilePic;
            }
            for (int i = 0; i < pics.Length; i++)
            {
                if(pics[i].profileImageName == charName)
                {
                    return pics[i].sprite;
                }
            }
            return null;
        }

        private void StartAnimations()
        {
            ShowScreen(Page.TopAndBottomBar.ToString(), Hide.none);
        }

        public void ShowPopWithText(string screenName,string message,float duration)
        {
            StartCoroutine(allPages[screenName].ShowAndHidePopUpText(message, duration));
        }

        public void ShowScreen(string screenName,Hide type)
        {
            if (_openScreen.Equals(screenName) || !allPages.ContainsKey(screenName))
            {
                return;
            }
            allPages[screenName].ShowGameObjWithAnim();
            if (_openScreen==string.Empty && type == Hide.previous)
            {
                _openScreen = screenName;
            }
            else if(allPages.ContainsKey(_openScreen) && type == Hide.previous)
            {
                allPages[_openScreen].SetActive(false);
                _openScreen = screenName;
            }                           
            closeScreen = string.Empty;
        }

        public void ShowCharacterName(string name)
        {
            if (characterName.Equals(name) || !allPages.ContainsKey(name))
            {
                return;
            }
            allPages[name].ShowGameObjWithAnim();
            if (allPages.ContainsKey(characterName))
            {
                allPages[characterName].SetActive(false);
            }
            characterName = name;
        }

        public ImageType ButtonImageType(ButtonType type)
        {
            for (int i = 0; i < allButtonImages.Length; i++)
            {
                if(allButtonImages[i].buttonType == type)
                {
                    return allButtonImages[i];
                }
            }
            return default;
        }

        public void HideScreen(string screenName)
        {
            Debug.Log("Hide Screen" + name);
            if (closeScreen.Equals(screenName) || !allPages.ContainsKey(screenName))
            {
                return;
            }
            allPages[screenName].HideGameObjWithAnim();
            closeScreen = screenName;
            _openScreen = string.Empty;
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
                child.Value.gameObject.SetActive(false);
            }
        }

        IEnumerator LogInCheck()
        {
            yield return new WaitUntil(()=> AddAllUIScreensToDictionary()==true);
            if (PlayerPrefs.GetInt("AlreadyLoggedIn") == 0)
            {
                ShowScreen(Page.AccountAccessDetails.ToString(), Hide.none);
            }            
        }

    }

    [Serializable]
    public struct ImageType
    {
        public ButtonType buttonType;
        public Sprite normalSprite;
        public Sprite pressedSprite;
        public Sprite disabledSprite;      
    }

    [Serializable]
    public struct ProfileImageType
    {
        public string profileImageName;
        public Sprite sprite;
    }
}
