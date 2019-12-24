using UnityEngine;
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
        UIManager.Instance.showProfilePic += SetEnemyProfileImage;
    }

    public void SetUserProfileImage(string imageName)
    {
        userProfileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium);
    }

    public void SetUserName(string userName)
    {
        this.userName.text = userName;
    }

    public void SetEnemyProfileImage(string imageName)
    {
        enemyProfileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium);
    }

    public void SetEnemyName(string enemyName)
    {
        this.enemyName.text = enemyName;
    }

    public void LoadScene()
    {
        StartCoroutine(LoadSceneAfterAnimation());
    }

    private IEnumerator LoadSceneAfterAnimation()
    {
        yield return new WaitForSeconds(animClip.length);
        SceneManagement.Instance.LoadScene(SceneType.Gameplay.ToString());
    }
}
