using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedApple;
using RedApple.Utils;
using Newtonsoft.Json;
using ArenaZ.Manager;
using TMPro;
using ArenaZ.Screens;
using ArenaZ.GameMode;

public class AutoRefillCoins : MonoBehaviour
{
    public GameObject autoRefillCoinPanel;
    public TextMeshProUGUI autoRefillText;
    [SerializeField] private ShootingRange shootingRange;

    public void ClickOnOkayBttn()
    {
        //User.UserCoin += User.AutoRefill;
        RestManager.AddUserCoins(User.UserName, User.AutoRefill.ToString(), OnAddUserCoin, OnAddUserCoinError);
    }

    public void OnAddUserCoin()
    {
        shootingRange.Refresh();
        autoRefillCoinPanel.SetActive(false);
    }
    public void OnAddUserCoinError(RestUtil.RestCallError a_ErrorObj)
    {
        Debug.LogError("Error On resend email: " + a_ErrorObj.Description);
    }
}