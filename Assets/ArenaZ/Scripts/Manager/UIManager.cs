﻿using ArenaZ.Screens;
using System.Collections.Generic;
using UnityEngine;
using System;
using ArenaZ.Data;
using UnityEngine.U2D;
using RedApple;
using System.Text;
using System.Collections;

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
        public Action<string, string> userLogin;

        protected override void Awake()
        {
            AddAllTextScreensToDictionary();
            AddAllUIScreensToDictionary();
        }

        private void Start()
        {
            StartAnimations();
            StartCoroutine(logInCheck());
        }
        private void StartAnimations()
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
                Debug.Log("Pushing....."+screenName);
                _openPages.Push(screenName);
                Debug.Log("stack count: " + _openPages.Count);
            }            
        }

        private void hidePreviousScreens()
        {
            if (_openPages.Count > 0)
            {
                for (int i =_openPages.Count; i > 0; i--)
                {
                    Debug.Log("Hiding..."+ _openPages.Peek());
                    HideScreenImmediately(_openPages.Peek());
                    _openPages.Pop();
                    Debug.Log("stack count: " + _openPages.Count);
                }
            }
        }

        public void ShowScreen(string screenName)
        {
            if (!allPages.ContainsKey(screenName) && allPages[screenName].gameObject.activeInHierarchy)
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
            if (!allPages.ContainsKey(screenName) && !allPages[screenName].gameObject.activeInHierarchy)
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

        public void ToggleScreenWithAnim(string screenName)
        {
            if (!allPages[screenName].gameObject.activeSelf)
            {
                allPages[screenName].ShowGameObjWithAnim();
            }
            else
            {
                allPages[screenName].HideGameObjWithAnim();
            }
        }

        public void ToggleScreenImmediately(string screenName)
        {
            if (!allPages[screenName].gameObject.activeSelf)
            {
                allPages[screenName].Show();
            }
            else
            {
                allPages[screenName].Hide();
            }
        }

        public void SetComponent<T>(string screenName, bool value)
        {
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

        private byte[] getByteFromString(string anyString)
        {
            return Encoding.ASCII.GetBytes(anyString);
        }

        public void SaveDetails(string filename, string data)
        {
            byte[] dataBytes = getByteFromString(data);
            //PlayerPrefs.SetString(filename, data);
            DataSaveAndLoad dataSaveAndLoad = new DataSaveAndLoad(filename, filename, dataBytes);
            Debug.Log("Saving...  " + filename);
            dataSaveAndLoad.SaveToDisk(dataBytes);
        }

        private string loadDetails(string fileName)
        {
            DataSaveAndLoad fileManagement = new DataSaveAndLoad(fileName, fileName);
            fileManagement.LoadDataFromStorage();
            byte[] details = fileManagement.LoadedData;
            Debug.Log("Data :::::: " + Encoding.ASCII.GetString(details));
            return Encoding.ASCII.GetString(details);
        }

        public void DeleteDetails(string fileName)
        {
            DataSaveAndLoad fileManagement = new DataSaveAndLoad(fileName, fileName);
            fileManagement.DeleteFile();
        }

        private IEnumerator logInCheck()
        {
            ShowScreen(Page.AccountAccesOverlay.ToString());
            yield return new WaitForSeconds(ConstantInteger.autoLoginWait);
            string savedLoginID = loadDetails(PlayerprefsValue.LoginID.ToString());
            string savedPassword = loadDetails(PlayerprefsValue.Password.ToString());
            Debug.Log("Loaded String:  " + savedLoginID + "   " + savedPassword);
            if (savedLoginID!=string.Empty && savedPassword!=string.Empty)
            {
                Debug.Log("Matched");
                ShowScreen(Page.LogINOverlay.ToString());
                userLogin?.Invoke(savedLoginID, savedPassword);
                PlayerPrefs.SetInt(PlayerprefsValue.AutoLogin.ToString(), 1);
            }
            else
            {
                PlayerPrefs.SetInt(PlayerprefsValue.Logout.ToString(), 1);
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
        public Sprite roundSprite;
    }
}
