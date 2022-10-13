using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//Movement Delegate
//reference type that will hold references to methods and the methods will have the same signature, i.e the parameters list that the delegate has defined
public delegate void MovementDelegate(float inputX, float inputY, 
    bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect, 
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight);

public static class EventHandler
{
    //Player mouse clicks on map Input Events (called from within the player script, depanding on players input clicks with specific item types)
    #region
    //Drop selected item event
    public static event Action DropSelectedItemEvent;

    //when we call this method, it invokes "DropSelectedItemEvent" which means all the methods subscribed to it, will be executed
    //in this case - we call this method from Player script when player clicks on the map to drop an item,
    //which then triggers the methods that is subscribed to it, from the UIInventorySlot that drops the item from the UI slot to the map
    public static void CallDropSelectedItemEvent() 
    {
        DropSelectedItemEvent?.Invoke();
    }


    //ADDED
    //Cast fishing Event
    public static event Action CastFishingEvent;
    public static void CallCastFishingEvent()
    {
        CastFishingEvent?.Invoke();
    }
    #endregion

    //Inventory Updated Event
    #region 
    //Inventory Updated Event. InventoryLocation: defined by the InventoryLocation enum, represents the kinds of inventories | 
    //List<InventoryItem>: list of inventory items, which holds itemCode and quantity of each item 
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    //the method that is being called by the publishers when the InventoryUpdatedEvent event is triggered
    //we trigger this event from the InventoryManager script, when we add new item. the UIInventoryBar script also runs a methods when this event is shot, to update the inventory 
    //(with InventoryUpdated() method)
    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (InventoryUpdatedEvent != null) //we make sure there are subscribers to the event
            InventoryUpdatedEvent(inventoryLocation, inventoryList);
    }

    public static event Action RemoveSelectedItemFromInventoryEvent;

    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        RemoveSelectedItemFromInventoryEvent?.Invoke(); 
    }
    #endregion

    //Movement Event (for the movement event we used a delegate because we use alot of parameters, so we cant use the Action saved method event)
    #region
    public static event MovementDelegate MovementEvent;

    //Movement Event Call For Publishers
    public static void CallMovementEvent(float xInput, float yInput, 
        bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect, 
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
        bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
        bool idleUp, bool idleDown, bool idleLeft, bool idleRight) 
    {
        if(MovementEvent!= null)
            MovementEvent (xInput, yInput, 
                isWalking, isRunning, isIdle, isCarrying, toolEffect, 
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown, 
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                idleUp, idleDown, idleLeft, idleRight);
    }
    #endregion

    // TIME EVENTS
    #region 
    // Advance game minute
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        //Debug.Log("test1");
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    // Advance game hour
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    // Advance game day
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    // Advance game season
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    // Advance game year
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }
    #endregion

    // Scene Load Events - in the order they happen
    #region
    // Before Scene Unload Fade Out Event
    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if (BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    // Before Scene Unload Event
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    // After Scene Loaded Event
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    // After Scene Load Fade In Event
    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }
    #endregion

    // Harvest Action Effect Event
    #region
    public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;

    public static void CallHarvestActionEffectEvent(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        HarvestActionEffectEvent?.Invoke(effectPosition, harvestActionEffect);
    }

    //an example of calling the effect (from the players script, for reaping):
    //EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);
    #endregion

    //Instantiate crop prefabs
    #region

    public static event Action InstantiateCropPrefabsEvent;

    public static void CallInstantiateCropPrefabsEvent()
    {
        InstantiateCropPrefabsEvent?.Invoke();
    }
    #endregion
}
