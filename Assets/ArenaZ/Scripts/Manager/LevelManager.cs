using System;
using RedApple;

namespace ArenaZ.Manager
{
    public class LevelManager : Singleton<LevelManager>
    {
        // Private Variables

        // Public Variables


        private void Awake()
        {
            SocketListener.Listen("LevelUpdate", onListen);
        }

        private void onListen(string data)
        {
            UnityEngine.Debug.Log(data);
        }
    }
}
