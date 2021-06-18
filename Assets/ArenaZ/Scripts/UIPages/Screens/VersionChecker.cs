using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedApple;
using RedApple.Utils;
using Newtonsoft.Json;
using ArenaZ.Manager;
using System.Net;
using System;

public class VersionChecker : MonoBehaviour
{
    public GameObject gameDeactivationPopup;
    public GameObject versionCheck;
    public GameObject bannedStatusPopup;
    private VersionCheckerResponse response;

    private void Start()
    {
        checkVersion();
    }

    private void checkVersion()
    {
        Debug.Log($"App Version: {Application.version}");
        RestManager.AppVersionCheck(Application.version, OnVersionCheckComplete, OnErrorVersionCheck);
    }

    private void OnVersionCheckComplete(VersionCheckerResponse a_Response)
    {
        response = a_Response;
        if (a_Response != null)
        {
            Debug.Log("Enter into the Null Response............");
            DownloadUrl.url = a_Response.DownloadLink;
            if (!a_Response.Status)
            {
                UIManager.Instance.ShowScreen(Page.VersionCheckPanel.ToString());
                versionCheck.SetActive(true);
            }
            else if(a_Response.BannedStatus == 1)
            {
                Debug.Log("Enter into the Banned Status............");
                UIManager.Instance.ShowScreen(Page.VersionCheckPanel.ToString());
                bannedStatusPopup.SetActive(true);
            }
            else if(a_Response.GameDeactivation.Equals("Yes"))
            {
                Debug.Log("Enter into the Game Deactivation............");
                UIManager.Instance.ShowScreen(Page.VersionCheckPanel.ToString());
                gameDeactivationPopup.SetActive(true);
            }
        }
    }
    public void OkayBttnClick()
    {
        Debug.Log("Enter on the Okay Bttn");
        Application.Quit();
    }
    private void OnErrorVersionCheck(RestUtil.RestCallError a_ErrorObj)
    {
        Debug.LogError("Error On VersionCheck: " + a_ErrorObj.Description);
    }

    public void GoToDownloadURL()
    {
        if (response != null)
        {
            Debug.Log($"Download Link: {response.DownloadLink}");
            Application.OpenURL(response.DownloadLink);
        }
    }
}

public class VersionCheckerResponse
{
    [JsonProperty("status")]
    public bool Status;
    [JsonProperty("download_link")]
    public string DownloadLink;
    [JsonProperty("bannedStatus")]
    public int BannedStatus;
    [JsonProperty("game_deactivation")]
    public string GameDeactivation;
}

