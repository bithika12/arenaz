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

    [Header("Names")][Space(5)]
    [SerializeField] private Text userName;
    [SerializeField] private Text enemyName;

    [Header("Cups")][Space(5)]
    [SerializeField] private Text userCup;
    [SerializeField] private Text opponentCup;

    [Header("Others")][Space(5)]
    [SerializeField] private AnimationClip animClip;

    private void Start()
    {
        UIManager.Instance.setUserName += SetUserName;
        UIManager.Instance.showProfilePic += SetUserProfileImage;
        ShootingRange.Instance.setOpponentImage += SetOpponentProfileImage;

        ShootingRange.Instance.setCupCount += setCupCount;
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
            //userPic.material.SetTexture("_MainTex", t_CharacterPicData.ProfilePic.texture);
            userPic.sprite = t_CharacterPicData.ProfilePic;
        }
    }

    private void setCupCount(string selfCupCount, string opponentCupCount)
    {
        userCup.text = selfCupCount;
        opponentCup.text = opponentCupCount;
    }

    public void SetUserName(string userName)
    {
        this.userName.text = userName;
    }

    public void SetOpponentProfileImage(string name, string race, string color)
    {
        enemyName.text = name;
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
            //opponentPic.material.SetTexture("_MainTex", t_CharacterPicData.ProfilePic.texture);
            opponentPic.sprite = t_CharacterPicData.ProfilePic;
        }
    }

    public void LoadGameplay()
    {
        StartCoroutine(LoadGameplayAfterAnim());
    }

    private IEnumerator LoadGameplayAfterAnim()
    {
        GameManager.Instance.CameraControllerRef.SetFocus(true);
        float value = animClip.length + 1f;
        Debug.Log($"-----------------------------LoadGameplayAfterAnimLength: {value}-----------------------------");
        yield return new WaitForSeconds(value);
        Debug.Log("-----------------------------LoadGameplayAfterAnim-----------------------------");
        //UIManager.Instance.ShowUiPanel(false);
        UIManager.Instance.HideScreen(Page.UIPanel.ToString());
        UIManager.Instance.ShowScreen(Page.GameplayPanel.ToString(),Hide.none);
        UIManager.Instance.ShowScreen(Page.GameplayUIPanel.ToString(), Hide.none);
        GameManager.Instance.InitializeOnGameStartSequences();
    }
}
