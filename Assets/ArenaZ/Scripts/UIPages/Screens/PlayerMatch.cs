using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using System.Collections;
using RedApple;

public class PlayerMatch : Singleton<PlayerMatch>
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Text userName;
    [SerializeField] private AnimationClip animClip;


    private void Start()
    {
        UIManager.Instance.setUserName += SetUserName;
        UIManager.Instance.showProfilePic += SetProfileImage;
    }

    public void SetProfileImage(string imageName)
    {
        Debug.Log("Sprite Name:  " + "    " + imageName + "   " + UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium));
        profileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium);
    }

    public void SetUserName(string userName)
    {
        this.userName.text = userName;
    }

    public void LoadScene()
    {
        StartCoroutine(LoadSceneAfterAnimation());
    }

    private IEnumerator LoadSceneAfterAnimation()
    {
        yield return new WaitForSeconds(animClip.length);
        SceneManagement.Instance.LoadScene("Gameplay");
    }
}
