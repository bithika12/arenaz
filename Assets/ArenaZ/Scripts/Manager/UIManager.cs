using ArenaZ.Screens;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace ArenaZ.Manager
{
    /// <summary>
    /// Add this class to the Main Canvas from where all the ui screens will be accesible
    /// </summary>
    public class UIManager : RedAppleSingleton<UIManager>
    {
        //Public Variables
        //Private Variables
        [SerializeField]
        private List<string> deativateAllUI;
        [SerializeField]
        private List<string> allPanels;
        private Dictionary<string, UIScreen> allPages = new Dictionary<string, UIScreen>();

        private string _openScreen;

        private void Start()
        {
            Debug.Log(UnityEngine.Screen.safeArea);
        }

        private void OnEnable()
        {
            LogInCheck();
            StartAnimations();
        }

        private void StartAnimations()
        {
            allPages[Page.TopAndBottomBar.ToString()].ShowGameObjWithAnim();
            allPages[Page.AccountAccessDetails.ToString()].ShowGameObjWithAnim();
        }

        public void ShowScreen(string screenName)
        {
            if (_openScreen.Equals(screenName))
                return;

            allPages[screenName].ShowGameObjWithAnim();
            if (allPages.ContainsKey(_openScreen))
                allPages[_openScreen].HideGameObjWithAnim();
            _openScreen = screenName;
        }

        private void AddAllUIScreensToDictionary()
        {
            allPages.Clear();
            foreach (UIScreen screen in FindObjectsOfType<UIScreen>())
            {
                allPages.Add(screen.name, screen);
            }
            DeactivateAllUI();
        }

        private void DeactivateAllUI()
        {
            foreach(KeyValuePair<string, UIScreen> child in allPages)
            {
                child.Value.gameObject.SetActive(false);
            }
        }

        public void DeactivateOtherPanels(string myPanel)
        {
            List<string> uiNames = allPanels;
            for (int i = 0; i < uiNames.Count; i++)
            {
                if (allPages.ContainsKey(uiNames[i]) && myPanel!=uiNames[i])
                {
                    allPages[uiNames[i]].HideGameObjWithAnim();
                }
            }
        }

        private void LogInCheck()
        {
            if (PlayerPrefs.GetInt("AlreadyLoggedIn") == 1)
            {
                deativateAllUI.Add("AccountAccessDetails");
            }
            AddAllUIScreensToDictionary();
        }

    }
}
