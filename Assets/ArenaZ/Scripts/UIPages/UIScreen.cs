using System;
using UnityEngine;
using System.Collections;


namespace ArenaZ.ScreenManagement
{
    /// <summary>
    /// In this class add default activity of each screens (like animation, show, hide etc)
    /// </summary>
    public class UIScreen : MonoBehaviour
    {
        protected Animator animator;
        protected virtual void Awake()
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

        public void ShowGameObjWithAnim(string triggerName)
        {
            Show();
            animator.SetTrigger(triggerName);            
        }

        public void HideGameObjWithAnim(string triggerName)
        {
            animator.SetTrigger(triggerName);
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
