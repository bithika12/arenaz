using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using System.Collections;
using RedApple;
using ArenaZ.GameMode;
using System.Collections.Generic;
using ArenaZ;
using DevCommons.Utility;

public class PlayerMatch : Singleton<PlayerMatch>
{
    [Header("User Image")]
    [SerializeField] private Image userFrame;
    [SerializeField] private Image userPic;

    [Header("Opponent Image")]
    [SerializeField] private Image opponentFrame;
    [SerializeField] private Image opponentPic;

    [Header("Text")][Space(5)]
    [SerializeField] private Text userName;
    [SerializeField] private Text enemyName;

    [Header("Text")][Space(5)]
    [SerializeField] private AnimationClip animClip;

    private void Start()
    {
        UIManager.Instance.setUserName += SetUserName;
        UIManager.Instance.showProfilePic += SetUserProfileImage;
        ShootingRange.Instance.setOpponentName += SetOpponentName;
        ShootingRange.Instance.setOpponentImage += SetOpponentProfileImage;
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

    public void SetUserName(string userName)
    {
        this.userName.text = userName;
    }

    public void SetOpponentProfileImage(string race, string color)
    {
        ERace t_Race = EnumExtensions.EnumFromString<ERace>(typeof(ERace), race);
        EColor t_Color = EnumExtensions.EnumFromString<EColor>(typeof(EColor), color);

        SquareFrameData t_FrameData = DataHandler.Instance.GetSquareFrameData(t_Color);
        if (t_FrameData != null)
        {
            opponentFrame.sprite = t_FrameData.FramePic;
        }

        CharacterPicData t_CharacterPicData = DataHandler.Instance.GetCharacterPicData(t_Race, t_Color);
        if (t_CharacterPicData != null)
        {
            opponentPic.sprite = t_CharacterPicData.ProfilePic;
        }
    }

    public void SetOpponentName(string OpponentName)
    {
        this.enemyName.text = OpponentName;
    }

    public void LoadGameplay()
    {
        StartCoroutine(LoadGameplayAfterAnim());
    }

    private IEnumerator LoadGameplayAfterAnim()
    {
        float value = animClip.length + 1f;
        Debug.Log($"-----------------------------LoadGameplayAfterAnimLength: {value}-----------------------------");
        yield return new WaitForSeconds(value);
        Debug.Log("-----------------------------LoadGameplayAfterAnim-----------------------------");
        UIManager.Instance.HideScreen(Page.UIPanel.ToString());
        UIManager.Instance.ShowScreen(Page.GameplayPanel.ToString(),Hide.none);
    }
}
