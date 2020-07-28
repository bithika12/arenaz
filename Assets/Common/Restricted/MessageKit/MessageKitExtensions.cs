#define DEBUG_MESSAGEKIT_CALLS
using System;
using Prime31.MessageKitLite;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedApple
{
    public enum Events
    {
        None = 0,
    }

    public static class MessageKitExtensions
    {
        public static void Observe(this IMessageReceiver iface, params Events[] events)
        {
            foreach (Events evt in events)
                MessageKitLite.AddObserver((int) evt, iface);
        }

        public static void StopObserving(this IMessageReceiver iface, params Events[] events)
        {
            foreach (Events evt in events)
                MessageKitLite.RemoveObserver((int) evt, iface);
        }

        public static void PostMessage(this Object obj, Events evt)
        {
            MessageKitLite.Post((int) evt);
            #if DEBUG_MESSAGEKIT_CALLS
            if (Application.isEditor)
                Debug.LogFormat("[MK] Posting event: {0}", evt);
            #endif
        }

        public static void AddObserver<T>(this IMessageReceiver<T> iface, params Events[] events)
        {
            foreach (Events evt in events)
                MessageKitLite<T>.AddObserver((int) evt, iface); 
        }

        public static void RemoveObserver<T>(this IMessageReceiver<T> iface,
            params Events[] events)
        {
            foreach (Events evt in events)
                MessageKitLite<T>.RemoveObserver((int) evt, iface);
        }

        public static void Post<T>(this Object obj, Events evt, T payload)
        {
            MessageKitLite<T>.Post((int) evt, payload);

            #if DEBUG_MESSAGEKIT_CALLS
            if (Application.isEditor)
                Debug.LogFormat("[MK] Posting event: {0} with payload {1}", evt, payload.ToString());
            #endif
        }

        public static void AddObserver<T, U>(this IMessageReceiver<T, U> iface, params Events[] events)
        {
            foreach (Events evt in events)
                MessageKitLite<T, U>.AddObserver((int) evt, iface);
        }

        public static void RemoveObserver<T, U>(this IMessageReceiver<T, U> iface, params Events[] events)
        {
            foreach (Events evt in events)
                MessageKitLite<T, U>.RemoveObserver((int) evt, iface);
        }

        public static void Post<T, U>(this Object obj, Events evt, T payload1, U payload2)
        {
            MessageKitLite<T, U>.Post((int) evt, payload1, payload2);
            #if DEBUG_MESSAGEKIT_CALLS
            if (Application.isEditor)
                Debug.LogFormat("[MK] Posting event: {0} with payloads {1}, {2}", evt, payload1.ToString(), payload2.ToString());
            #endif
        }
    }

    public static class ErrorDialogExtensions
    {
        public static void ShowNotificationDialog(this Object obj, string message, Action onDismissal = null)
        {
            obj.HideLoadingScreen();
            ShowNotificationDialog(obj, Application.productName, message, onDismissal);
        }

        public static void ShowNotificationDialog(Object obj, string title, string message, Action onDismissal = null)
        {
            obj.HideLoadingScreen();
            ShowErrorDialog(obj, title, message, onDismissal);
        }

        public static void ShowErrorDialog(this Object obj, string message, Action onDismissal = null)
        {
            obj.HideLoadingScreen();
            ShowErrorDialog(obj, Application.productName, message, onDismissal);
        }

        public static void ShowErrorDialog(this Object obj, string title, string message, Action onDismissal = null)
        {
            obj.HideLoadingScreen();
            /*obj.Post(Events.ShowErrorDialog, new ErrorDialog.Data()
            {
                Title = title,
                Message = message,
                OnCancelButton = onDismissal
            });*/
        }
    }

    public static class YesNoDialogExtensions
    {
        public static void ShowYesNoDialog(this Object obj, string message, Action onYesClicked,
            Action onNoClicked = null)
        {
            ShowYesNoDialog(obj, Application.productName, message, onYesClicked, onNoClicked);
        }

        public static void ShowYesNoDialog(this Object obj, string title, string message, Action onYesClicked,
            Action onNoClicked = null)
        {
            //showDialog(obj, Events.ShowYesNoDialog, title, message, onYesClicked, onNoClicked);
        }

        public static void ShowOkCancelDialog(this Object obj, string message, Action onOkClicked,
            Action onCancelClicked = null)
        {
            ShowOkCancelDialog(obj, Application.productName, message, onOkClicked, onCancelClicked);
        }

        public static void ShowOkCancelDialog(this Object obj, string title, string message, Action onOkClicked,
            Action onCancelClicked = null)
        {
            //showDialog(obj, Events.ShowOkCancelDialog, title, message, onOkClicked, onCancelClicked);
        }

        private static void showDialog(Object obj, Events evt, string title, string message, Action onOkClicked,
            Action onCancelClicked)
        {
            /*obj.Post(evt, new AlertDialog.Data()
            {
                Title = title,
                Message = message,
                OnOkClicked = onOkClicked,
                OnCancelButton = onCancelClicked
            });*/
        }
    }

    public static class LoadingDialogExtensions
    {

        public static void ShowLoadingScreen(this Object obj)
        {
            //obj.PostMessage(Events.ShowLoadingScreen);
        }

        public static void ShowLoadingScreen(this Object obj, string message)
        {
            //obj.Post(Events.ShowLoadingScreen, message);
        }

        public static void HideLoadingScreen(this Object obj)
        {
            //obj.PostMessage(Events.HideLoadingScreen);
        }

    }
}
 