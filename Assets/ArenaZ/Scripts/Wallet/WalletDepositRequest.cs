using System.Collections;
using System.Collections.Generic;
using ArenaZ.Manager;
using UnityEngine;
using TMPro;
using RedApple;
using System;
using RedApple.Utils;

namespace ArenaZ.Wallet
{
    public class WalletDepositRequest : MonoBehaviour
    {
        [SerializeField] private WalletHandler walletHandlerRef;
        [SerializeField] private WalletDepositConfirm walletDepositConfirmRef;
        [SerializeField] private TMP_InputField amountField, dollarField, bitcoinField;
        private int depositAmount = 0;

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
                    depositAmount = t_Amount;
                    ConvertedCoinRequest t_Request = new ConvertedCoinRequest() { UserEmail = User.UserEmailId, CoinNumber = t_Amount, TransactionType = "deposit" };
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

        public void RequestDeposit()
        {
            int t_MinimumDeposit = 0;
            int.TryParse(walletHandlerRef.GetWalletDetailsResponse().MinimumDeposit, out t_MinimumDeposit);
            if (depositAmount >= t_MinimumDeposit)
            {
                walletDepositConfirmRef.Initialize(depositAmount);
                RequestDepositRequest t_Request = new RequestDepositRequest() { CoinAmount = depositAmount, UserEmail = User.UserEmailId, UserName = User.UserName, AmountUsd = walletHandlerRef.GetConvertedCoinResponse().DollarAmount };
                RestManager.WalletRequestDeposit(t_Request, onRequest, onError);
            }
        }

        private void onRequest(RequestDepositResponse a_Obj)
        {
            walletHandlerRef.SetRequestDepositResponse(a_Obj);

            UIManager.Instance.HideScreen(Page.WalletDepositRequestPanel.ToString());
            UIManager.Instance.ShowScreen(Page.WalletDepositConfirmPanel.ToString());
            depositAmount = 0;
        }

        public void ClosePanel()
        {
            depositAmount = 0;
            amountField.text = string.Empty;
            dollarField.text = string.Empty;
            bitcoinField.text = string.Empty;
            UIManager.Instance.HideScreen(Page.WalletDepositRequestPanel.ToString());
        }
    }
}