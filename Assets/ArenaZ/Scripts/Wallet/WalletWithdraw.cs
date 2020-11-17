using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ArenaZ.Manager;
using RedApple;
using RedApple.Utils;

namespace ArenaZ.Wallet
{
    public class WalletWithdraw : MonoBehaviour
    {
        [SerializeField] private WalletHandler walletHandlerRef;
        [SerializeField] private TMP_InputField amountField, walletKeyField, dollarField, bitcoinField;
        [SerializeField] private TextMeshProUGUI receivingDollarText;
        [SerializeField] private GameObject confirmWindow, confirmationWindow;

        private int withdrawAmount = 0;

        private void Start()
        {
            amountField.onValueChanged.AddListener(onAmountValueChange);
        }

        private void onAmountValueChange(string a_Value)
        {
            if (!string.IsNullOrEmpty(a_Value) && !string.IsNullOrWhiteSpace(a_Value))
            {
                int t_Amount = GenericExtensions.GetLeadingInt(a_Value);
                if (t_Amount > 0)
                {
                    withdrawAmount = t_Amount;
                    ConvertedCoinRequest t_Request = new ConvertedCoinRequest() { UserEmail = User.UserEmailId, CoinNumber = t_Amount, TransactionType = "withdraw" };
                    RestManager.WalletConvertedCoin(t_Request, onConversion, onError);
                }
            }
        }

        private void onConversion(ConvertedCoinResponse a_Obj)
        {
            walletHandlerRef.SetConvertedCoinResponse(a_Obj);
            dollarField.text = a_Obj.DollarAmount.ToString();
            bitcoinField.text = a_Obj.BtcAmount.ToString();
        }

        private void onError(RestUtil.RestCallError a_Obj)
        {
            Debug.LogError(a_Obj.Error);
        }

        public void RequestWithdraw()
        {
            int t_MinimumWithdrawl = 0;
            int.TryParse(walletHandlerRef.GetWalletDetailsResponse().MinimumWithdrawl, out t_MinimumWithdrawl);
            if (withdrawAmount >= t_MinimumWithdrawl && withdrawAmount <= User.UserCoin && !string.IsNullOrEmpty(walletKeyField.text) && !string.IsNullOrWhiteSpace(walletKeyField.text))
            {
                RequestWithdrawRequest t_Request = new RequestWithdrawRequest() { AmountUsd = walletHandlerRef.GetConvertedCoinResponse().DollarAmount, CoinAmount = withdrawAmount, UserEmail = User.UserEmailId, UserName = User.UserName, WalletKey = walletKeyField.text };
                RestManager.WalletRequestWithdraw(t_Request, onRequest, onError);
            }
        }

        private void onRequest(RequestWithdrawResponse a_Obj)
        {
            walletHandlerRef.SetRequestWithdrawResponse(a_Obj);
            confirmWindow.SetActive(true);
            withdrawAmount = 0;
        }

        public void ConfirmWithdraw()
        {
            ConfirmDepositRequest t_Request = new ConfirmDepositRequest() { UserEmail = User.UserEmailId, TransactionId = walletHandlerRef.GetRequestWithdrawResponse().TransactionDetailsObj.TransactionId };
            RestManager.WalletConfirmDeposit(t_Request, onConfirm, onError);
        }

        private void onConfirm(ConfirmDepositResponse a_Obj)
        {
            walletHandlerRef.OnCompleteAction();
            walletHandlerRef.SetConfirmDepositResponse(a_Obj);

            receivingDollarText.text = string.Format($"You will receive ${walletHandlerRef.GetConvertedCoinResponse().DollarAmount} in bitcoins in your wallet soon.");
            confirmWindow.SetActive(false);
            confirmationWindow.SetActive(true);
        }

        public void CancelWithdraw()
        {
            CancelDepositRequest t_Request = new CancelDepositRequest() { UserEmail = User.UserEmailId, TransactionId = walletHandlerRef.GetRequestWithdrawResponse().TransactionDetailsObj.TransactionId };
            RestManager.WalletCancelDeposit(t_Request, onCancel, onError);

            CloseWindow();
        }

        private void onCancel(CancelDepositResponse a_Obj)
        {
            walletHandlerRef.SetCancelDepositResponse(a_Obj);
        }

        public void CloseWindow()
        {
            withdrawAmount = 0;
            amountField.text = string.Empty;
            dollarField.text = string.Empty;
            bitcoinField.text = string.Empty;
            walletKeyField.text = string.Empty;
            receivingDollarText.text = string.Empty;

            confirmWindow.SetActive(false);
            confirmationWindow.SetActive(false);
            UIManager.Instance.HideScreen(Page.WalletWithdrawPanel.ToString());
        }
    }
}