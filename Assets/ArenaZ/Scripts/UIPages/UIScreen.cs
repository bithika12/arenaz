using System;
using UnityEngine;
using UnityEngine.Events;


namespace ArenaZ.Screen
{
    /// <summary>
    /// In this class add default activity of each screens (like animation, show, hide etc)
    /// </summary>
    public class UIScreen : MonoBehaviour
    {
        Animator animator;
        private void Awake()
        {
            if (GetComponent<Animator>())
            {
                animator = GetComponent<Animator>();
            }
        }

        public void Show()
        {
            if (gameObject.activeSelf)
            {
                return;
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void PlayAnimation(string triggerName)
        {
            animator.SetTrigger(triggerName);
        }

        public void StopAnimation(string triggerName)
        {
            animator.ResetTrigger(triggerName);
        }

    }
}
