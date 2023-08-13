using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event Action OnTimerEnded;
    public event Action OnTimerChanged;
    public float Time { get; private set; } = 0.0f;
    public float Target { get; private set; } = 5.0f;
    public float Speed { get; private set; } = 1.0f;
    public string Name { get; private set; } = "Timer";

    public void SetTimer(float timerTarget, float timeSpeed = 1.0f)
    {
        Speed = timeSpeed;
        Target = timerTarget;
    }

    public IEnumerator StartTimer()
    {
        Time = 0.0f;
        float timeLapse = 1.0f * Speed;

        while (Time < Target)
        {
            yield return new WaitForSeconds(timeLapse);
            Time += timeLapse;

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