using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchToShowAndHideButton : MonoBehaviour
{
    public GameObject showButton;
    private Button myButton;
    string allLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string mailID = string.Empty;

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }
    private void OnEnable()
    {
        myButton.onClick.AddListener(ClickToShowAndHide);
    }
    private void Start()
    {
        mailID = GettingRandomID();
        Debug.Log("ID" + mailID);
    }
    private void OnDisable()
    {
        myButton.onClick.RemoveListener(ClickToShowAndHide);
    }

    private string GettingRandomID()
    {
        string ID =string.Format("{0}{1}{2}", Random.Range(1000, 9999).ToString(), allLetters[Random.Range(0,allLetters.Length)], Random.Range(10, 99).ToString());
        return ID;
    }

    void ClickToShowAndHide()
    {
        Debug.Log("ClickToShowAndHide");
        string previousButtonID = MailboxManager.Instance.previoslyTouchedMailID;
        if (!showButton.activeSelf)
        {
            showButton.SetActive(true);
        }
        Debug.Log("ID" + showButton.GetInstanceID() + "   " + MailboxManager.Instance.previoslyTouchedMailID);
        if (MailboxManager.Instance.previoslyTouchedMailID != string.Empty && mailID != previousButtonID)
        {
            MailboxManager.Instance.previouslyTouchedMailShowButton.SetActive(false);
        }
        MailboxManager.Instance.previouslyTouchedMailShowButton = showButton;
        MailboxManager.Instance.previoslyTouchedMailID = mailID;
    }
}
