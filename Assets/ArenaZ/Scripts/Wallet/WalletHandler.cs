using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using RedApple;
using System;
using RedApple.Utils;

namespace ArenaZ.Wallet
{
    public class WalletHandler : MonoBehaviour
    {
        [SerializeField] private WalletDetailsResponse walletDetailsResponseObj;
        [SerializeField] private ConvertedCoinResponse convertedCoinResponseObj;
        [SerializeField] private RequestDepositResponse requestDepositResponseObj;
        [SerializeField] private ConfirmDepositResponse confirmDepositResponseObj;
        [SerializeField] private CancelDepositResponse cancelDepositResponseObj;
        [SerializeField] private RequestWithdrawResponse requestWithdrawResponseObj;

        public WalletDetailsResponse GetWalletDetailsResponse() => walletDetailsResponseObj;

        public void SetConvertedCoinResponse(ConvertedCoinResponse a_Data) => convertedCoinResponseObj = a_Data;
        public ConvertedCoinResponse GetConvertedCoinResponse() => convertedCoinResponseObj;

        public void SetRequestDepositResponse(RequestDepositResponse a_Data) => requestDepositResponseObj = a_Data;
        public RequestDepositResponse GetRequestDepositResponse() => requestDepositResponseObj;

        public void SetConfirmDepositResponse(ConfirmDepositResponse a_Data) => confirmDepositResponseObj = a_Data;
        public ConfirmDepositResponse GetConfirmDepositResponse() => confirmDepositResponseObj;

        public void SetCancelDepositResponse(CancelDepositResponse a_Data) => cancelDepositResponseObj = a_Data;
        public CancelDepositResponse GetCancelDepositResponse() => cancelDepositResponseObj;

        public void SetRequestWithdrawResponse(RequestWithdrawResponse a_Data) => requestWithdrawResponseObj = a_Data;
        public RequestWithdrawResponse GetRequestWithdrawResponse() => requestWithdrawResponseObj;

        public void GetWalletDetails()
        {
            WalletDetailsRequest t_Request = new WalletDetailsRequest() { UserEmail = User.UserEmailId };
            RestManager.WalletDetails(t_Request, onReceiveDetails, onError);
        }

        private void onReceiveDetails(WalletDetailsResponse a_Obj)
        {
            walletDetailsResponseObj = a_Obj;
        }

        private void onError(RestUtil.RestCallError a_Obj)
        {
            Debug.LogError(a_Obj.Error);
        }

        public void OnCompleteAction()
        {
            GameMode.ShootingRange.Instance.Refresh();
        }
    }
}