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

    public void SetTimer(float targetTime, float timeSpeed = 1.0f)
    {
        TargetTime = targetTime;
        TimeSpeed = timeSpeed;
    }

    public IEnumerator StartTimer()
    {
        CurrentTime = 0.0f;
        float timeLapse = 1.0f * TimeSpeed;

        while (CurrentTime < TargetTime)
        {
            yield return new WaitForSeconds(timeLapse);
            CurrentTime += timeLapse;

            OnTimeChangeHandler();
        }

        OnTimerEndHandler();
    }

    private void OnTimerEndHandler()
    {
        OnTimerEnded?.Invoke();
    }

    private void OnTimeChangeHandler()
    {
        OnTimerChanged?.Invoke();
    }
}