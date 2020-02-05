using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using UnityEngine.Audio;
using RedApple;
using System;
using ArenaZ.Screens;
using ArenaZ.Data;
using DevCommons.Utility;

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

        [Header("User Image")]
        [SerializeField] private Image userFrame;
        [SerializeField] private Image userPic;

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
            UIManager.Instance.showProfilePic += SetUserProfileImage;
            PlayerColorChooser.setColorAfterChooseColor += setSelectedColorImage;
            UIManager.Instance.ShowScreen(Page.LoggedInText.ToString());
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
            UIManager.Instance.DeleteDetails(PlayerPrefsValue.LoginID.ToString());
            UIManager.Instance.DeleteDetails(PlayerPrefsValue.Password.ToString());
            UIManager.Instance.SetComponent<Text>(Page.LoggedInText.ToString(), false);
            LogInLogOutButtonNameSet(ConstantStrings.login);
            UIManager.Instance.showProfilePic?.Invoke(ERace.Canines.ToString(), EColor.DarkBlue.ToString());
            UIManager.Instance.setUserName?.Invoke(ConstantStrings.defaultUserName);
            PlayerPrefs.SetInt(PlayerPrefsValue.Logout.ToString(), 1);
            // UIManager.Instance.ShowPopWithText(Page.PopUpTextSettings.ToString(), successFullyLoggedOut, PopUpduration);
        }

        public void SetUserProfileImage(string race, string color)
        {
            ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), race);
            EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), color);

            SquareFrameData t_FrameData = DataHandler.Instance.GetSquareFrameData(t_Color);
            if (t_FrameData != null)
            {
                userFrame.sprite = t_FrameData.FramePic;
            }

            CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
            if (t_CharacterPicData != null)
            {
                userPic.sprite = t_CharacterPicData.ProfilePic;
            }
        }

        private void SetUserName(string userName)
        {
            this.userName.text = userName;
        }

        private void setSelectedColorImage(string imageName)
        {
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
            UIManager.Instance.HideScreenImmediately(Page.PlayerColorChooser.ToString());
            UIManager.Instance.HideScreen(Page.SettingsPanel.ToString());
        }

        private void OnClickLogInLogOut()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsValue.Logout.ToString()) == 0)
            {
                Debug.Log("Logged Out");
                UIManager.Instance.ShowScreen(Page.LogOutAlertOverlay.ToString(),Hide.none);
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
            UIManager.Instance.ShowScreen(Page.AccountAccessDetailsPanel.ToString());
            UIManager.Instance.ShowScreen(Page.AccountAccesOverlay.ToString());
            UIManager.Instance.HideScreen(Page.CharacterSelectionPanel.ToString());
            UIManager.Instance.HideScreenImmediately(Page.SettingsPanel.ToString());
        }

        private void GoToLogIn()
        {
            UIManager.Instance.HideScreenImmediately(Page.SettingsPanel.ToString());
            UIManager.Instance.ShowScreen(Page.AccountAccessDetailsPanel.ToString());
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
