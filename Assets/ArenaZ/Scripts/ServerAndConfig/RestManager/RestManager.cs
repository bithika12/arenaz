using System;
using RedApple.Utils.Rest;
using Newtonsoft.Json;
using ArenaZ.Api;
using UnityEngine;
using RestUtil = RedApple.Utils.RestUtil<ArenaZ.Api.ApiErrorResponse>;
using CreateAccountResponse = ArenaZ.Api.ApiResponseFormat<ArenaZ.Api.CreateAccountResponse>;
using RedApple.Utils;

namespace ArenaZ.Manager
{
    public class RestManager : MonoBehaviour
    {
        private static RestManager instance;

        public static string AccessToken { set { instance.userAccessToken = value; } }
        public static string RefreshToken { set { instance.refreshToken = value; } }

        private RestUtil restUtil;

        private string userAccessToken;
        private string refreshToken;


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

        #region Authentication

        public static void CreateAccount(string data, Action<CreateAccountResponse> onCompletion, Action<RestUtil.RestCallError> onError)
        {
            var builder = new WebRequestBuilder()
                .Url(getApiUrl(APIConstants.API_REGISTER))
                .Verb(Verbs.POST)
                .FormData(APIConstants.FIRST_NAME, "dummyData")
                .FormData(APIConstants.LAST_NAME, "dummyData")
                .FormData(APIConstants.EMAIL_ID, "dummyData")
                .FormData(APIConstants.PASSWORD, "dummyData")
                .FormData(APIConstants.CONFIRM_PASSWORD, "dummyData");

            //addClientAuthHeader(ref builder);

            SendWebRequest<CreateAccountResponse>(builder, onCompletion, onError);
        }

        #endregion

        private static void SendWebRequest<T>(WebRequestBuilder builder, Action<T> onCompletion, Action<RestUtil<ApiErrorResponse>.RestCallError> onError)
        {
            instance.restUtil.Send(builder, handler =>
            {
                Debug.Log($"Response Data of {builder.url} :: {handler.text}");
                var response = DataConverter.DeserializeObject<T>(handler.text);// JsonConvert.DeserializeObject<T>(handler.text);
                if (onCompletion != null)
                {
                    onCompletion(response);
                }
            }, restError =>
            {
                interceptError(
                 restError, () =>
                 {
                     if (onError != null)
                         onError(restError);
                 }, onError);
            });
        }

        private static void interceptError(RestUtil.RestCallError error, Action onSuccess,
            Action<RestUtil.RestCallError> defaultOnError)
        {
            if ("invalid_grant".Equals(error.Error))
            {
                defaultOnError(new RestUtil.RestCallError() { Response = new ApiErrorResponse() { Message = error.Description } });
            }
            else
            {
                defaultOnError(error);
            }
        }

        private static string getApiUrl(string path)
        {
            return string.Format("{0}{1}", Config.Api.Host, path);
        }

        private static string getApiUrlWithPath(string path, params string[] paths)
        {
            string additionalPath = string.Join("/", paths);
            string finalPath = string.Format("{0}{1}{2}", path, "/", additionalPath);
            return string.Format("{0}{1}", Config.Api.Host, finalPath);
        }

        private static void addSecurityHeaders(ref WebRequestBuilder builder)
        {
            //builder.FormData("client_id", Config.ApiClientId)
            //    .FormData("client_secret", Config.ApiClientSecret);
        }

        private static void addUserAuthHeader(ref WebRequestBuilder builder)
        {
            builder.Header("Authorization", string.Format("Bearer {0}", instance.userAccessToken));
        }
    }
}