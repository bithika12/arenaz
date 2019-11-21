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

    public void SetProfileImage(string imageName)
    {
        profileImage.sprite = UIManager.Instance.GetCorrespondingProfileSprite(imageName, ProfilePic.Medium);
    }

    public void SetUserName()
    {
        userName.text = AccountAccessManager.Instance.UserName;
    }
}
