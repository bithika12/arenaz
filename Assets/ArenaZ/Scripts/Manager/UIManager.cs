using ArenaZ.ScreenManagement ;
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
        private Action invokeMandatoryThingsAtStart;

        
        private void Start()
        {
            invokeMandatoryThingsAtStart?.Invoke();
            Debug.Log(Screen.safeArea);
        }

        private void OnEnable()
        {
            invokeMandatoryThingsAtStart += LogInCheck;
            invokeMandatoryThingsAtStart += AddAllUIScreensToDictionary;
            invokeMandatoryThingsAtStart += DeactivateAllUI;
            invokeMandatoryThingsAtStart += StartAnimations;
        }

        private void OnDisable()
        {
            invokeMandatoryThingsAtStart -= LogInCheck;
            invokeMandatoryThingsAtStart -= AddAllUIScreensToDictionary;
            invokeMandatoryThingsAtStart -= DeactivateAllUI;
            invokeMandatoryThingsAtStart -= StartAnimations;
        }

        private void StartAnimations()
        {
            allPages["TopAndBottomBar"].ShowGameObjWithAnim("Straight");
            allPages["AccountAccessDetails"].ShowGameObjWithAnim("Straight");
        }

        private void AddAllUIScreensToDictionary()
        {
            allPages.Clear();
            foreach (UIScreen child in FindObjectsOfType<UIScreen>())
            {
                allPages.Add(child.name, child);
            }

        }

        private void DeactivateAllUI()
        {
            for (int i = 0; i < deativateAllUI.Count; i++)
            {
                if(allPages.ContainsKey(deativateAllUI[i]))
                {
                    allPages[deativateAllUI[i]].Hide();
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
                deativateAllUI.Add("AccountAccessDetails");
            }
        }

    }
}
