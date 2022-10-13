using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>, ISaveable
{
    [Header("Starting Time Values")]
    [SerializeField] private int gameYear = 1;
    [SerializeField] private Season gameSeason = Season.Spring; //Season enum representing different seasons
    [SerializeField] private int gameDay = 1;
    [SerializeField] private int gameHour = 6;
    [SerializeField] private int gameMinute = 30;
    [SerializeField] private int gameSecond = 0;
    [SerializeField] private string gameDayOfWeek = "Mon";

    private bool gameClockPaused = false;

    [Header("Just For Tracking Purposes")]
    [SerializeField] private float gameTick = 0f; //updates with time.delaTime everyframe - Makes the game time advance

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    //pause game clock when scene fade out
    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    //pause game clock hen scene load
    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }


    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
        }
        
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime; //everyframe we adding Time.deltaTime 

        //we compare the gameTick to the secondsperGameSecond and if it reaches its value we know a second has passed in the game
        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond; //reseting gameTick value     
            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++;

        if (gameSecond > 59) //increase min
        {
            gameSecond = 0;
            gameMinute++;


            if (gameMinute > 59) //increase hour
            {
                gameMinute = 0;
                gameHour++;

                if (gameHour > 23) //increase day
                {
                    gameHour = 0;
                    gameDay++;

                    if (gameDay > 30) //increase season
                    {
                        gameDay = 1;

                        int gs = (int)gameSeason; //cast enum to int and set it to local field
                        gs++;

                        gameSeason = (Season)gs; //cast the value enum to gameSeason enum

                        if (gs > 3) //if we passed gameSeason 4 we reset gameSeason, and advance year
                        {
                            gs = 0;
                            gameSeason = (Season)gs;

                            gameYear++;

                            if (gameYear > 9999)
                                gameYear = 1;


                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }
                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }
                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }
                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            
            // Debug.Log("Game Year: " + gameYear + "   Game Season: " + gameSeason + "   Game Day " + gameDay + "   Game Hour " + gameHour + 
            //     "   Game Minute: " + gameMinute);
        }

        // Call to advance game second event would go here if required
    }

    private string GetDayOfWeek()
    {
        int totalDays = (((int)gameSeason) * 30) + gameDay;
        int dayOfWeek = totalDays % 7;

        switch (dayOfWeek)
        {
            case 1:
                return "Mon";

            case 2:
                return "Tue";

            case 3:
                return "Wed";

            case 4:
                return "Thu";

            case 5:
                return "Fri";

            case 6:
                return "Sat";

            case 0:
                return "Sun";

            default:
                return "";
        }
    }

    // public TimeSpan GetGameTime()
    // {
    //     TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);

    //     return gameTime;
    // }


    //TODO:Remove
    /// Advance 1 game minute
    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }

    //TODO:Remove
    /// Advance 1 day
    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    //collects data for current game time and adds it to sceneSave
    public GameObjectSave ISaveableSave()
    {
        // Delete existing scene save if exists
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Create new scene save
        SceneSave sceneSave = new SceneSave();

        // Create new int dictionary
        sceneSave.intDictionary = new Dictionary<string, int>();

        // Create new string dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // Add values to the int dictionary
        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);

        // Add values to the string dictionary
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        // Add scene save to game object for persistent scene
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    //reverse of what we did in the above save method
    public void ISaveableLoad(GameSave gameSave)
    {
        // Get saved gameobject from gameSave data
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Get savedscene data for gameObject
            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // if int and string dictionaries are found
                if (sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    // populate saved int values
                    if (sceneSave.intDictionary.TryGetValue("gameYear", out int savedGameYear))
                        gameYear = savedGameYear;

                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay))
                        gameDay = savedGameDay;

                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int savedGameHour))
                        gameHour = savedGameHour;

                    if (sceneSave.intDictionary.TryGetValue("gameMinute", out int savedGameMinute))
                        gameMinute = savedGameMinute;

                    if (sceneSave.intDictionary.TryGetValue("gameSecond", out int savedGameSecond))
                        gameSecond = savedGameSecond;

                    // populate string saved values
                    if (sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                        gameDayOfWeek = savedGameDayOfWeek;

                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        if (Enum.TryParse<Season>(savedGameSeason, out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    // Zero gametick
                    gameTick = 0f;

                    // Trigger advance minute event
                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                    // Refresh game clock
                }
            }
        }
    }
    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required here since Time Manager is running on the persistent scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since Time Manager is running on the persistent scene
    }
}
