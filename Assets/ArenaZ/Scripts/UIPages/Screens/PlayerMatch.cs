﻿using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using System.Collections;
using RedApple;
using RedApple.Api.Data;
using System;

public class PlayerMatch : Singleton<PlayerMatch>
{
    [Header("Images")][Space(5)]
    [SerializeField] private Image userProfileImage;
    [SerializeField] private Image enemyProfileImage;

    [Header("Text")][Space(5)]
    [SerializeField] private Text userName;
    [SerializeField] private Text enemyName;

    [Header("Text")][Space(5)]
    [SerializeField] private AnimationClip animClip;


    private void Start()
    {
        UIManager.Instance.setUserName += SetUserName;
        UIManager.Instance.showProfilePic += SetUserProfileImage;
    }

    public void SetUserProfileImage(string imageName)
    {
        userProfileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium);
    }

    public void SetUserName(string userName)
    {
        this.userName.text = userName;
    }

    public void SetOpponentProfileImage(string imageName)
    {
        enemyProfileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium);
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
        yield return new WaitForSeconds(animClip.length+1f);
        UIManager.Instance.HideScreenImmediately(Page.UIPanel.ToString());
        UIManager.Instance.ScreenShowNormal(Page.GameplayPanel.ToString());
    }
}