using ArenaZ.Screen ;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ArenaZ.Manager
{
    /// <summary>
    /// Add this class to the Main Canvas from where all the ui screens will be accesible
    /// </summary>
    public class UIManager : RedAppleSingleton<UIManager>
    {
        //Public Variables
        public Dictionary<string, UIScreen> allPages = new Dictionary<string, UIScreen>();
        public List<string> deactivateUnusableUIOnStart;
        public List<string> allPanels;

        //Private Variables
        protected override void Awake()
        {
           // PlayerPrefs.SetInt("AlreadyLoggedIn", 0);
        }

        private void Start()
        {
            LogInCheck();
            AddAllUIScreensToDictionary();
            DeactivateNonUsableUI(deactivateUnusableUIOnStart);
        }

        private void AddAllUIScreensToDictionary()
        {
            allPages.Clear();
            foreach (UIScreen child in FindObjectsOfType<UIScreen>())
            {
                allPages.Add(child.name, child);
            }
        }
        public void DeactivateNonUsableUI(List<string> uiNames)
        {
            for (int i = 0; i < uiNames.Count; i++)
            {
                if(allPages.ContainsKey(uiNames[i]))
                {
                    allPages[uiNames[i]].Hide();
                }
            }
        }
        public void DeactivateOtherPanels(string myPanel)
        {
            List<string> uiNames = allPanels;
            for (int i = 0; i < uiNames.Count; i++)
            {
                if (allPages.ContainsKey(uiNames[i]) && myPanel!=uiNames[i])
                {
                    allPages[uiNames[i]].Hide();
                }
            }
        }

        private void LogInCheck()
        {
            if (PlayerPrefs.GetInt("AlreadyLoggedIn") == 1)
            {
                deactivateUnusableUIOnStart.Add("AccountAccessDetails");
            }
        }

        public void ShowScreen()
        {
            
        }

        public void ShowPopUp()
        {

        }
    }
}
