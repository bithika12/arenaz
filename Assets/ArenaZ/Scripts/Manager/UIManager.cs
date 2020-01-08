using ArenaZ.Screens;
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
        private string _openScreen = string.Empty;
        private string closeScreen = string.Empty;
        private string characterName = string.Empty;
        private string startColorName = ButtonType.DarkGreen.ToString();
        public string StartColorName { get { return startColorName; } }

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
#if UNITY_EDITOR
            ScreenShow(Page.AccountAccesOverlay.ToString());
            PlayerPrefs.SetInt(PlayerprefsValue.Logout.ToString(), 1);
#elif UNITY_ANDROID
            logInCheck();
#endif
        }
        private void StartAnimations()
        {
            ScreenShow(Page.UIPanel.ToString(), Hide.none);
            ScreenShow(Page.TopAndBottomBarPanel.ToString(), Hide.none);
            ScreenShow(Page.AccountAccessDetailsPanel.ToString(), Hide.none);
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

        public void ScreenShow(string screenName)
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

        public void HideScreenWithAnim(string screenName)
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

        private string getFileName(string fileName)
        {
            return string.Format("Arena_{0}.dat", fileName);
        }

        private byte[] getByteFromString(string anyString)
        {
            return Encoding.ASCII.GetBytes(anyString);
        }

        public void SaveDetails(string filename, string data)
        {
            byte[] dataBytes = getByteFromString(data);
            PlayerPrefs.SetString(filename, data);
            DataSaveAndLoad dataSaveAndLoad = new DataSaveAndLoad(filename, filename, dataBytes);
            Debug.Log("Saving...  " + filename);
            dataSaveAndLoad.SaveToDisk(dataBytes);
        }

        private string loadDetails(string filename)
        {
            DataSaveAndLoad fileManagement = new DataSaveAndLoad(filename, filename);
            fileManagement.LoadDataFromStorage();
            byte[] details = fileManagement.LoadedData;
            Debug.Log("Data :::::: " + Encoding.ASCII.GetString(details));
            return Encoding.ASCII.GetString(details);
        }

        private IEnumerator logInCheck()
        {
            yield return new WaitForSeconds(ConstantInteger.autoLoginWait);
            string savedLoginID = loadDetails(PlayerprefsValue.LoginID.ToString());
            string savedPassword = loadDetails(PlayerprefsValue.Password.ToString());
            Debug.Log("PlayerPrefs: " + PlayerPrefs.GetString(PlayerprefsValue.LoginID.ToString()) + "   " + PlayerPrefs.GetString(PlayerprefsValue.Password.ToString()) + "   Loaded String:  " + savedLoginID + "   " + savedPassword);
            if (PlayerPrefs.GetString(PlayerprefsValue.LoginID.ToString()) != string.Empty && PlayerPrefs.GetString(PlayerprefsValue.Password.ToString()) != string.Empty)
            {
                if (PlayerPrefs.GetString(PlayerprefsValue.LoginID.ToString()) == savedLoginID && PlayerPrefs.GetString(PlayerprefsValue.Password.ToString()) == savedPassword)
                {
                    Debug.Log("Matched");
                    ScreenShow(Page.AccountAccesOverlay.ToString());
                    ScreenShow(Page.LogINOverlay.ToString());
                    userLogin?.Invoke(savedLoginID, savedPassword);
                    PlayerPrefs.SetInt(PlayerprefsValue.AutoLogin.ToString(), 1);
                }
            }
            else
            {
                ScreenShow(Page.AccountAccesOverlay.ToString());
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
