using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using UnityEngine.Audio;
using RedApple;
using System;
using ArenaZ.Screens;

namespace ArenaZ.SettingsManagement
{
    public class Settings : Singleton<Settings>
    {
        //Private Properties


        [Header("Audio Mixers")]
        [Space(5)]
        [SerializeField] private AudioMixer MusicAudioMixer;
        [SerializeField] private AudioMixer SFXAudioMixer;

        [Header("Sprites And Images")]
        [Space(5)]
        [SerializeField] private Sprite UnmuteMusicSprite;
        [SerializeField] private Sprite MuteMusicSprite;
        [SerializeField] private Sprite UnmuteSFXSprite;
        [SerializeField] private Sprite MuteSFXSprite;
        [SerializeField] private Image MusicImage;
        [SerializeField] private Image SFXImage;
        [SerializeField] private Image profileImage;
        [SerializeField] private Image selectedColorImage;


        [Header("Text Fields")]
        [Space(5)]
        [SerializeField] private Text userName;

        [Header("Data Types")]
        [Space(5)]
        [SerializeField] private float MuteValue = -80.0f;
        [SerializeField] private float UnmuteValue = 0.0f;

        [Header("Toggles And Buttons")]
        [Space(5)]
        [SerializeField] private Toggle MusicToggle;
        [SerializeField] private Toggle SFXToggle;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button supportButton;
        [SerializeField] private Button logOutButton;
        [SerializeField] private Button languageButton;
        [SerializeField] private Button deleteAccountButton;
        [SerializeField] private Button playerColorButton;

        [Header("Integer and Floating Point")]
        [Space(5)]
        [SerializeField] private float LogOutPopUpCloseduration;

        private bool IsMusicMute = false;
        private bool IsSFXMute = false;
      //  private FacebookLogin facebookLogin;

        private Sprite countrySprite;

        public static Action inputFieldclear;

        //------------------------------------------------------------+
        private void Start()
        {
           // facebookLogin = GetComponent<FacebookLogin>();
            UpdateButtonsOnStart();
            GettingButtonReferences();
            UIManager.Instance.setUserName += SetUserName;
            UIManager.Instance.showProfilePic += SetProfileImage;
            PlayerColorChooser.setColorAfterChooseColor += setSelectedColorImage;
            UIManager.Instance.ScreenShow(Page.LoggedInText.ToString());
        }

        private void OnDestroy()
        {
            ReleaseButtonReferences();
        }

        #region Button_References
        private void GettingButtonReferences()
        {
            closeButton.onClick.AddListener(OnClickClose);

            logOutButton.onClick.AddListener(OnClickLogInLogOut);

            playerColorButton.onClick.AddListener(onClickPlayerColor);

            MusicToggle.onValueChanged.AddListener(delegate
            {
                UpdateMusicValue(MusicToggle);
            });

            SFXToggle.onValueChanged.AddListener(delegate
            {
                UpdateSFXValue(SFXToggle);
            });
        }

        private void ReleaseButtonReferences()
        {
            closeButton.onClick.RemoveListener(OnClickClose);

            logOutButton.onClick.RemoveListener(OnClickLogInLogOut);

            playerColorButton.onClick.RemoveListener(onClickPlayerColor);

            MusicToggle.onValueChanged.RemoveListener(delegate
            {
                UpdateMusicValue(MusicToggle);
            });

            SFXToggle.onValueChanged.RemoveListener(delegate
            {
                UpdateSFXValue(SFXToggle);
            });
        }
        #endregion

        public void LogInLogOutButtonNameSet(string buttonName)
        {
            logOutButton.transform.GetChild(0).GetComponent<Text>().text = buttonName;
        }

        private void TasksAfterLogout()
        {
            UIManager.Instance.SetComponent<Text>(Page.LoggedInText.ToString(), false);
            LogInLogOutButtonNameSet(ConstantStrings.login);
            UIManager.Instance.showProfilePic?.Invoke((Race.Canines.ToString()));
            UIManager.Instance.setUserName?.Invoke(ConstantStrings.defaultUserName);
            TopAndBottomBarScreen.Instance.count = 0;
            // UIManager.Instance.ShowPopWithText(Page.PopUpTextSettings.ToString(), successFullyLoggedOut, PopUpduration);
        }

        private void SetProfileImage(string imageName)
        {
            profileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Small);
        }

        private void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        private void setSelectedColorImage(string imageName)
        {
            User.userColor = imageName;
            ButtonImage buttonImage = UIManager.Instance.ButtonImageType(imageName);
            if(buttonImage.normalSprite)
            {
                selectedColorImage.sprite = buttonImage.normalSprite;
            }
        }

        #region UI_Functionalities

        private void onClickPlayerColor()
        {
            UIManager.Instance.ToggleScreenWithAnim(Page.PlayerColorChooser.ToString());
        }

        private void OnClickClose()
        {
            //toggleAnimation = false;
            //UIManager.Instance.HideScreenImmediately(Page.PlayerColorChooser.ToString());
            UIManager.Instance.ToggleScreenImmediately(Page.PlayerColorChooser.ToString());
            UIManager.Instance.HideScreen(Page.SettingsPanel.ToString());
        }

        private void OnClickLogInLogOut()
        {
            if (PlayerPrefs.GetInt(PlayerprefsValue.Logout.ToString()) == 0)
            {
                Debug.Log("Logged Out");
                UIManager.Instance.ScreenShow(Page.LogOutAlertOverlay.ToString());
            }
            else
            {
                Debug.Log("Not Logged Out");
                GoToLogIn();
            }
        }

        public void AfterCompleteLogout()
        {
            TasksAfterLogout();
            UIManager.Instance.ScreenShow(Page.AccountAccessDetailsPanel.ToString(), Hide.none);
            UIManager.Instance.HideScreen(Page.CharacterSelectionPanel.ToString());
            PlayerPrefs.SetInt(PlayerprefsValue.Logout.ToString(), 1);
            PlayerPrefs.SetInt(PlayerprefsValue.AutoLogin.ToString(), 0);
            UIManager.Instance.HideScreenImmediately(Page.SettingsPanel.ToString());
        }

        private void GoToLogIn()
        {
            UIManager.Instance.HideScreenImmediately(Page.SettingsPanel.ToString());
            UIManager.Instance.ScreenShow(Page.AccountAccessDetailsPanel.ToString(), Hide.none);
            inputFieldclear?.Invoke();
        }

        #endregion
        #region Music_AND_SFX
        private void UpdateMusicValue(Toggle stateChange)
        {
            Debug.Log("MusicValue::" + stateChange.isOn);
            IsMusicMute = stateChange.isOn;

            if (IsMusicMute)
            {
                MusicImage.sprite = MuteMusicSprite;
                MusicAudioMixer.SetFloat("MusicVolume", MuteValue);

                PlayerPrefs.SetFloat("MusicVolume", MuteValue);
            }
            else if (!IsMusicMute)
            {
                MusicImage.sprite = UnmuteMusicSprite;
                MusicAudioMixer.SetFloat("MusicVolume", UnmuteValue);

                PlayerPrefs.SetFloat("MusicVolume", UnmuteValue);
            }

            Debug.Log("IsMusic is " + IsMusicMute);
        }

        private void UpdateSFXValue(Toggle stateChange)
        {
            Debug.Log("SfxValue::" + stateChange.isOn);
            IsSFXMute = stateChange.isOn;

            if (IsSFXMute)
            {
                SFXImage.sprite = MuteSFXSprite;
                SFXAudioMixer.SetFloat("SFXVolume", MuteValue);

                PlayerPrefs.SetFloat("SFXVolume", MuteValue);
            }
            else if (!IsSFXMute)
            {
                SFXImage.sprite = UnmuteSFXSprite;
                SFXAudioMixer.SetFloat("SFXVolume", UnmuteValue);

                PlayerPrefs.SetFloat("SFXVolume", UnmuteValue);
            }

            Debug.Log("IsSFX is " + IsSFXMute);
        }

        private void UpdateButtonsOnStart()
        {
            if (PlayerPrefs.GetFloat("MusicVolume") == MuteValue)
            {
                MusicImage.sprite = MuteMusicSprite;
                MusicAudioMixer.SetFloat("MusicVolume", MuteValue);
                IsMusicMute = true;
                MusicToggle.isOn = true;
            }

            if (PlayerPrefs.GetFloat("MusicVolume") == UnmuteValue)
            {
                MusicImage.sprite = UnmuteMusicSprite;
                MusicAudioMixer.SetFloat("MusicVolume", UnmuteValue);
                IsMusicMute = false;
                MusicToggle.isOn = false;
            }


            if (PlayerPrefs.GetFloat("SFXVolume") == MuteValue)
            {
                SFXImage.sprite = MuteSFXSprite;
                SFXAudioMixer.SetFloat("SFXVolume", MuteValue);
                IsSFXMute = true;
                SFXToggle.isOn = true;
            }

            if (PlayerPrefs.GetFloat("SFXVolume") == UnmuteValue)
            {
                SFXImage.sprite = UnmuteSFXSprite;
                SFXAudioMixer.SetFloat("SFXVolume", UnmuteValue);
                IsSFXMute = false;
                SFXToggle.isOn = false;
            }
        }
        #endregion

    }
}
