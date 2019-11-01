using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Mail;
using System;

public class Mail : MonoBehaviour
{
    //Public Variables
    public Button showButton;

    //Private Variables
    private Button myButton;
    private string allLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private string mailID = string.Empty;

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }
    private void OnEnable()
    {
        myButton.onClick.AddListener(ClickToShowAndHideViewButton);
        showButton.onClick.AddListener(ShowMessagePopup);
    }
    private void Start()
    {
        mailID = GettingRandomID();
    }
    private void OnDisable()
    {
        myButton.onClick.RemoveListener(ClickToShowAndHideViewButton);
        showButton.onClick.RemoveListener(ShowMessagePopup);
    }

    private string GettingRandomID()
    {
        string ID = string.Format("{0}{1}{2}",UnityEngine.Random.Range(1000, 9999).ToString(), allLetters[UnityEngine.Random.Range(0, allLetters.Length)],UnityEngine.Random.Range(10, 99).ToString());
        return ID;
    }

    private void ShowMessagePopup()
    {
        MailboxManager.Instance.MailPopup.GetComponent<UIScreenChild>().ShowGameObjWithAnim();  
    }

    private void ClickToShowAndHideViewButton()
    {
        Debug.Log("PreviouslyTouchedMailID " + MailboxManager.Instance.previoslyTouchedGOMailID);
        string previousButtonID = MailboxManager.Instance.previoslyTouchedGOMailID;
        if (!showButton.gameObject.activeSelf)
        {
            showButton.gameObject.SetActive(true);
        }
        Debug.Log("ID" + mailID);
        if (MailboxManager.Instance.previoslyTouchedGOMailID != string.Empty && mailID != previousButtonID)
        {
            MailboxManager.Instance.previouslyTouchedMailShowButton.SetActive(false);
        }
        MailboxManager.Instance.previouslyTouchedMailShowButton = showButton.gameObject;
        MailboxManager.Instance.previoslyTouchedGOMailID = mailID;
    }
}
