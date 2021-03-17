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
    [SerializeField] private TMP_InputField code;
    [SerializeField] private TextMeshProUGUI status;

    [SerializeField] private List<Button> copyButtons = new List<Button>();

    private WalletHistoryData walletHistoryDataObj;

    public void Initialize(WalletHistoryData a_WalletHistoryData)
    {
        if (a_WalletHistoryData == null)
            return;

        walletHistoryDataObj = a_WalletHistoryData;

        setDateTime(a_WalletHistoryData.DateTime);
        type.text = a_WalletHistoryData.Type;
        amount.text = a_WalletHistoryData.Amount;
        setCopyStatus(string.Equals(a_WalletHistoryData.Status.ToUpper(), "NEW"));
        code.text = a_WalletHistoryData.Code;
        status.text = a_WalletHistoryData.Status;
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
                t_Str.Append(string.Format($"{t_StrArr[0]}\n"));
                t_Str.Append(t_StrArr[1]);
                t_Str.Append(t_StrArr[2].ToUpper());
            }
        }
    }

    public void CopyCode() => UniClipboard.SetText(walletHistoryDataObj?.Code);
}
