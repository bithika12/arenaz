using System.Collections.Generic;

namespace Prime31.MessageKitLite
{
    public interface IMessageReceiver
    {
        void OnMessageReceived(int messageType);
    }

    public interface IMessageReceiver<in T>
    {
        void OnMessageReceived(int messageType, T payload);
    }

    public interface IMessageReceiver<in T1, in T2>
    {
        void OnMessageReceived(int messageType, T1 payload1, T2 payload2);
    }

    /// <summary>
    /// this is a slightly different take dealing with message receivers. Instead of Actions like MessageKit uses receivers must implement
    /// the appropriate MessageReceiver interface. The main advantage to this way of doing things is something that really only matters on a
    /// mobile/resource constrained game with a SHIT-TON of message handlers being added/removed at runtime. Getting rid of the Action means
    /// there is no allocation when adding a receiver.
    /// 
    /// Downsides are having to implement interfaces for each different message type. When working with Actions you get to pass in named methods
    /// which is very handy for code readability. With interfaces you are stuck with a generic name which isn't fabulous.
    /// </summary>
    public static class MessageKitLite
    {
        private static Dictionary<int, List<IMessageReceiver>> _messageTable =
            new Dictionary<int, List<IMessageReceiver>>();

        public static void AddObserver(int messageType, IMessageReceiver handler)
        {
            if (!_messageTable.TryGetValue(messageType, out var list))
            {
                list = new List<IMessageReceiver>();
                _messageTable.Add(messageType, list);
            }

            if (!list.Contains(handler))
                _messageTable[messageType].Add(handler);
        }

        public static void RemoveObserver(int messageType, IMessageReceiver handler)
        {
            if (!_messageTable.TryGetValue(messageType, out var list))
                return;

            if (list.Contains(handler))
                list.Remove(handler);
        }

        public static void Post(int evt)
        {
            if (!_messageTable.TryGetValue(evt, out var list))
                return;

            for (var i = list.Count - 1; i >= 0; i--)
                list[i].OnMessageReceived(evt);
        }

        public static void ClearMessageTable(int messageType)
        {
            if (_messageTable.ContainsKey(messageType))
                _messageTable.Remove(messageType);
        }


        public static void ClearMessageTable()
        {
            _messageTable.Clear();
        }
    }


    public static class MessageKitLite<T>
    {
        private static readonly Dictionary<int, List<IMessageReceiver<T>>> _messageTable =
            new Dictionary<int, List<IMessageReceiver<T>>>();

        public static void AddObserver(int messageType, IMessageReceiver<T> handler)
        {
            if (!_messageTable.TryGetValue(messageType, out var list))
            {
                list = new List<IMessageReceiver<T>>();
                _messageTable.Add(messageType, list);
            }

            if (!list.Contains(handler))
                _messageTable[messageType].Add(handler);
        }

        public static void RemoveObserver(int messageType, IMessageReceiver<T> handler)
        {
            if (!_messageTable.TryGetValue(messageType, out var list)) 
                return;
            
            if (list.Contains(handler))
                list.Remove(handler);
        }


        public static void Post(int evt, T param)
        {
            if (!_messageTable.TryGetValue(evt, out var list)) 
                return;
            
            for (var i = list.Count - 1; i >= 0; i--)
                list[i].OnMessageReceived(evt, param);
        }

        public static void ClearMessageTable(int messageType)
        {
            if (_messageTable.ContainsKey(messageType))
                _messageTable.Remove(messageType);
        }


        public static void ClearMessageTable()
        {
            _messageTable.Clear();
        }
    }


    public static class MessageKitLite<T1, T2>
    {
        private static readonly Dictionary<int, List<IMessageReceiver<T1, T2>>> _messageTable =
            new Dictionary<int, List<IMessageReceiver<T1, T2>>>();

        public static void AddObserver(int messageType, IMessageReceiver<T1, T2> handler)
        {
            if (!_messageTable.TryGetValue(messageType, out var list))
            {
                list = new List<IMessageReceiver<T1, T2>>();
                _messageTable.Add(messageType, list);
            }

            if (!list.Contains(handler))
                _messageTable[messageType].Add(handler);
        }

        public static void RemoveObserver(int messageType, IMessageReceiver<T1, T2> handler)
        {
            if (!_messageTable.TryGetValue(messageType, out var list)) 
                return;

            if (list.Contains(handler))
                list.Remove(handler);
        }

        public static void Post(int evt, T1 firstParam, T2 secondParam)
        {
            if (!_messageTable.TryGetValue(evt, out var list)) 
                return;
            
            for (var i = list.Count - 1; i >= 0; i--)
                list[i].OnMessageReceived(evt, firstParam, secondParam);
        }

        public static void ClearMessageTable(int messageType)
        {
            if (_messageTable.ContainsKey(messageType))
                _messageTable.Remove(messageType);
        }


        public static void ClearMessageTable()
        {
            _messageTable.Clear();
        }
    }
}