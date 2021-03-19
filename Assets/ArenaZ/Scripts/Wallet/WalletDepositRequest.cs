using System.Collections;
using System.Collections.Generic;
using ArenaZ.Manager;
using UnityEngine;
using TMPro;
using RedApple;
using System;
using RedApple.Utils;
using DG.Tweening;
using UnityEngine.UI;
using System.Globalization;
using ArenaZ.Screens;

namespace ArenaZ.Wallet
{
    public class WalletDepositRequest : MonoBehaviour, IWalletEvent
    {
        [SerializeField] private WalletHandler walletHandlerRef;
        [SerializeField] private WalletDepositConfirm walletDepositConfirmRef;
        [SerializeField] private TMP_InputField amountField, dollarField, bitcoinField;
        [SerializeField] private TextMeshProUGUI minimumAmountHintText, transactionFeeText, warningText;
        [SerializeField] private GameObject warningWindow;
        [SerializeField] private Button requestDepositButton;
        [SerializeField] private Text requestDepositButtonText;

        private bool requestInProgress = false;
        private int depositAmount = 0;
        private int minimumDepositAmount = 0;

        private void Start()
        {
            enableRequestButton(false);
            walletHandlerRef.SubscribeToEvent(this, true);
            amountField.onValueChanged.AddListener(onAmountValueChange);
        }

        private void onAmountValueChange(string a_Value)
        {
            if (requestInProgress)
                return;

            if (!string.IsNullOrEmpty(a_Value) && !string.IsNullOrWhiteSpace(a_Value))
            {
                depositAmount = GenericExtensions.GetLeadingInt(a_Value);
                if (depositAmount >= minimumDepositAmount)
                {
                    enableRequestButton(true);
                    ConvertedCoinRequest t_Request = new ConvertedCoinRequest() { UserEmail = User.UserEmailId, CoinNumber = depositAmount, TransactionType = "deposit" };
                    RestManager.WalletConvertedCoin(t_Request, onConversion, onError);
                }
                else
                {
                    enableRequestButton(false);
                    dollarField.text = "INVALID";
                    bitcoinField.text = "INVALID";
                }
            }
            else
            {
                enableRequestButton(false);
                depositAmount = 0;
                dollarField.text = string.Empty;
                bitcoinField.text = string.Empty;
            }
        }

        private void enableRequestButton(bool a_State)
        {
            requestDepositButton.interactable = a_State;
            requestDepositButtonText.color = a_State ? Color.white : Color.gray;
        }

        private void onConversion(ConvertedCoinResponse a_Obj)
        {
            walletHandlerRef.SetConvertedCoinResponse(a_Obj);
            dollarField.text = a_Obj.DollarAmount.ToString("N", new CultureInfo("en-US"));
            bitcoinField.text = a_Obj.BtcAmount.ToString();
        }

        private void onError(RestUtil.RestCallError a_Obj)
        {
            Debug.LogError(a_Obj.Error);
        }

        public void RequestDeposit()
        {
            if (requestInProgress)
                return;

            if (depositAmount >= minimumDepositAmount)
            {
                requestInProgress = true;
                RequestDepositRequest t_Request = new RequestDepositRequest() { CoinAmount = depositAmount, UserEmail = User.UserEmailId, UserName = User.UserName, AmountUsd = walletHandlerRef.GetConvertedCoinResponse().DollarAmount };
                RestManager.WalletRequestDeposit(t_Request, onRequest, (error) =>
                {
                    onError(error);
                    requestInProgress = false;
                });
            }
            else if (depositAmount == 0)
            {
                warningWindow.SetActive(true);
            }
            else
            {
                // Do nothing....
            }
        }

        private void onRequest(RequestDepositResponse a_Obj)
        {
            walletHandlerRef.SetRequestDepositResponse(a_Obj);
            walletDepositConfirmRef.Initialize(depositAmount);

            UIManager.Instance.HideScreen(Page.WalletDepositRequestPanel.ToString());
            UIManager.Instance.ShowScreen(Page.WalletDepositConfirmPanel.ToString());
            resetAttributes();

            CharacterSelection.Instance.GetUnreadMail();
        }

        public void ClosePanel()
        {
            UIManager.Instance.HideScreen(Page.WalletDepositRequestPanel.ToString());
            resetAttributes();
        }

        private void resetAttributes()
        {
            enableRequestButton(false);
            requestInProgress = false;
            depositAmount = 0;
            amountField.text = string.Empty;
            dollarField.text = string.Empty;
            bitcoinField.text = string.Empty;
        }

        private void OnDestroy()
        {
            walletHandlerRef.SubscribeToEvent(this, false);
        }

        public void OnReceiveWalletDetails(WalletDetailsResponse a_WalletDetailsResponse)
        {
            minimumAmountHintText.text = string.Format($"Minimum amount of {a_WalletDetailsResponse.MinimumDeposit} is required for deposit.");
            warningText.text = string.Format($"Please enter at least {a_WalletDetailsResponse.MinimumDeposit} to make a deposit. If you accidentally deposit a different amount, our support team will adjust it and the correct amount will reflect your account.");

            if (float.TryParse(a_WalletDetailsResponse.TransactionFeeDeposit, out float t_TransactionFeeDeposit))
                transactionFeeText.text = t_TransactionFeeDeposit > 0.0f ? string.Format($"The amount above reflects a {a_WalletDetailsResponse.TransactionFeeDeposit}% transaction fee.") : "";
            else
                transactionFeeText.text = "";

            minimumDepositAmount = 0;
            int.TryParse(a_WalletDetailsResponse.MinimumDeposit, out minimumDepositAmount);
        }

        public void OnCompleteWalletAction()
        {
            
        }
    }
}