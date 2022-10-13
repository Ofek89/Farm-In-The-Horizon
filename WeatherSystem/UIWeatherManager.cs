using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIWeatherManager : MonoBehaviour
{
    [SerializeField] private Image[] weatherIconPrefabs = null;

    [Header(" == Weather UI Icons from 0 - 4: Sunny, Cloudy, Rain, Wind, Snow == ")]
    [SerializeField] private Sprite[] weatherIconSprites = null;
    //[SerializeField] private Text currentTick = null;
    [SerializeField] private Text currentWeatherText = null;
    //[SerializeField] private Text currentTimeText = null;


    private void OnEnable()
    {
        WeatherManager.OnWeatherChange += WeatherChanged; //subscribe to OnWeatherChange event
        TickManager.OnTick += Tick;
    }

    private void OnDisable()
    {
        WeatherManager.OnWeatherChange -= WeatherChanged;
        TickManager.OnTick -= Tick;
    }

    private void Update()
    {
        //currentTimeText.text = $"For: {TickManager.CurrentGameTime.ToString("F1")}s";
    }

    // private void WeatherChanged(Weather currentWeather, Queue<Weather> weatherQueue) {
    //     //the current weather
    //     weatherIconPrefabs[0].sprite = Resources.Load<Sprite>($"Sprites/WeatherSprites/weather-sprite-{currentWeather}");
    //     currentWeatherText.text = currentTimeText.ToString().ToUpper();

    //     int index = 1;
    //     foreach (var item in weatherQueue) {
    //         weatherIconPrefabs[index].sprite = Resources.Load<Sprite>($"Sprites/WeatherSprites/weather-sprite-{item}");
    //         index++;
    //     }
    // }

    private void WeatherChanged(Weather currentWeather, Queue<Weather> weatherQueue)
    {
        //the current weather
        weatherIconPrefabs[0].sprite = weatherIconSprites[(int)currentWeather]; //sets the most left icon to the currentWeather

        //Debug.Log("currentWeather: " + currentWeather);
        //Debug.Log("currentWeather index: " + (int)currentWeather);
        //Debug.Log("currentWeather sprite name: " + weatherIconSprites[(int)currentWeather]);

        currentWeatherText.text = "Currently: " + currentWeather.ToString();

        //sets the rest of the icons to the next weather upcoming in the queue
        int index = 1;
        foreach (var item in weatherQueue)
        {
            if (index < 5)
            {
            weatherIconPrefabs[index].sprite = weatherIconSprites[(int)item];
            index++;
            }
        }
    }

    private void Tick()
    {
        //currentTick.text = $"Tick: {TickManager.currentTick}";
    }

}
