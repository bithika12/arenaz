using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using RedApple;
using RedApple.Utils;
using System;
using System.Linq;

public class WalletHistory : MonoBehaviour
{
    [SerializeField] private GameObject cellPrototype;
    [SerializeField] private Transform cellContainer;

    private List<WalletHistoryCell> activeHistoryCells = new List<WalletHistoryCell>();

    public void Initialize()
    {
        clearCells();
        RestManager.GetWalletHistory(User.UserEmailId, onReceiveData, (error) =>
        {
            Debug.LogError(error.Error);
        });
    }

    private void onReceiveData(WalletHistoryList a_WalletHistoryList)
    {
        GameObject t_Go;
        WalletHistoryCell t_Cell;
        for (int i = 0; i < a_WalletHistoryList.WalletHistoryDatas.Count; i++)
        {
            t_Go = Instantiate(cellPrototype, cellContainer);
            t_Go.SetActive(true);
            t_Cell = t_Go.GetComponent<WalletHistoryCell>();
            t_Cell.Initialize(a_WalletHistoryList.WalletHistoryDatas[i]);
            activeHistoryCells.Add(t_Cell);
        }
    }

    private void clearCells()
    {
        if (activeHistoryCells.Any())
        {
            for (int i = activeHistoryCells.Count; i-- > 0;)
            {
                Destroy(activeHistoryCells[i].gameObject);
            }
            activeHistoryCells.Clear();
        }
    }
}

public class WalletHistoryList
{
    [JsonProperty("totalTransactionList")]
    public List<WalletHistoryData> WalletHistoryDatas = new List<WalletHistoryData>();
}

public class WalletHistoryData
{
    [JsonProperty("datetime")]
    public string DateTime;
    [JsonProperty("type")]
    public string Type;
    [JsonProperty("amount")]
    public string Amount;
    [JsonProperty("code")]
    public string Code;
    [JsonProperty("status")]
    public string Status;
}
