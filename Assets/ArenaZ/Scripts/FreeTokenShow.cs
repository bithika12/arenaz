using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArenaZ.Manager;
using RedApple;
using DevCommons.Utility;
using ArenaZ.Screens;
using ArenaZ.SettingsManagement;
using System.Collections.Generic;
using RedApple.Utils;
using Newtonsoft.Json;
using ArenaZ.GameMode;

public class FreeTokenShow : MonoBehaviour
{
    [SerializeField] private GameObject shootingRangePanel;
    [SerializeField] private FreeTokenPanel freeTokenPanel;
    [SerializeField] private ShootingRange shootingRange;

    private void OnEnable()
    {
        Debug.Log("The User Status........................"+ User.ShowFreeTokenPanel);
        if (User.ShowFreeTokenPanel == 1)
        {
            Debug.Log("Email Id........................................." + User.UserEmailId);
            RestManager.FreeCoinStatus(User.UserEmailId, FreeCoinVerificationOnComplete, OnFreeCoinVerificationError);
            RestManager.RequestCount(User.UserEmailId, FreeRequestCountOnComplete, OnFreeRequestCountError);
        }
    }
    private void FreeRequestCountOnComplete(RequestCountVerificationResponse a_response)
    {
        Debug.Log($"Free Request Count: {JsonConvert.SerializeObject(a_response)}");
        shootingRange.DisableAllGlow(shootingRange.total_ten);
        shootingRange.DisableAllGlow(shootingRange.total_fifty);
        shootingRange.DisableAllGlow(shootingRange.total_hundred);
        shootingRange.DisableAllGlow(shootingRange.total_two_fifty);
        shootingRange.DisableAllGlow(shootingRange.total_five_hundred);
        shootingRange.EnableDisableCoin(a_response.total_ten, shootingRange.total_ten);
        shootingRange.EnableDisableCoin(a_response.total_fifty, shootingRange.total_fifty);
        shootingRange.EnableDisableCoin(a_response.total_hundred, shootingRange.total_hundred);
        shootingRange.EnableDisableCoin(a_response.total_two_fifty, shootingRange.total_two_fifty);
        shootingRange.EnableDisableCoin(a_response.total_five_hundred, shootingRange.total_five_hundred);
    }
    private void OnFreeRequestCountError(RestUtil.RestCallError a_ErrorObj)
    {
        Debug.LogError("Error On Free Coin Verification Error: " + a_ErrorObj.Description);
    }
    private void FreeCoinVerificationOnComplete(FreeCoinVerificationResponse a_Response)
    {
        Debug.Log($"Free Coin Verification: {JsonConvert.SerializeObject(a_Response)}");
        if (a_Response.countStatus == 0)
        {
            UIManager.Instance.ShowScreen(Page.FreeTokenPanel.ToString());
            User.UserGameCount = a_Response.userGameCount;
            User.GameCountType = a_Response.free_coin_incentive;
            freeTokenPanel.EnableFreeTokenPanel();
            Debug.Log("The User Game Count................." + User.UserGameCount);
        }
    }
    private void OnFreeCoinVerificationError(RestUtil.RestCallError a_ErrorObj)
    {
        Debug.LogError("Error On Free Coin Verification Error: " + a_ErrorObj.Description);
    }
}
public class FreeCoinVerificationResponse
{
    [JsonProperty("countStatus")]
    public int countStatus;
    [JsonProperty("userGameCount")]
    public int userGameCount;
    [JsonProperty("free_coin_incentive")]
    public int free_coin_incentive;
}
public class RequestCountVerificationResponse
{
    [JsonProperty("total_ten")]
    public int total_ten;
    [JsonProperty("total_fifty")]
    public int total_fifty;
    [JsonProperty("total_hundred")]
    public int total_hundred;
    [JsonProperty("total_two_fifty")]
    public int total_two_fifty;
    [JsonProperty("total_five_hundred")]
    public int total_five_hundred;
}
