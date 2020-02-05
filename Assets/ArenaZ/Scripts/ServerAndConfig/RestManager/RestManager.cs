using System;
using RedApple.Utils.Rest;
using RedApple.Api;
using RedApple.Api.Data;
using UnityEngine;
using Newtonsoft.Json;
using RestUtil = RedApple.Utils.RestUtil;
using RestError = RedApple.Utils.RestUtil.RestCallError;

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

        #region UserProfile

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

        #endregion
        #region USER_LOGIN_REGISTRATION
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

        public static void ForgotPassword(bool isEmail, string emailOrUserName, Action OnCompleteForgotPassword, Action<RestError> restError)
        {
            WebRequestBuilder webRqstbuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.FORGOT_PASSWORD))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM)
                .FormData(isEmail ? Attributes.EMAIL_ID : Attributes.NAME, emailOrUserName);

            sendWebRequest(webRqstbuilder, OnCompleteForgotPassword, restError);
        }
        #endregion

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
                .FormData(Attributes.DART_NAME, userSelectionDetails.DartName);

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

        private static void sendWebRequest(WebRequestBuilder builder, Action onCompletion, Action<RestError> onError)
        {
            Instance.restUtil.Send(builder, handler => { onCompletion?.Invoke(); },
                restError => interceptError(restError, () => onError?.Invoke(restError), onError));
        }

        private static void sendWebRequest<T>(WebRequestBuilder builder, Action<T> onCompletion,
            Action<RestError> onError = null)
        {
            Instance.restUtil.Send(builder,
                handler =>
                {
                    try
                    {
                        var response = DataConverter.DeserializeObject<ApiResponseFormat<T>>(handler.text);
                        onCompletion?.Invoke(response.Result);
                    } catch(Exception e)
                    {

                    }
                    
                },
                restError => interceptError(restError, () => onError?.Invoke(restError), onError));
        }

        private static void sendWebRequestForCountryDetails(WebRequestBuilder builder, Action<CountryData> onCompletion,
           Action<RestError> onError = null)
        {
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