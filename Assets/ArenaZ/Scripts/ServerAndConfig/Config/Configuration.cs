﻿using System;
using UnityEngine;

namespace RedApple
{
    [CreateAssetMenu(menuName = "RedApple/Configuration")]
    public class Configuration : ScriptableObject
    {
        public ApiConfiguration Api = new ApiConfiguration();
        public IpApiConfiguration ipApi = new IpApiConfiguration();
        //public CommonConfiguration Common = new CommonConfiguration();
        //public PhotonConfiguration Photon = new PhotonConfiguration();

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

        //[Serializable]
        //public class CommonConfiguration
        //{
        //    public string DefaultLocale = "en-US";
        //    public string DefaultLoadingPromptText = "loading_prompt";
        //}

        //[Serializable]
        //public class PhotonConfiguration
        //{
        //    public string Address = "188.117.216.243";
        //    public int Port = 15055;
        //}
    }
}