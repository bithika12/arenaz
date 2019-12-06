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

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowGameObjWithAnim()
        {
            Show();
            if (animator != null)
            {
                animator.SetTrigger(Constants.showTrigger);
            }
        }

        public void HideGameObjWithAnim()
        {
            if (animator == null)
            {
                Hide();
                return;
            }
            animator.SetTrigger(Constants.hideTrigger);
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

        public void EnableDisableComponent<T>(bool value)
        {
            UnityEngine.Behaviour component  = GetComponent<T>() as UnityEngine.Behaviour;
            component.enabled = value;
        }

    }
}
