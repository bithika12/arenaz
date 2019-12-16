using System.Collections.Generic;
using System;

namespace RedApple
{
    public static class SocketListener
    {
        private static Dictionary<string, List<Action<string>>> m_LitenerDatas = new Dictionary<string, List<Action<string>>>();
       
        public static void ActivateListener()
        {
            SocketManager.onListen += onListen;
        }

        public static void Listen(string eventName, Action<string> callback)
        {
            if (!m_LitenerDatas.TryGetValue(eventName, out var list))
            {
                list = new List<Action<string>>();
                m_LitenerDatas.Add(eventName, list);
            }

            if (!list.Contains(callback))
                m_LitenerDatas[eventName].Add(callback);
        }

        private static void onListen(string eventName, string data)
        {
            UnityEngine.Debug.Log("eventName:  " + eventName);
            UnityEngine.Debug.Log("Data:  " + data);
            if (m_LitenerDatas.ContainsKey(eventName))
            {
                m_LitenerDatas[eventName].ForEach(callback => callback?.Invoke(data));
            }
        }
    }
}
