using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RedApple;
using System;
using RedApple.Utils;
using ArenaZ.Manager;
using DG.Tweening;

namespace ArenaZ.Wallet
{
    public class WalletDepositConfirm : MonoBehaviour
    {
        [SerializeField] private WalletHandler walletHandlerRef;
        [SerializeField] private CountdownTimer countdownTimerRef;
        [SerializeField] private TMP_InputField amountField, walletKeyField, dollarField, bitcoinField;
        [SerializeField] private TextMeshProUGUI countdownTimerText, toastText;

        private bool requestInProgress = false;
        private Sequence toastSequence;

        public void Initialize(int a_Amount)
        {
            amountField.text = a_Amount.ToString();
            dollarField.text = walletHandlerRef.GetConvertedCoinResponse().DollarAmount.ToString();
            bitcoinField.text = walletHandlerRef.GetConvertedCoinResponse().BtcAmount.ToString();
            walletKeyField.text = walletHandlerRef.GetRequestDepositResponse().TransactionId;

            countdownTimerRef.StartCountdown(walletHandlerRef.GetRequestDepositResponse().ExpiredAtInSecond, false, (t) => countdownTimerText.text = "Time Left " + t, () => countdownTimerText.text = "Time Left 00:00");
        }

        public void ConfirmDeposit()
        {
            if (requestInProgress)
                return;

            requestInProgress = true;
            ConfirmDepositRequest t_Request = new ConfirmDepositRequest() { UserEmail = User.UserEmailId, TransactionId = walletHandlerRef.GetRequestDepositResponse().TransactionId };
            RestManager.WalletConfirmDeposit(t_Request, onConfirm, onError);

            UIManager.Instance.HideScreen(Page.WalletDepositConfirmPanel.ToString());
        }

        private void onConfirm(ConfirmDepositResponse a_Obj)
        {
            walletHandlerRef.OnCompleteAction();
            walletHandlerRef.SetConfirmDepositResponse(a_Obj);

            requestInProgress = false;
            resetAttributes();
        }

        private void onError(RestUtil.RestCallError a_Obj)
        {
            Debug.LogError(a_Obj.Error);
        }

        public void CancelDeposit()
        {
            if (requestInProgress)
                return;

            requestInProgress = true;
            CancelDepositRequest t_Request = new CancelDepositRequest() { UserEmail = User.UserEmailId, TransactionId = walletHandlerRef.GetRequestDepositResponse().TransactionId };
            RestManager.WalletCancelDeposit(t_Request, onCancel, onError);

            UIManager.Instance.HideScreen(Page.WalletDepositConfirmPanel.ToString());

            resetAttributes();
        }

        private void onCancel(CancelDepositResponse a_Obj)
        {
            walletHandlerRef.SetCancelDepositResponse(a_Obj);
            requestInProgress = false;
        }

        public void CopyWalletKey()
        {
            UniClipboard.SetText(walletHandlerRef.GetRequestDepositResponse().TransactionId);
            toastSequence.Kill();
            toastSequence = DOTween.Sequence();
            toastSequence.Append(toastText.DOFade(1.0f, .5f));
            toastSequence.AppendInterval(1.0f);
            toastSequence.Append(toastText.DOFade(0.0f, .5f));
            toastSequence.Play();
        }

        private void resetAttributes()
        {
            amountField.text = string.Empty;
            dollarField.text = string.Empty;
            bitcoinField.text = string.Empty;
            walletKeyField.text = string.Empty;

            countdownTimerRef.StopCountdown();
        }
    }
}