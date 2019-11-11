using System.Text;
using UnityEngine;

namespace RedApple.Utils
{
    public static class Extensions
    {
        public static byte[] GetBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static void Show(this CanvasGroup canvasGroup)
        {
            canvasGroup.toggle(true);
        }

        public static void Hide(this CanvasGroup canvasGroup)
        {
            canvasGroup.toggle(false);
        }

        private static void toggle(this CanvasGroup canvasGroup, bool show)
        {
            canvasGroup.alpha = show ? 1 : 0;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
        }
    }
}