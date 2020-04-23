using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ
{
    public class ScoreElement : MonoBehaviour
    {
        public void FadeInOutAnimation(bool a_AutoDestroy = false, Action a_OnComplete = null, Action a_OnCompleteInternal = null, float a_FadeInOutTransitionTime = 0.2f, float a_HoldIntervalTime = 1.0f)
        {
            Image t_Image = GetComponent<Image>();
            if (t_Image != null)
            {
                Sequence t_Sequence = DOTween.Sequence();
                t_Sequence.Append(t_Image.DOFade(1.0f, a_FadeInOutTransitionTime).SetEase(Ease.InSine));
                t_Sequence.AppendInterval(a_HoldIntervalTime);
                t_Sequence.Append(t_Image.DOFade(0.0f, a_FadeInOutTransitionTime).SetEase(Ease.InSine));
                t_Sequence.AppendCallback(() =>
                {
                    a_OnComplete?.Invoke();
                    a_OnCompleteInternal?.Invoke();
                    if (a_AutoDestroy)
                        Destroy(gameObject);
                });
                t_Sequence.Play();
            }
        }

        public void MoveFadeInOutAnimation(Transform a_Parent, Action a_OnStepComplete = null, Action a_OnComplete = null, Action a_OnCompleteInternal = null, float a_FadeInTransitionTime = 0.2f, float a_HoldIntervalTime = 1.0f, float a_FadeOutTransitionTime = 1.5f)
        {
            Image t_Image = GetComponent<Image>();
            RectTransform t_RectTransform = GetComponent<RectTransform>();
            if (t_Image != null && t_RectTransform != null)
            {
                Sequence t_Sequence = DOTween.Sequence();
                t_Sequence.Append(t_Image.DOFade(1.0f, a_FadeInTransitionTime).SetEase(Ease.InSine));
                t_Sequence.AppendInterval(a_HoldIntervalTime);
                t_Sequence.AppendCallback(() =>
                {
                    a_OnStepComplete?.Invoke();
                    moveFadeOutAnimation(t_Image, t_RectTransform, a_Parent, a_OnComplete, a_OnCompleteInternal, a_FadeOutTransitionTime);
                });
                t_Sequence.Play();
            }
        }

        private void moveFadeOutAnimation(Image a_Image, RectTransform a_RectTransform, Transform a_Parent, Action a_OnComplete, Action a_OnCompleteInternal, float a_FadeOutTransitionTime)
        {
            transform.SetParent(a_Parent);
            a_Image.DOFade(0.0f, a_FadeOutTransitionTime / 2.0f).OnComplete(() =>
            {
                a_OnComplete?.Invoke();
                a_OnCompleteInternal?.Invoke();
                Destroy(gameObject);
            });
            a_RectTransform.DOScale(0.0f, a_FadeOutTransitionTime);
            a_RectTransform.DOAnchorPos3DZ(0.0f, a_FadeOutTransitionTime * 2.5f);
            a_RectTransform.DOAnchorPos(Vector2.zero, a_FadeOutTransitionTime);
        }
    }
}