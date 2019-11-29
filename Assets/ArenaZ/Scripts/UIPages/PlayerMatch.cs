using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.AccountAccess;
using ArenaZ.Screens;

public class PlayerMatch : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Text userName;


    private void Start()
    {
        UIManager.Instance.setUserName += SetUserName;
        UIManager.Instance.showProfilePic += SetProfileImage;
    }

    public void SetProfileImage(string imageName)
    {
        Debug.Log("Sprite Name:  " +"    "+imageName+"   "+ UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium));
        profileImage.sprite = UIManager.Instance.GetProfile(imageName, ProfilePicType.Medium);
    }

    public void SetUserName(string userName)
    {
        this.userName.text = userName;
    }
}
