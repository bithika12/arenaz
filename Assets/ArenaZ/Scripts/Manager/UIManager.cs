using ArenaZ.Screens;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

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
        [SerializeField]private ImageType[] allButtonImages = new ImageType[17];

        private void Start()
        {
            StartCoroutine(LogInCheck());
            StartAnimations();
        }

        private void StartAnimations()
        {
            ShowScreen(Page.TopAndBottomBar.ToString(), Hide.none);
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

}
