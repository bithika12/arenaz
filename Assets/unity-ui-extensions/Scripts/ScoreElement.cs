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
        public void PlayMoveTowardsAnimation(GameObject a_Target, ScoreGraphic.EMoveTowards a_MoveTowards, Action<GameObject> a_Callback)
        {
            transform.SetParent(a_Target.transform);
            transform.GetComponent<Image>().DOFade(0.0f, 1.5f).OnComplete(() => a_Callback?.Invoke(gameObject));
            transform.GetComponent<RectTransform>().DOScale(0.0f, 1.25f);
            transform.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 1.0f);
        }

        public void PlayCloseAnimation(Action<GameObject> a_Callback)
        {
            transform.GetComponent<Image>().DOFade(0.0f, 0.5f).OnComplete(() => a_Callback?.Invoke(gameObject));
        }
    }
}