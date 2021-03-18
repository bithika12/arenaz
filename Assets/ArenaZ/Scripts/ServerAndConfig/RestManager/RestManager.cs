using System;
using RedApple.Utils.Rest;
using RedApple.Api;
using RedApple.Api.Data;
using UnityEngine;
using Newtonsoft.Json;
using RestUtil = RedApple.Utils.RestUtil;
using RestError = RedApple.Utils.RestUtil.RestCallError;
using ArenaZ.Manager;
using ArenaZ;
using ArenaZ.Wallet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RedApple
{
    public class RestManager : Singleton<RestManager>
    {

        public static string AccessToken { set { Instance.userAccessToken = value; } }

        private RestUtil restUtil;

        private string userAccessToken;

        private string clientAccessToken = "";

        protected override void Awake()
        {
            restUtil = RestUtil.Initialize(this);
        }

        #region API Call Methods
        public static void UpdateUserProfile<T>(string name, string locaton, Action<T> onCompletion,
            Action<RestError> onError)
        {           
            var builder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.PROFILE))
                .Verb(Verbs.PUT)
                .FormData(Attributes.NAME, name)
                .FormData(Attributes.LOCATION, locaton);

            addUserAuthHeader(ref builder);
            sendWebRequest(builder, onCompletion, onError);
        }
        
        public static void UserRegistration(string email_id, string name, string password, Action<CreateAccount> onCompletionRegistration, Action<RestError> restError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
            .Url(getApiUrl(Urls.REGISTER))
            .Verb(Verbs.POST)
            .ContentType(ContentTypes.FORM)
            .FormData(Attributes.EMAIL_ID, email_id)
            .FormData(Attributes.NAME, name)
            .FormData(Attributes.PASSWORD, password);

            sendWebRequest(webRqstBuilder, onCompletionRegistration, restError);
        }

        public static void LoginProfile(string email_id, string password, Action<UserLogin> onCompletionLogin, Action<RestError> restError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.USER_LOGIN))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.EMAIL_ID, email_id)
                .FormData(Attributes.PASSWORD, password);
            
            sendWebRequest(webRqstBuilder, onCompletionLogin, restError);
        }

        public static void LogOutProfile(Action OnCompleteLogOut,Action<RestError> restError)
        {
            WebRequestBuilder webRqstbuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.LOGOUT))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM);

            addUserAuthHeader(ref webRqstbuilder);
            sendWebRequest(webRqstbuilder, OnCompleteLogOut, restError);
        }

        public static void DeleteAccount(Action a_OnComplete, Action<RestError> a_RestError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.DELETE_ACCOUNT))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM);

            addUserAuthHeader(ref webRqstBuilder);
            sendWebRequest(webRqstBuilder, a_OnComplete, a_RestError);
        }

        public static void ForgotPassword(bool isEmail, string emailOrUserName, Action OnCompleteForgotPassword, Action<RestError> restError)
        {
            WebRequestBuilder webRqstbuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.FORGOT_PASSWORD))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(isEmail ? Attributes.EMAIL_ID : Attributes.NAME, emailOrUserName);

            sendWebRequest(webRqstbuilder, OnCompleteForgotPassword, restError);
        }

        public static void GetUnreadMailCount(string email_id, Action<UnreadMailCountData> onCompletionSaveData, Action<RestError> restError)
        {
            Debug.Log("URL: " + getApiUrl(Urls.FETCH_UNREAD_MESSAGE));
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.FETCH_UNREAD_MESSAGE))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.USER_EMAIL, email_id);

            sendWebRequest(webRqstBuilder, onCompletionSaveData, restError);
        }

        public static void SaveUserSelection(string email_id, UserSelectionDetails userSelectionDetails, Action onCompletionSaveData, Action<RestError> restError)
        {
            Debug.Log($"User Email: {email_id}, Data: {JsonConvert.SerializeObject(userSelectionDetails)}");
            Debug.Log("URL: " + getApiUrl(Urls.SAVE_SELECTION_DETAILS));
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.SAVE_SELECTION_DETAILS))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.USER_EMAIL, email_id)
                .FormData(Attributes.COLOR_NAME, userSelectionDetails.ColorName)
                .FormData(Attributes.RACE_NAME, userSelectionDetails.RaceName)
                .FormData(Attributes.CHARACTER_ID, userSelectionDetails.CharacterId)
                .FormData(Attributes.DART_NAME, userSelectionDetails.DartName)
                .FormData(Attributes.COUNTRY_NAME, userSelectionDetails.CountryName)
                .FormData(Attributes.LANGUAGE_NAME, userSelectionDetails.LanguageName);

            sendWebRequest(webRqstBuilder, onCompletionSaveData, restError);
        }

        public static void GetUserSelection(string email_id, Action<UserSelectionDetails> onCompletionGetData, Action<RestError> restError)
        {
            Debug.Log($"User Email: {email_id}");
            Debug.Log("URL: " + getApiUrl(Urls.GET_SELECTION_DETAILS));
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.GET_SELECTION_DETAILS))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.USER_EMAIL, email_id);

            sendWebRequest(webRqstBuilder, onCompletionGetData, restError);
        }

        public static void GetCountryDetails(Action<CountryData> OnCompleteteCountryDetailsFetch,Action<RestError> restError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getIpApiUrl(Urls.JSON))
                .Verb(Verbs.GET)
                .ContentType(ContentTypes.FORM);

            sendWebRequestForCountryDetails(webRqstBuilder, OnCompleteteCountryDetailsFetch, restError);
        }

        public static void GetGameHistory(string a_UserEmail, Action<GameHistoryMatchDetails> a_OnComplete, Action<RestError> a_RestError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.GET_GAME_HISTORY))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.USER_EMAIL, a_UserEmail);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_RestError);
        }

        public static void GetLastGameHistory(string a_UserEmail, Action<LastGameHistory> a_OnComplete, Action<RestError> a_RestError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.GET_USER_GAME_HISTORY))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.USER_EMAIL, a_UserEmail);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_RestError);
        }

        public static void FetchNotifications(string a_UserEmail, Action<MessageDetails> a_OnComplete, Action<RestError> a_RestError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.FETCH_NOTIFICATIONS))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.USER_EMAIL, a_UserEmail);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_RestError);
        }

        public static void ChangeNotificationStatus(string a_UserEmail, string a_NotificationId, Action a_OnComplete, Action<RestError> a_RestError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.CHANGE_NOTIFICATION_STATUS))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.USER_EMAIL, a_UserEmail)
                .FormData(Attributes.NOTIFICATION_ID, a_NotificationId);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_RestError);
        }

        public static void FetchLeaderboard(string a_UserEmail, Action<LeaderboardDetails> a_OnComplete, Action<RestError> a_RestError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.FETCH_LEADERBOARD))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.USER_EMAIL, a_UserEmail);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_RestError);
        }

        public static void AppVersionCheck(string appVersion, Action<VersionCheckerResponse> onCompletionLogin, Action<RestError> restError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.APP_VERSION_CHECK))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(Attributes.APP_VERSION, appVersion);

            sendWebRequest(webRqstBuilder, onCompletionLogin, restError);
        }

        //public static void WalletDetails(WalletDetailsRequest a_RequestObj, Action<WalletDetailsResponse> a_OnComplete, Action<RestError> a_OnError)
        //{
        //    IDictionary<string, string> t_ClassData = null;
        //    t_ClassData = t_ClassData.ObjectToDictionaryField<WalletDetailsRequest>(a_RequestObj);

        //    if (t_ClassData != null && t_ClassData.Any())
        //    {
        //        WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
        //            .Url(getApiUrl(Urls.WALLET_USER_GET_COIN))
        //            .Verb(Verbs.POST)
        //            .ContentType(ContentTypes.FORM)
        //            .FormData(t_ClassData);

        //        sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        //    }
        //}

        public static void WalletDetails(WalletDetailsRequest a_RequestObj, Action<WalletDetailsResponse> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.WALLET_USER_GET_COIN))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData("userEmail", a_RequestObj.UserEmail);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }

        public static void WalletConvertedCoin(ConvertedCoinRequest a_RequestObj, Action<ConvertedCoinResponse> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.WALLET_CURRENCY_PRICE))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData("coin_number", a_RequestObj.CoinNumber)
                .FormData("userEmail", a_RequestObj.UserEmail)
                .FormData("transactionType", a_RequestObj.TransactionType);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }

        public static void WalletRequestDeposit(RequestDepositRequest a_RequestObj, Action<RequestDepositResponse> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.WALLET_REQUEST_DEPOSIT))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData("userEmail", a_RequestObj.UserEmail)
                .FormData("coinAmount", a_RequestObj.CoinAmount)
                .FormData("user_name", a_RequestObj.UserName)
                .FormData("amount_usd", a_RequestObj.AmountUsd.ToString());

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }

        public static void WalletRequestWithdraw(RequestWithdrawRequest a_RequestObj, Action<RequestWithdrawResponse> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.WALLET_REQUEST_WITHDRAW))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData("userEmail", a_RequestObj.UserEmail)
                .FormData("coinAmount", a_RequestObj.CoinAmount)
                .FormData("user_name", a_RequestObj.UserName)
                .FormData("amount_usd", a_RequestObj.AmountUsd.ToString())
                .FormData("wallet_key", a_RequestObj.WalletKey);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }

        public static void WalletCancelDeposit(CancelDepositRequest a_RequestObj, Action<CancelDepositResponse> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.WALLET_CANCEL_DEPOSIT))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData("userEmail", a_RequestObj.UserEmail)
                .FormData("transactionId", a_RequestObj.TransactionId);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }

        public static void WalletConfirmDeposit(ConfirmDepositRequest a_RequestObj, Action<ConfirmDepositResponse> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.WALLET_CONFIRM_DEPOSIT))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData("userEmail", a_RequestObj.UserEmail)
                .FormData("transactionId", a_RequestObj.TransactionId);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }

        public static void WalletTransactionStatus(TransactionStatusRequest a_RequestObj, Action<TransactionStatusResponse> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.WALLET_CHECK_TRANSACTION_STATUS))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData("userEmail", a_RequestObj.UserEmail)
                .FormData("transactionId", a_RequestObj.TransactionId);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }

        public static void GetWalletHistory(string a_UserName, Action<WalletHistoryList> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.WALLET_HISTORY))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData("user_name", a_UserName);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }

        public static void GetGameList(Action<GameListResponse> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                    .Url(getApiUrl(Urls.GAME_ALLGAMES))
                    .Verb(Verbs.POST)
                    .ContentType(ContentTypes.FORM);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }

        public static void GetMasterData(string a_UserEmail, Action<MasterData> a_OnComplete, Action<RestError> a_OnError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.GET_MASTER_DATA))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData("userEmail", a_UserEmail);

            sendWebRequest(webRqstBuilder, a_OnComplete, a_OnError);
        }
        #endregion

        private static void sendWebRequest(WebRequestBuilder builder, Action onCompletion, Action<RestError> onError)
        {
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
            Instance.restUtil.Send(builder, handler => { onCompletion?.Invoke(); },
                restError => interceptError(restError, () => onError?.Invoke(restError), onError));
        }

        private static void sendWebRequest<T>(WebRequestBuilder builder, Action<T> onCompletion,
            Action<RestError> onError = null)
        {
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
            Instance.restUtil.Send(builder,
                handler =>
                {
                    try
                    {
                        var response = DataConverter.DeserializeObject<ApiResponseFormat<T>>(handler.text);
                        onCompletion?.Invoke(response.Result);
                    } catch(Exception e)
                    {
                        Debug.LogError($"Rest Api Sent Error: {e.ToString()}");
                    }
                    
                },
                restError => interceptError(restError, () => onError?.Invoke(restError), onError));
        }

        private static void sendWebRequestForCountryDetails(WebRequestBuilder builder, Action<CountryData> onCompletion,
           Action<RestError> onError = null)
        {
            if (!GameManager.Instance.InternetConnection())
            {
                //UIManager.Instance.ShowScreen(Page.InternetConnectionLostPanel.ToString());
                return;
            }
            Instance.restUtil.Send(builder,
                handler =>
                {
                    var response = DataConverter.DeserializeObject<CountryData>(handler.text);
                    onCompletion?.Invoke(response);
                },
                restError => interceptError(restError, () => onError?.Invoke(restError), onError));
        }

        private static void interceptError(RestError error, Action onSuccess,
            Action<RestError> defaultOnError)
        {
             defaultOnError?.Invoke(error);
        }

        private static string getApiUrl(string path)
        {
            return $"{Config.Api.Host}{path}";
        }

        private static string getIpApiUrl(string path)
        {
            return $"{Config.IpApi.IpHost}{path}";
        }

        protected static string FormatApiUrl(string path, params object[] args)
        {
            return string.Format($"{Config.Api.Host}{path}", args);
        }

        private static void addSecurityHeaders(ref WebRequestBuilder builder)
        {
            //builder.FormData("client_id", Config.ApiClientId)
            //    .FormData("client_secret", Config.ApiClientSecret);
        }

        private static void addUserAuthHeader(ref WebRequestBuilder builder)
        {
            builder.Header("access_token", Instance.userAccessToken);
            Debug.Log("User Access Token: " + Instance.userAccessToken);
        }
    }
}