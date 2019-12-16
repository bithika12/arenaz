using System.Collections.Generic;
using UnityEngine;

namespace RedApple
{
    public class Config : Singleton<Config>
    {

        #region Serialized Fields
#pragma warning disable 649
        [SerializeField]
        private Configuration configuration;
#pragma warning restore
        #endregion
        
        public class Api
        {
            public static string Host
            {
                get
                {
                    return Instance.configuration.Api.Host;
                }
            }
        }

        public class IpApi
        {
            public static string IpHost { get { return Instance.configuration.ipApi.ipHost; } }
        }

        public class SocketConfig
        {
            public static string Host { get { return Instance.configuration.socketConfig.socketHost; } }
            public static List<string> SocketListenEvents { get { return Instance.configuration.socketConfig.SocketListenEvents; } }
        }

        //public class Common
        //{
        //    public static string DefaultLoadingPromptText { get { return instance.configuration.Common.DefaultLoadingPromptText; } }
        //    public static string DefaultLocale { get { return instance.configuration.Common.DefaultLocale; } }
        //}

        //public class Photon
        //{
        //    public static string Address { get { return instance.configuration.Photon.Address; } }
        //    public static int Port { get { return instance.configuration.Photon.Port; } }
        //}
    }

}
