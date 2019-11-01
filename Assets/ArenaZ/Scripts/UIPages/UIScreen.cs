using System;
using UnityEngine;
using System.Collections;


namespace ArenaZ.Screens
{
    /// <summary>
    /// In this class add default activity of each screens (like animation, show, hide etc)
    /// </summary>
    public class UIScreen : MonoBehaviour
    {
        private readonly string showTrigger = "Straight";
        private readonly string hideTrigger = "Reverse";

        protected Animator animator;
        protected virtual void Awake()
        {
            if (GetComponent<Animator>())
            {
                animator = GetComponent<Animator>();
            }
        }

        protected void Show()
        {
            gameObject.SetActive(true);
        }

        protected void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowGameObjWithAnim()
        {
            Debug.Log($"Showing Animation {name}");
            Show();
            if (animator != null)
                animator.SetTrigger(showTrigger);            
        }

        public void HideGameObjWithAnim()
        {
            if (animator == null)
            {
                Hide();
                return;
            }
            animator.SetTrigger(hideTrigger);
            StartCoroutine(HideAfterAnimation());
        }

        IEnumerator HideAfterAnimation()
        {
            AnimationClip myReverseAnimClip = animator.runtimeAnimatorController.animationClips[0];
            yield return new WaitForSeconds(myReverseAnimClip.length);
            Hide();
        }

    }
}
