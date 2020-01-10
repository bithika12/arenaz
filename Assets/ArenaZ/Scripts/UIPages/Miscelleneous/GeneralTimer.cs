using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralTimer
{
    private MonoBehaviour _monoBehaviour;
    private float _time;
    private float _remainingTime;
    private float _slowTimeFactor;
    private Action _onTimerComplete;
    private IEnumerator _coroutine;
    private bool _isTimerPaused;

    //public uint RemainingTime { get => Convert.ToUInt32(_remainingTime); set => _remainingTime = value; }
    public float RemainingTime { get => _remainingTime; set => _remainingTime = value; }
    public float SlowTimeFactor { get => _slowTimeFactor; set => _slowTimeFactor = value; }
    public bool IsTimerPaused { get => _isTimerPaused; set => _isTimerPaused = value; }

    public GeneralTimer(MonoBehaviour mono)
    {
        _monoBehaviour = mono;
    }
    public void StartTimer(float time, Action onTimerComplete)
    {
        IsTimerPaused = false;
        _time = time;
        SlowTimeFactor = 1;
        _onTimerComplete = onTimerComplete;
        if (_coroutine != null)
            _monoBehaviour.StopCoroutine(_coroutine);
        _coroutine = CountDown();
        _monoBehaviour.StartCoroutine(_coroutine);
    }

    public void StopTimer()
    {
        if (_coroutine != null)
        {
            _monoBehaviour.StopCoroutine(_coroutine);
        }
        _coroutine = null;
        _onTimerComplete = null;
    }

    IEnumerator CountDown()
    {
        while (_time > 0)
        {
            if(!IsTimerPaused)
            {
                _time -= Time.deltaTime / SlowTimeFactor;
                _remainingTime = _time;
                yield return null;
            }
            else
                yield return null;                  
        }

        if (_onTimerComplete != null)
            _onTimerComplete?.Invoke();
        StopTimer();
    }
}
