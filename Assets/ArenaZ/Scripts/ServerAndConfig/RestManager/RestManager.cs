using System;
using RedApple.Utils.Rest;
using RedApple.Api;
using RedApple.Api.Data;
using UnityEngine;
using RestUtil = RedApple.Utils.RestUtil;
using RestError = RedApple.Utils.RestUtil.RestCallError;
using UnityEngine.Networking;

namespace RedApple
{
    public class RestManager : MonoBehaviour
    {
        private static RestManager instance;

        public static string AccessToken { set { instance.userAccessToken = value; } }

        private RestUtil restUtil;

        private string userAccessToken;

        private string clientAccessToken = "";

        public void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

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
        public static void UserRegistration(string email_id, string name, string password, string confirmPassword, Action<CreateAccount> onCompletionRegistration, Action<RestError> restError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
            .Url(getApiUrl(Urls.REGISTER))
            .Verb(Verbs.POST)
            .ContentType(ContentTypes.FORM)
            .FormData(Attributes.EMAIL_ID, email_id)
            .FormData(Attributes.NAME, name)
            .FormData(Attributes.PASSWORD, password)
            .FormData(Attributes.CONFIRM_PASSWORD, confirmPassword);

            addUserAuthHeader(ref webRqstBuilder);
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

        public static void LogOutProfile(Action<UserLogin> OnCompleteLogOut,Action<RestError> restError)
        {
            WebRequestBuilder webRqstbuilder = new WebRequestBuilder()
                .Url(getApiUrl(Urls.LOGOUT))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.FORM);

            addUserAuthHeader(ref webRqstbuilder);
            sendWebRequest(webRqstbuilder, OnCompleteLogOut, restError);
        }
        #endregion

        public static void GetCountryDetails(Action<CountryDetails> OnCompleteteCountryDetailsFetch,Action<RestError> restError)
        {
            WebRequestBuilder webRqstBuilder = new WebRequestBuilder()
                .Url(getIpApiUrl(Urls.JSON))
                .Verb(Verbs.GET)
                .ContentType(ContentTypes.FORM);

            sendWebRequestForCountryDetails(webRqstBuilder, OnCompleteteCountryDetailsFetch, restError);
        }

        private static void sendWebRequest(WebRequestBuilder builder, Action onCompletion, Action<RestError> onError)
        {
            instance.restUtil.Send(builder, handler => { onCompletion?.Invoke(); },
                restError => interceptError(restError, () => onError?.Invoke(restError), onError));
        }

        private static void sendWebRequest<T>(WebRequestBuilder builder, Action<T> onCompletion,
            Action<RestError> onError = null)
        {
            instance.restUtil.Send(builder,
                handler =>
                {
                    var response = DataConverter.DeserializeObject<ApiResponseFormat<T>>(handler.text);
                    onCompletion?.Invoke(response.Result);
                },
                restError => interceptError(restError, () => onError?.Invoke(restError), onError));
        }

        private static void sendWebRequestForCountryDetails(WebRequestBuilder builder, Action<CountryDetails> onCompletion,
           Action<RestError> onError = null)
        {
            instance.restUtil.Send(builder,
                handler =>
                {
                    var response = DataConverter.DeserializeObject<CountryDetails>(handler.text);
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
            builder.Header("access-token", string.Format("Bearer {0}", instance.userAccessToken));
            Debug.Log("User Access Token:  " + instance.userAccessToken);
        }
    }
}