using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArenaZ
{
    public class Stopwatch : MonoBehaviour
    {
        private enum EStopwatch
        {
            Idle = 0,
            Running,
        }

        private EStopwatch stopwatchState = EStopwatch.Idle;
        private float startTime = 0.0f;
        private float currentTime = 0.0f;
        private List<StopwatchData> stopwatchDatas = new List<StopwatchData>();

        public void Initialize(params StopwatchData[] a_Args)
        {
            stopwatchDatas = a_Args.ToList();
            startTime = Time.time;
            stopwatchState = EStopwatch.Running;
        }

        public void Stop()
        {
            stopwatchState = EStopwatch.Idle;
            stopwatchDatas.Clear();
            startTime = 0.0f;
        }

        private void Update()
        {
            Timer();
        }

        private void Timer()
        {
            if (stopwatchState == EStopwatch.Running && stopwatchDatas.Any())
            {
                currentTime = Time.time;
                float diff = startTime - currentTime;
                foreach (StopwatchData item in stopwatchDatas.Reverse<StopwatchData>())
                {
                    if (diff >= item.Mark)
                    {
                        item.Callback?.Invoke();
                        stopwatchDatas.Remove(item);
                    }
                }
            }
        }
    }

    public class StopwatchData
    {
        public StopwatchData(float a_Mark, Action a_Callback)
        {
            Mark = a_Mark;
            Callback = a_Callback;
        }

        public float Mark { get; set; } = 0.0f;
        public Action Callback { get; set; } = null;
    }
}