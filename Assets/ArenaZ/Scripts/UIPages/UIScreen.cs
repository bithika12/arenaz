using System;
using UnityEngine;
using System.Collections;
using TMPro;


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
        protected TextMeshProUGUI txtMeshPro;
        protected virtual void Awake()
        {
            if (GetComponent<Animator>())
            {
                animator = GetComponent<Animator>();
            }
            if(GetComponent<TextMeshProUGUI>())
            {
                txtMeshPro = GetComponent<TextMeshProUGUI>();
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
           // Debug.Log($"Showing Animation {name}");
            Show();
            if (animator != null)
            {
                animator.SetTrigger(showTrigger);
            }
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

        private IEnumerator HideAfterAnimation()
        {
            AnimationClip myReverseAnimClip = animator.runtimeAnimatorController.animationClips[0];
            yield return new WaitForSeconds(myReverseAnimClip.length);
            Hide();
        }

        public IEnumerator ShowAndHidePopUpText(string message,float duration)
        {
            txtMeshPro.text = message;
            ShowGameObjWithAnim();
            yield return new WaitForSeconds(duration);
            HideGameObjWithAnim();
        }

    }
}
