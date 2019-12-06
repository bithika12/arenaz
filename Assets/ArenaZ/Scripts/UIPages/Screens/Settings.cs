﻿using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using UnityEngine.Audio;
using System.Collections;
using RedApple;
using RedApple.Utils;
using RedApple.Api.Data;
using System;
using ArenaZ.Screens;

namespace ArenaZ.SettingsManagement
{
    public class Settings : RedAppleSingleton<Settings>
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
        [SerializeField] private Image countryButtonImage;
        [SerializeField] private Image profileImage;

        [Header("Text Fields")]
        [Space(5)]
        [SerializeField] private Text userName;
        [SerializeField] private Text countryButtonText;

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
        [SerializeField] private Button regionButton;
        [SerializeField] private Button languageButton;
        [SerializeField] private Button deleteAccountButton;
        [SerializeField] private Button playerColor;

        [Header("Integer and Floating Point")]
        [Space(5)]
        [SerializeField] private float LogOutPopUpCloseduration;

        private bool IsMusicMute = false;
        private bool IsSFXMute = false;
      //  private FacebookLogin facebookLogin;

        private Sprite countrySprite;

        public Action inputFieldclear;

        //------------------------------------------------------------+
        private void Start()
        {
           // facebookLogin = GetComponent<FacebookLogin>();
            UpdateButtonsOnStart();
            GettingButtonReferences();
            GetCountryDetailsOnStart();
            UIManager.Instance.setUserName += SetUserName;
            UIManager.Instance.showProfilePic += SetProfileImage;
            UIManager.Instance.ScreenShowNormal(Page.LoggedInText.ToString());
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

        private void GetCountryDetailsOnStart()
        {
            RestManager.GetCountryDetails(OnCompletionOfCountryDetailsFetch, OnErrorCountryDetailsFetch);
        }

        private void OnCompletionOfCountryDetailsFetch(CountryData details)
        {
            Debug.Log("The Country Code Is:     " + details.Country_code);
            if (!countryButtonImage.enabled)
            {
                countryButtonImage.enabled = true;
            }
            countryButtonImage.sprite = UIManager.Instance.GetCorrespondingCountrySprite(details.Country_code.ToLower());
            countryButtonText.text = details.Country_code;
        }

        private void OnErrorCountryDetailsFetch(RestUtil.RestCallError obj)
        {
            UIManager.Instance.ShowPopWithText(Page.PopUpTextSettings.ToString(), Constants.noInternet, LogOutPopUpCloseduration);
        }

        public void LogInLogOutButtonNameSet(string buttonName)
        {
            logOutButton.transform.GetChild(0).GetComponent<Text>().text = buttonName;
        }

        private void TasksAfterLogout()
        {
            UIManager.Instance.SetComponent<Text>(Page.LoggedInText.ToString(), false);
            LogInLogOutButtonNameSet(Constants.login);
            UIManager.Instance.showProfilePic?.Invoke((Page.Canines.ToString()));
            UIManager.Instance.setUserName?.Invoke(Constants.defaultUserName);
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

        #region UI_Functionalities
        private void OnClickClose()
        {
            UIManager.Instance.HideScreen(Page.SettingsPanel.ToString());
        }

        private void OnClickLogInLogOut()
        {
            if (PlayerPrefs.GetInt("Logout") == 0)
            {
                Debug.Log("Logged Out");
                UIManager.Instance.ScreenShowNormal(Page.LogOutAlertOverlay.ToString());
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
            PlayerPrefs.SetInt("Logout", 1);
            OnClickClose();
        }

        private void GoToLogIn()
        {
            OnClickClose();
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
