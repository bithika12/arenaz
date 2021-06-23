using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ArenaZ.Manager;
using RedApple;
using RedApple.Utils;
using DG.Tweening;
using UnityEngine.UI;
using System.Globalization;
using ArenaZ.Screens;

namespace ArenaZ.Wallet
{
    public class WalletWithdraw : MonoBehaviour, IWalletEvent
    {
        [SerializeField] private WalletHandler walletHandlerRef;
        [SerializeField] private TMP_InputField amountField, walletKeyField, dollarField, bitcoinField;
        [SerializeField] private TextMeshProUGUI receivingDollarText, minimumAmountHintText, transactionFeeText, walletKeyWarningText, notEnoughCoinsWarningText, marketVolatilityText;
        [SerializeField] private GameObject notEnoughCoinsWarningWindow, confirmWindow, confirmationWindow;
        [SerializeField] private int walletKeyMinimumLength = 10;
        [SerializeField] private Button requestWithdrawButton;
        [SerializeField] private Text requestWithdrawButtonText;

        private bool requestInProgress = false;
        private bool allowMiniAccountWithdrawal = false;
        private int withdrawAmount = 0;
        private int minimumWithdrawlAmount = 0;

        [SerializeField] private TextMeshProUGUI countdownTimerText;
        [SerializeField] private CountdownTimer countdownTimer;

        private Sequence warningSequence;

        private void Start()
        {
            enableRequestButton(false);
            walletHandlerRef.SubscribeToEvent(this, true);
            amountField.onValueChanged.AddListener(onAmountValueChange);
            countdownTimer.StartCountdown(86400, false, (t) => countdownTimerText.text = t, () =>
            {
                //countdownTimerText.DOKill();
                countdownTimerText.color = Color.yellow;
            },true);
        }

        private void onAmountValueChange(string a_Value)
        {
            if (requestInProgress)
                return;

            if (!string.IsNullOrEmpty(a_Value) && !string.IsNullOrWhiteSpace(a_Value))
            {
                withdrawAmount = GenericExtensions.GetLeadingInt(a_Value);
                if (allowMiniAccountWithdrawal)
                {
                    if (withdrawAmount > 0)
                    {
                        enableRequestButton(true);
                        ConvertedCoinRequest t_Request = new ConvertedCoinRequest() { UserEmail = User.UserEmailId, CoinNumber = withdrawAmount, TransactionType = "withdraw" };
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
                    if (withdrawAmount >= minimumWithdrawlAmount)
                    {
                        enableRequestButton(true);
                        ConvertedCoinRequest t_Request = new ConvertedCoinRequest() { UserEmail = User.UserEmailId, CoinNumber = withdrawAmount, TransactionType = "withdraw" };
                        RestManager.WalletConvertedCoin(t_Request, onConversion, onError);
                    }
                    else
                    {
                        enableRequestButton(false);
                        dollarField.text = "INVALID";
                        bitcoinField.text = "INVALID";
                    }
                }
            }
            else
            {
                enableRequestButton(false);
                withdrawAmount = 0;
                dollarField.text = string.Empty;
                bitcoinField.text = string.Empty;
            }
        }

        private void enableRequestButton(bool a_State)
        {
            requestWithdrawButton.interactable = a_State;
            requestWithdrawButtonText.color = a_State ? Color.white : Color.gray;
        }

        private void onConversion(ConvertedCoinResponse a_Obj)
        {
            walletHandlerRef.SetConvertedCoinResponse(a_Obj);
            dollarField.text = a_Obj.DollarAmount.ToString("N", new CultureInfo("en-US"));
            bitcoinField.text = a_Obj.BtcAmount;
        }

        private void onError(RestUtil.RestCallError a_Obj)
        {
            Debug.LogError(a_Obj.Error);
        }

        public void RequestWithdraw()
        {
            if (requestInProgress)
                return;

            walletKeyField.text = walletKeyField.text.NonWhitespaceStr();
            int t_WalletKeyLength = walletKeyField.text.Length;

            if (withdrawAmount >= minimumWithdrawlAmount && withdrawAmount <= User.UserCoin && !string.IsNullOrEmpty(walletKeyField.text) && !string.IsNullOrWhiteSpace(walletKeyField.text) && t_WalletKeyLength >= walletKeyMinimumLength)
            {
                confirmWindow.SetActive(true);
            }
            else
            {
                // Do nothing....
            }

            if (t_WalletKeyLength < walletKeyMinimumLength)
            {
                flashNotAValidWalletKeyWarning();
            }
            if (withdrawAmount > User.UserCoin)
            {
                notEnoughCoinsWarningText.text = string.Format($"You have {User.UserCoin} and cannot withdraw more than this amount.");
                notEnoughCoinsWarningWindow.SetActive(true);
            }
            else
            {
                // Do nothing....
            }
        }

        private void onRequest(RequestWithdrawResponse a_Obj)
        {
            walletHandlerRef.SetRequestWithdrawResponse(a_Obj);
            withdrawAmount = 0;
            requestInProgress = false;


            walletHandlerRef.OnCompleteAction();
            receivingDollarText.text = string.Format($"You will receive ${walletHandlerRef.GetConvertedCoinResponse().DollarAmount.ToString("N", new CultureInfo("en-US"))} in bitcoins in your wallet soon.");

            confirmWindow.SetActive(false);
            confirmationWindow.SetActive(true);


            CharacterSelection.Instance.GetUnreadMail();
        }

        public void ConfirmWithdraw()
        {
            if (requestInProgress)
                return;

            requestInProgress = true;
            RequestWithdrawRequest t_Request = new RequestWithdrawRequest() { AmountUsd = walletHandlerRef.GetConvertedCoinResponse().DollarAmount, CoinAmount = withdrawAmount, UserEmail = User.UserEmailId, UserName = User.UserName, WalletKey = walletKeyField.text };
            RestManager.WalletRequestWithdraw(t_Request, onRequest, (error) =>
            {
                onError(error);
                requestInProgress = false;
            });
        }

        public void CancelWithdraw()
        {
            if (requestInProgress)
                return;

            CloseWindow();
        }

        public void CloseWindow()
        {
            resetAttributes();
            confirmWindow.SetActive(false);
            confirmationWindow.SetActive(false);
            UIManager.Instance.HideScreen(Page.WalletWithdrawPanel.ToString());
        }

        private void resetAttributes()
        {
            enableRequestButton(false);
            requestInProgress = false;
            withdrawAmount = 0;
            amountField.text = string.Empty;
            dollarField.text = string.Empty;
            bitcoinField.text = string.Empty;
            walletKeyField.text = string.Empty;
            receivingDollarText.text = string.Empty;
        }

        private void flashNotAValidWalletKeyWarning()
        {
            warningSequence.Kill();
            warningSequence = DOTween.Sequence();
            warningSequence.Append(walletKeyWarningText.DOFade(1.0f, .5f));
            warningSequence.AppendInterval(3.0f);
            warningSequence.Append(walletKeyWarningText.DOFade(0.0f, .5f));
            warningSequence.Play();
        }

        private void OnDestroy()
        {
            walletHandlerRef.SubscribeToEvent(this, false);
        }

        public void OnReceiveWalletDetails(WalletDetailsResponse a_WalletDetailsResponse)
        {
            minimumAmountHintText.text = string.Format($"Minimum amount of {a_WalletDetailsResponse.MinimumWithdrawl} is required to Withdraw.");
            marketVolatilityText.text = a_WalletDetailsResponse.MarketVolatility;
            
            if (float.TryParse(a_WalletDetailsResponse.TransactionFeeDeposit, out float t_TransactionFeeDeposit))
                transactionFeeText.text = t_TransactionFeeDeposit > 0.0f ? string.Format($"The amount above reflects a {a_WalletDetailsResponse.TransactionFeeWithdrawl}% transaction fee.") : "";
            else
                transactionFeeText.text = "";

            minimumWithdrawlAmount = 0;
            int.TryParse(a_WalletDetailsResponse.MinimumWithdrawl, out minimumWithdrawlAmount);

            if (!string.IsNullOrEmpty(a_WalletDetailsResponse.AllowMiniAccountWithdrawal))
                allowMiniAccountWithdrawal = string.Equals(a_WalletDetailsResponse.AllowMiniAccountWithdrawal.ToUpper(), "YES") ? true : false;
        }

        public void OnCompleteWalletAction()
        {

        }
    }
}