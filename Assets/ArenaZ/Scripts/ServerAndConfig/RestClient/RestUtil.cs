#define DEBUG_REST_CALLS

using RedApple.Utils.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RedApple.Utils
{
    public class RestUtil<T> where T : new()
    {
        private const long HTTP_OK = 200;
        private const long HTTP_CREATED = 201;

        public UnityWebRequest CurrentRequest { get { return currentCall.Request; } }

        private bool uploading { get { return currentCall.Request.method.Equals("POST"); } }
        public float Progress { get { return uploading ? currentCall.Request.uploadProgress : currentCall.Request.downloadProgress; } }
        public ulong TransmitedBytes { get { return uploading ? currentCall.Request.uploadedBytes : currentCall.Request.downloadedBytes; } }

        private MonoBehaviour monoBehaviour;
        private int callCounter = 0;
        private Queue<Call> callQueue = new Queue<Call>();
        private Call currentCall;

        private RestUtil(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
            Start();
        }

        public static RestUtil<T> Initialize(MonoBehaviour monoBehaviour)
        {
            return new RestUtil<T>(monoBehaviour);
        }

        //public IEnumerator GetRaw(string url, RequestCompletedDelegate<byte[]> onCompletion, RequestErrorDelegate onError)
        //{
        //    DownloadHandler handler = new DownloadHandlerBuffer();
        //    yield return get(url, handler, onCompletion, onError);
        //}

        //public IEnumerator Get<T>(string url, RequestCompletedDelegate<T> onCompletion, RequestErrorDelegate onError)
        //{
        //    DownloadHandler handler;

        //    if (T is string)
        //    {
        //        handler = new DownloadHandlerBuffer();
        //        yield return get(url, handler, onCompletion, onError);
        //    }
        //}

        public void Start()
        {
            monoBehaviour.StartCoroutine(run());
        }

        public void OnDestroy()
        {
            monoBehaviour.StopCoroutine(run());
        }

        public int Send(WebRequestBuilder builder,
            Action<DownloadHandler> onCompletion, Action<RestCallError> onError)
        {
            callQueue.Enqueue(new Call()
            {
                Builder = builder,
                OnCompletion = onCompletion,
                OnError = onError
            });

            return callCounter++;
        }

        private IEnumerator run()
        {
            do
            {
                do
                {
                    yield return new WaitForEndOfFrame();
                }
                while (currentCall == null && callQueue.Count == 0);

                currentCall = callQueue.Dequeue();
                currentCall.Request = currentCall.Builder.Build();
                #if DEBUG_REST_CALLS
                Debug.LogFormat("Making call to: {0}", currentCall.Request.url);
                #endif
                yield return currentCall.Request.SendWebRequest();

#if DEBUG_REST_CALLS
                Debug.LogFormat("Call completed with status {0}", currentCall.Request.responseCode);
#endif
                if (currentCall.Request.responseCode == HTTP_OK || currentCall.Request.responseCode == HTTP_CREATED)
                {
                    currentCall.OnCompletion(currentCall.Request.downloadHandler);
                }
                else
                {
#if DEBUG_REST_CALLS
                    Debug.LogFormat("Called: {0}\nResponse: {1}", currentCall.Request.url, currentCall.Request.downloadHandler.text);
#endif
                    RestCallError restCallError = new RestCallError()
                    {
                        Raw = currentCall.Request.downloadHandler.text,
                        Code = currentCall.Request.responseCode,
                        Headers = currentCall.Request.GetResponseHeaders(),
                    };

                    //OauthErrorResponse oauthResponse =
                    //    JsonConvert.DeserializeObject<OauthErrorResponse>(currentCall.Request.downloadHandler.text);

                    //if (!string.IsNullOrEmpty(oauthResponse.Error))
                    //{
                    //    restCallError.Error = oauthResponse.Error;
                    //    restCallError.Description = oauthResponse.Description;
                    //    restCallError.Response = null;
                    //}
                    //else
                    //{
                    //    restCallError.Response = JsonConvert.DeserializeObject<ApiErrorResponse>(currentCall.Request.downloadHandler.text);
                    //    restCallError.Error = currentCall.Request.error;
                    //}

                    currentCall.OnError(restCallError);
                }
                currentCall.Request.Dispose();
                currentCall = null;

            } while (true);
        }

        public struct RestCallError
        {
            public string Raw;
            public long Code;
            public string Error;
            public string Description;
            public Dictionary<string, string> Headers;
            public T Response;
        }

        private class Call
        {
            public WebRequestBuilder Builder;
            public Action<DownloadHandler> OnCompletion;
            public Action<RestCallError> OnError;
            public UnityWebRequest Request;
        }
    }
}