using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class WalletHistoryCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dateTime;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private TextMeshProUGUI amount;
    [SerializeField] private GameObject copyIconGO;
    [SerializeField] private TextMeshProUGUI code;
    [SerializeField] private TextMeshProUGUI status;

    [SerializeField] private Color color1, color2;

    [SerializeField] private List<Button> copyButtons = new List<Button>();

    private WalletHistoryData walletHistoryDataObj;

    public void Initialize(WalletHistoryData a_WalletHistoryData)
    {
        if (a_WalletHistoryData == null)
            return;

        walletHistoryDataObj = a_WalletHistoryData;

        setDateTime(a_WalletHistoryData.DateTime);
        setType(a_WalletHistoryData.Type);
        amount.text = a_WalletHistoryData.Amount;
        setCopyStatus(string.Equals(a_WalletHistoryData.Status.ToUpper(), "NEW"));
        code.text = a_WalletHistoryData.Code;
        setStatus(a_WalletHistoryData.Status);
    }

    private void setType(string a_Data)
    {
        type.text = a_Data;
        type.color = string.Equals(a_Data.ToUpper(), "WITHDRAW") ? color2 : color1;
    }

    private void setStatus(string a_Data)
    {
        status.text = a_Data;
        status.color = string.Equals(a_Data.ToUpper(), "COMPLETED") ? color2 : color1;
    }

    private void setCopyStatus(bool a_Status)
    {
        copyIconGO.SetActive(a_Status);
        copyButtons.ForEach(x => x.interactable = a_Status);
    }

    private void setDateTime(string a_Data)
    {
        if (!string.IsNullOrEmpty(a_Data))
        {
            string[] t_StrArr = a_Data.Split(new char[0]);
            if (t_StrArr.Length == 3)
            {
                StringBuilder t_Str = new StringBuilder();
                t_Str.Append(string.Format($"{t_StrArr[0]}/\n"));
                t_Str.Append(t_StrArr[1]);
                t_Str.Append(t_StrArr[2].ToUpper());
                dateTime.text = t_Str.ToString();
            }
        }
    }

    public void CopyCode() => UniClipboard.SetText(walletHistoryDataObj?.Code);
}
