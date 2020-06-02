using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ArenaZ
{
    public class ReconnectCountdown : MonoBehaviour
    {
        [SerializeField] private Text countdownText;
        private float timeLeft = 0.0f;
        private bool isCountingDown = false;
        private Action callback;

        public void StartCountdown(float a_TimeLeft = 5.0f, Action a_Callback = null)
        {
            timeLeft = a_TimeLeft;
            callback = a_Callback;
            isCountingDown = true;
        }

        public void StopCountdown()
        {
            callback = null;
            isCountingDown = false;
        }

        private void Update()
        {
            if (isCountingDown)
            {
                if (timeLeft > 0)
                {
                    timeLeft -= Time.deltaTime;
                }
                else
                {
                    callback?.Invoke();
                    StopCountdown();
                }

                int t_Time = (int)timeLeft;
                if (countdownText != null && t_Time >= 0)
                    countdownText.text = t_Time.ToString();
            }
        }
    }
}
