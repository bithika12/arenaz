using DevCommons.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public void Show()
        {
            if (animationState == EAnimation.Idle)
            {
                Debug.Log("Show Clock");
                animationState = EAnimation.Playing;
                clock.SetActive(true);

                AudioPlayer.Play(new AudioPlayerData() { audioClip = DataHandler.Instance.GetAudioClipData(EAudioClip.Timeout).Clip, oneShot = true });

                Animator t_Animator = clock.GetComponent<Animator>();
                t_Animator.Play("Jerk");

                //JerkAnimation t_JerkAnimation = t_Animator.GetBehaviour<JerkAnimation>();
                //t_JerkAnimation.SetCallback(Hide);
            }
        }

        public void Hide()
        {
            Debug.Log("Hide Clock");
            animationState = EAnimation.Idle;
            clock.GetComponent<Animator>().Play("Idle");
            clock.SetActive(false);
        }
    }
}