using System.Collections.Generic;
using System;
using UnityEngine;

namespace RedApple
{
    public static class SocketListener
    {
        private static Dictionary<string, List<Action<string>>> m_ListenerDatas = new Dictionary<string, List<Action<string>>>();
       
        public static void ActivateListener()
        {
            SocketManager.onListen += onListen;
        }

        public static void Listen(string eventName, Action<string> callback)
        {
            if (!m_ListenerDatas.TryGetValue(eventName, out var list))
            {
                list = new List<Action<string>>();
                m_ListenerDatas.Add(eventName, list);
            }

            if (!list.Contains(callback))
                m_ListenerDatas[eventName].Add(callback);
        }

        private static void onListen(string eventName, string data)
        {
            Debug.Log($"ReceivedData:- \nEventName: {eventName} \nData: {data}");
            if (m_ListenerDatas.ContainsKey(eventName))
            {
                m_ListenerDatas[eventName].ForEach(callback => callback?.Invoke(data));
            }
        }
    }
}
