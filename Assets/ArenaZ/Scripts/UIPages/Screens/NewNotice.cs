using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ArenaZ.Manager;

public class NewNotice : MonoBehaviour
{
    [SerializeField] private Text welcomeText;

    public void SetMasterMessageText(string text)
    {
        welcomeText.text = text;
    }

    public void CloseScreen()
    {
        UIManager.Instance.HideScreen(Page.NewNoticePanel.ToString());
    }
}

public class MasterData
{
    [JsonProperty("master_message")]
    public string MasterMessage;
    [JsonProperty("support_email")]
    public string SupportEmail;
}
