using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;
using UnityEngine.Audio;
using ArenaZ.AccountAccess;
using TMPro;
using System.Collections;

[RequireComponent(typeof(UIScreen))]
public class Settings : RedAppleSingleton<Settings>
{
    //Private Properties
   

    [Header("Audio Mixers")][Space(5)]
    [SerializeField]private AudioMixer MusicAudioMixer;
    [SerializeField]private AudioMixer SFXAudioMixer;

    [Header("Sprites And Images")][Space(5)]
    [SerializeField] private Sprite UnmuteMusicSprite;
    [SerializeField] private Sprite MuteMusicSprite;
    [SerializeField] private Sprite UnmuteSFXSprite;
    [SerializeField] private Sprite MuteSFXSprite;
    [SerializeField] private Image MusicImage;
    [SerializeField] private Image SFXImage;
    [SerializeField] private Image countryButtonImage;
    [SerializeField] private Image profileImage;

    [Header("Text Fields")][Space(5)]
    [SerializeField] private Text userName;

    [Header("Data Types")][Space(5)]
    [SerializeField] private float MuteValue = -80.0f;
    [SerializeField] private float UnmuteValue = 0.0f;

    [Header("Toggles And Buttons")][Space(5)]
    [SerializeField] private Toggle MusicToggle;
    [SerializeField] private Toggle SFXToggle;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button supportButton;
    [SerializeField] private Button logOutButton;
    [SerializeField] private Button regionButton;
    [SerializeField] private Button languageButton;
    [SerializeField] private Button deleteAccountButton;
    [SerializeField] private Button playerColor;

    [Header("Text")][Space(5)]
    [SerializeField] private Text countryButtonText;

    [Header("Integer")][Space(5)]
    [SerializeField] private int PopUpduration;

    private bool IsMusicMute = false;
    private bool IsSFXMute = false;

    //------------------------------------------------------------+
    protected override void Awake()
    {
        UIManager.Instance.setUserName += SetUserName;
        UIManager.Instance.showProfilePic += SetProfileImage;
    }

    private void Start()
    {
        UpdateButtonsOnStart();
        GettingButtonReferences();
        //PlayerPrefs.SetInt("Logout", 1);
    }

    private void OnDestroy()
    {
        ReleaseButtonReferences();
        UIManager.Instance.setUserName -= SetUserName;
        UIManager.Instance.showProfilePic -= SetProfileImage;
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

    public void LogInLogOutButtonNameSet()
    {
        logOutButton.transform.GetChild(0).GetComponent<Text>().text = AccountAccessManager.Instance.LogInAndLogOutButtonName;
    }

    private void LogoutUserProfile()
    {
        AccountAccessManager.Instance.OnClickLogout();
    }

    public void SetCountrySpriteAndCountryNameOnButton()
    {
        if (!countryButtonImage.enabled)
        {
            countryButtonImage.enabled = true;
        }
        countryButtonImage.sprite = AccountAccessManager.Instance.CountrySprite;
        countryButtonText.text = AccountAccessManager.Instance.CountryId;
    }

    public void SetProfileImage(string imageName)
    { 
        profileImage.sprite = UIManager.Instance.GetCorrespondingProfileSprite(imageName, ProfilePic.Small);
    }

    public void SetUserName(string userName)
    {
       this.userName.text = userName;
    }

    #region UI_Functionalities
    public void OnClickClose()
    {
        UIManager.Instance.HideScreen(Page.Settings.ToString());
    }

    public void OnClickLogInLogOut()
    {
        if (PlayerPrefs.GetInt("Logout") == 0)
        {
            Debug.Log("Logged Out");
            StartCoroutine(LogOut());
        }
        else
        {
            Debug.Log("Not Logged Out");
            GoToLogIn();
        }      
    }

    IEnumerator LogOut()
    {
        LogoutUserProfile();
        yield return new WaitForSeconds(PopUpduration);
        UIManager.Instance.ScreenShowAndHide(Page.AccountAccessDetails.ToString(), Hide.none);
        UIManager.Instance.HideScreen(Page.CharacterSelection.ToString());
        PlayerPrefs.SetInt("Logout", 1);
        OnClickClose();
    }

    private void GoToLogIn()
    {
        OnClickClose();
        UIManager.Instance.ScreenShowAndHide(Page.AccountAccessDetails.ToString(), Hide.none);
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
