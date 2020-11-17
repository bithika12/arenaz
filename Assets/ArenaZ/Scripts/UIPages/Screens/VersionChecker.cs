using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedApple;
using RedApple.Utils;
using Newtonsoft.Json;
using ArenaZ.Manager;

public class VersionChecker : MonoBehaviour
{
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
        if (a_Response != null && !a_Response.Status)
            UIManager.Instance.ShowScreen(Page.VersionCheckPanel.ToString());
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
}

