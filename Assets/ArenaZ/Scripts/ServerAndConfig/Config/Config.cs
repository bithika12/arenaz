using UnityEngine;

namespace RedApple
{
    public class Config : MonoBehaviour
    {
        private static Config instance;

        #region Serialized Fields
#pragma warning disable 649
        [SerializeField]
        private Configuration configuration;
#pragma warning restore
        #endregion

        public void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this;
        }

        public class Api
        {
            public static string Host { get { return instance.configuration.Api.Host; } }
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
