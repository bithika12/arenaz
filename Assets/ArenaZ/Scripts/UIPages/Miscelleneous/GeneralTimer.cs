using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralTimer
{
    private MonoBehaviour _monoBehaviour;
    private float _time;
    private float _remainingTime;
    private const float _slowTimeFactor = 1;
    private Action _onTimerComplete;
    private IEnumerator _coroutine;
    private bool _isTimerPaused = false;

    private float vanillaTimerRef= 0.0f;

    //public uint RemainingTime { get => Convert.ToUInt32(_remainingTime); set => _remainingTime = value; }
    public float RemainingTime { get => _remainingTime; set => _remainingTime = value; }
    public float SlowTimeFactor { get => _slowTimeFactor; set { } }
    public bool IsTimerPaused { get => _isTimerPaused; set => _isTimerPaused = value; }



    public GeneralTimer(MonoBehaviour mono, float time)
    {
        _monoBehaviour = mono;
        vanillaTimerRef = time;
    }
    public void StartTimer(Action onTimerComplete)
    {
        ResetTimer();
        _onTimerComplete = onTimerComplete;
        if (_coroutine != null)
            _monoBehaviour.StopCoroutine(_coroutine);
        _coroutine = CountDown();
        _monoBehaviour.StartCoroutine(_coroutine);
    }

    public void StopTimer()
    {
        //_monoBehaviour.StopAllCoroutines();
        if (_coroutine != null)
        {
            _monoBehaviour.StopCoroutine(_coroutine);
        }
        _coroutine = null;
        _onTimerComplete = null;
    }

    public void ResetTimer()
    {
        _time = vanillaTimerRef;
        _remainingTime = vanillaTimerRef;
    }

    IEnumerator CountDown()
    {
        while (_time > 0)
        {
            if(!IsTimerPaused)
            {
                _time -= Time.deltaTime / _slowTimeFactor;
                _remainingTime = _time;
                yield return null;
            }
            else
                yield return null;                  
        }
        _onTimerComplete?.Invoke();
    }
}
