using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedApple
{
    [CreateAssetMenu(menuName = "RedApple/Configuration")]
    public class Configuration : ScriptableObject
    {
        public ApiConfiguration Api = new ApiConfiguration();
        public IpApiConfiguration ipApi = new IpApiConfiguration();
        public SocketConfiguration socketConfig = new SocketConfiguration();
        
        [Serializable]
        public class ApiConfiguration
        {
            public string Host = "http://52.66.82.72:3012/";
        }

        [Serializable]
        public class IpApiConfiguration
        {
            public string ipHost = "https://freegeoip.app/";
        }

        [Serializable]
        public class SocketConfiguration
        {
            public string socketHost = "http://192.168.2.73:3012/";
            public List<string> SocketListenEvents = new List<string>();
        }
    }
}