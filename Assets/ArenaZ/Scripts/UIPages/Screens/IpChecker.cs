using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedApple.Utils;
using System.Net;
using System;
using Newtonsoft.Json;
using ArenaZ.Manager;

public class IpChecker : MonoBehaviour
{
    private IpCheckerResponse response;

    // Start is called before the first frame update
    void Start()
    {
        CheckIp();
    }
    private void CheckIp()
    {
        Debug.Log("The Console................." + GetLocalIPAddress());
    }

    private void OnIpCheckComplete(IpCheckerResponse _response)
    {

    }
    private void OnErrorIpCheck(RestUtil.RestCallError a_ErrorObj)
    {
        Debug.LogError("Error On VersionCheck: " + a_ErrorObj.Description);
    }
    public void OkayBttnClick()
    {
        Debug.Log("Enter on the Okay Bttn");
        Application.Quit();
    }
    private string GetLocalIPAddress()
    {
        string strHostName = "";
        strHostName = System.Net.Dns.GetHostName();

        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

        IPAddress[] addr = ipEntry.AddressList;

        return addr[addr.Length - 1].ToString();
    }
}

public class IpCheckerResponse
{
    [JsonProperty("status")]
    public bool Status;
}
