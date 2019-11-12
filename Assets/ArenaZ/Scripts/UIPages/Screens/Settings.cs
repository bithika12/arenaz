using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;
using UnityEngine.Audio;
using System.Collections;

[RequireComponent(typeof(UIScreen))]
public class Settings : MonoBehaviour
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

    [Header("Floats")][Space(5)]
    [SerializeField] private float MuteValue = -80.0f;
    [SerializeField] private float UnmuteValue = 0.0f;
    private float animClipLenght;

    [Header("Toggles And Buttons")][Space(5)]
    [SerializeField] private Toggle MusicToggle;
    [SerializeField] private Toggle SFXToggle;
    [SerializeField] private Button closeButton;

    [Header("Booleans")][Space(5)]
    [SerializeField] private bool IsMusicMute = false;
    [SerializeField] private bool IsSFXMute = false;  

    //------------------------------------------------------------+
    private void Start()
    {
        UpdateButtonsOnStart();
    }
    private void OnEnable()
    {
        GettingButtonReferences();
    }
    private void OnDisable()
    {
        ReleaseButtonReferences();
    }

    #region Button_References
    private void GettingButtonReferences()
    {
        closeButton.onClick.AddListener(OnClickClose);

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

    private void OnClickClose()
    {
        UIManager.Instance.HideScreen(Page.Settings.ToString());
    }

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

}
