using DevCommons.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ArenaZ
{
    public class AlarmClock : MonoBehaviour
    {
        public enum EAnimation
        {
            Idle = 0,
            Playing,
        }

        [SerializeField] private GameObject clock;
        private EAnimation animationState = EAnimation.Idle;

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.A))
        //        Show();
        //}

        public void Show()
        {
            if (animationState == EAnimation.Idle)
            {
                animationState = EAnimation.Playing;
                clock.SetActive(true);

                clock.transform.localScale = Vector3.zero;
                clock.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.InBounce);

                AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.Timeout).Clip, oneShot = true, volume = SettingData.SFXVolume });

                Animator t_Animator = clock.GetComponent<Animator>();
                t_Animator.Play("Jerk");

                //JerkAnimation t_JerkAnimation = t_Animator.GetBehaviour<JerkAnimation>();
                //t_JerkAnimation.SetCallback(Hide);
            }
        }

        public void Hide()
        {
            animationState = EAnimation.Idle;
            clock.GetComponent<Animator>().Play("Idle");
            clock.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBounce).OnComplete(() =>
            {
                clock.transform.localScale = Vector3.zero;
                clock.SetActive(false);
            });
        }
    }
}