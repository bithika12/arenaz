using System;
using UnityEngine;

namespace ArenaZ
{
    [CreateAssetMenu(fileName = "ArenaZConfiguration", menuName = "ServerConfig")]
    public class Configuration : ScriptableObject
    {
        public ApiConfiguration Api = new ApiConfiguration();
        public CommonConfiguration Common = new CommonConfiguration();
        public PhotonConfiguration Photon = new PhotonConfiguration();

        [Serializable]
        public class ApiConfiguration
        {
            public string Host = "http://skylect.local/";
        }

        [Serializable]
        public class CommonConfiguration
        {
            public string DefaultLocale = "en-US";
            public string DefaultLoadingPromptText = "loading_prompt";
        }

        [Serializable]
        public class PhotonConfiguration
        {
            public string Address = "188.117.216.243";
            public int Port = 15055;
        }
    }
}