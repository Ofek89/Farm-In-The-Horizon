using ClipperLib;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [Header(" === Weather Management === ")]
    [SerializeField] private int tickBetweenWeather = 10;
    [SerializeField] private int weatherQueueSize = 5; //for UI to forecast weather types
    private int currentWeatherTick = 0;
    [SerializeField] private Weather currentWeather = Weather.Sunny; //default start to Sunny
    public Weather CurrentWeather => currentWeather; //Weather property that returns currentWeather
    private Queue<Weather> weatherQueue;

    [Header(" === Weather VFX === ")]
    [SerializeField] ParticleSystem rainParticles = null;
    [SerializeField] ParticleSystem snowParticles = null;
    [SerializeField] ParticleSystem windParticles = null;
    [SerializeField] ParticleSystem secondWindParticles = null;
    [SerializeField] ParticleSystem cloudParticles = null;
    [SerializeField] ParticleSystem sunParticles = null;

    [Header(" === Debug Options === ")]
    public bool forceRain = false;

    //we pass in Weather and the Weather queue 
    public static Action <Weather, Queue<Weather>> OnWeatherChange;

    private void Start()
    {
        rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        windParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        secondWindParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        sunParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        FillWeatherQueue();
        ChangeWeather();
    }

    private void OnEnable()
    {
        TickManager.OnTick += Tick;
    }

    private void OnDisable()
    {
        TickManager.OnTick -= Tick;
    }

    private void Tick()
    {
        currentWeatherTick++;
        if (currentWeatherTick >= tickBetweenWeather)
        {
            currentWeatherTick = 0;
            ChangeWeather();
        }
    }

    private void FillWeatherQueue()
    {
        weatherQueue = new Queue<Weather>();

        for (int i = 0; i < weatherQueueSize; i++)
        {
            Weather tempWeather = GetRandomWeather();
            weatherQueue.Enqueue(tempWeather);
            //Debug.Log($"Weather is {tempWeather} at index {i}");
        }
    }

    private Weather GetRandomWeather()
    {
        int randomWeather = 0;

        randomWeather = UnityEngine.Random.Range(0, (int)Weather.WEATHER_MAX + 1);

        return (Weather)randomWeather;
    }

    void ChangeWeather()
    {
        currentWeather = weatherQueue.Dequeue();
        weatherQueue.Enqueue(GetRandomWeather());

        //added value

        OnWeatherChange?.Invoke(currentWeather, weatherQueue);

        switch (CurrentWeather)
        {
            case Weather.Sunny:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                windParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                secondWindParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                sunParticles.Play();
                break;
            case Weather.Cloudy:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                windParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                secondWindParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                cloudParticles.Play();
                sunParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                break;
            case Weather.Rainy:
                rainParticles.Play();
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                windParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                secondWindParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                sunParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                break;
            case Weather.Wind:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                windParticles.Play();
                secondWindParticles.Play();
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                sunParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                break;
            case Weather.Snowy:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                snowParticles.Play();
                windParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                secondWindParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                sunParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                break;
            default:
                break;
        }
    }
}

public enum Weather
{
    Sunny = 0,
    Cloudy = 1,
    Rainy = 2,
    Wind = 3,
    Snowy = 4,
    WEATHER_MAX = Snowy
}
