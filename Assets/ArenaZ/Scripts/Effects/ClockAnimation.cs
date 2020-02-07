using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaZ
{
    public class ClockAnimation : MonoBehaviour
    {
        [SerializeField] private AlarmClock alarmClock;

        public void OnEnd()
        {
            alarmClock.Hide();
        }
    }
}