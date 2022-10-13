using System;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    [SerializeField] private float tickFrequency = 1f;
    public static int currentTick = 0;
    private float lastTickTime = 0;
    [SerializeField] private static float currentGameTime;
    public static float CurrentGameTime => currentGameTime;
    public static Action OnTick;

    private void Update()
    {
        Tick();
    }

    private void Tick()
    {
        currentGameTime += Time.deltaTime;
        if (currentGameTime >= lastTickTime + tickFrequency)
        {
            lastTickTime = currentGameTime;
            OnTick?.Invoke();
            currentTick++;
        }
    }
}