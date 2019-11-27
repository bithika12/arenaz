using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.AccountAccess;
using ArenaZ.Screens;

[RequireComponent(typeof(UIScreen))]
public class PlayerMatch : RedAppleSingleton<PlayerMatch>
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Text userName;

    protected override void Awake()
    {
        UIManager.Instance.setUserName += SetUserName;
        UIManager.Instance.showProfilePic += SetProfileImage;
    }

    private void OnDestroy()
    {
        UIManager.Instance.setUserName -= SetUserName;
        UIManager.Instance.showProfilePic -= SetProfileImage;
    }

    public void SetProfileImage(string imageName)
    {
        profileImage.sprite = UIManager.Instance.GetCorrespondingProfileSprite(imageName, ProfilePic.Medium);
    }

    public void SetUserName(string userName)
    {
        this.userName.text = userName;
    }
}
