using ArenaZ.Screen ;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ.Manager
{
    /// <summary>
    /// Add this class to the Main Canvas from where all the ui screens will be accesible
    /// </summary>
    public class UIManager : RedAppleSingleton<UIManager>
    {
        public Dictionary<string, UIScreen> allPages = new Dictionary<string, UIScreen>();

        private void Start()
        {
            AddAllUIScreensToDictionary();
        }

        private void AddAllUIScreensToDictionary()
        {
            allPages.Clear();
            foreach(UIScreen child in FindObjectsOfType<UIScreen>())
            {
                allPages.Add(child.name, child);
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
