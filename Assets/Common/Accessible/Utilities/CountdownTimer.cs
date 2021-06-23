using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace RedApple.Utils
{
    [DisallowMultipleComponent]
    public class CountdownTimer : MonoBehaviour
    {
        private enum ETimerState
        {
            Idle = 0,
            CountdownActive,
        }

        private ETimerState timerState = ETimerState.Idle;
        private Action<string> onTimerUpdate;
        private Action onCountdownFinished;

        private int hours = 0;
        private int minutes = 0;
        private int seconds = 0;
        private float timeLeft = 0.0f;

        private bool autoDestroy = false;
        private bool isWalletDraw = false;

        public void StartCountdown(float a_CountdownTime, bool a_AutoDestroy, Action<string> a_OnTimerUpdate, Action a_OnCountdownFinished, bool _isWalletDraw = false)
        {
            autoDestroy = a_AutoDestroy;
            onTimerUpdate = a_OnTimerUpdate;
            onCountdownFinished = a_OnCountdownFinished;
            timerState = ETimerState.CountdownActive;
            timeLeft = a_CountdownTime;
            isWalletDraw = _isWalletDraw;
        }

        public void UpdateTime(float a_CountdownTime)
        {
            if (timerState == ETimerState.CountdownActive)
                timeLeft = a_CountdownTime;
        }

        public void StopCountdown()
        {
            timerState = ETimerState.Idle;
            onTimerUpdate?.Invoke("00:00");

            onTimerUpdate = null;
            onCountdownFinished = null;

            autoDestroyObj();
        }

        private void Update()
        {
            countdown();
        }

        private void countdown()
        {
            if (timerState == ETimerState.CountdownActive)
            {
                timeLeft -= Time.deltaTime;

                if(!isWalletDraw)
                {
                    minutes = Mathf.FloorToInt(timeLeft / 60.0f);
                    seconds = Mathf.FloorToInt(timeLeft % 60.0f);
                    if (timeLeft > 0.0f)
                        onTimerUpdate?.Invoke(minutes.ToString("00") + ":" + seconds.ToString("00"));
                }
                else
                {
                    hours = TimeSpan.FromSeconds(timeLeft).Hours;
                    minutes = TimeSpan.FromSeconds(timeLeft).Minutes;
                    seconds = TimeSpan.FromSeconds(timeLeft).Seconds;
                    if (timeLeft > 0.0f)
                        onTimerUpdate?.Invoke(hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00"));
                }


                if (timeLeft <= 0.0f)
                    countdownFinished();
            }
        }

        private void countdownFinished()
        {
            timerState = ETimerState.Idle;
            onTimerUpdate?.Invoke("00:00");
            onCountdownFinished?.Invoke();

            autoDestroyObj();
        }

        private void autoDestroyObj()
        {
            if (autoDestroy)
            {
                GameObject t_Go = GetComponent<GameObject>();
                if (t_Go != null)
                    Destroy(this.gameObject);
                else
                    Destroy(this);
            }
        }
    }
}
