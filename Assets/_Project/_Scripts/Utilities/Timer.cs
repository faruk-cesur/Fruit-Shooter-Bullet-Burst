using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event Action OnTimerEnded;
    public event Action OnTimerChanged;
    [field: SerializeField] public float CurrentTime { get; private set; } = 0.0f;
    [field: SerializeField] public float TargetTime { get; private set; } = 5.0f;
    [field: SerializeField] public float TimeSpeed { get; private set; } = 1.0f;
    [field: SerializeField] public string TimerName { get; private set; } = "Timer";
    [field: SerializeField] public bool IsTimerEnded { get; private set; } = false;
    private Coroutine _timerCoroutine;

    public void SetTimer(float targetTime, float timeSpeed = 1.0f)
    {
        TargetTime = targetTime;
        TimeSpeed = timeSpeed;
    }

    public void StartTimer()
    {
        if (_timerCoroutine is not null)
        {
            StopCoroutine(_timerCoroutine);
        }

        _timerCoroutine = StartCoroutine(StartTimerCoroutine());
    }

    public void ResetCurrentTime()
    {
        CurrentTime = 0.0f;
        IsTimerEnded = false;
    }

    private IEnumerator StartTimerCoroutine()
    {
        ResetCurrentTime();
        float timeLapse = 1.0f * TimeSpeed;

        while (CurrentTime < TargetTime)
        {
            yield return new WaitForSeconds(timeLapse);
            CurrentTime += timeLapse;

            OnTimerChangeHandler();
        }

        OnTimerEndHandler();
        IsTimerEnded = true;
    }

    private void OnTimerEndHandler()
    {
        OnTimerEnded?.Invoke();
    }

    private void OnTimerChangeHandler()
    {
        OnTimerChanged?.Invoke();
    }
}