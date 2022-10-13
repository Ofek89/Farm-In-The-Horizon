public interface ISaveable
{
    string ISaveableUniqueID { get; set; } //unique ID for the gameobject we want to save. (GUID) declaring property and not a field because on Interface we cant declare a field

    GameObjectSave GameObjectSave { get; set; } //will store the save data for each game object we want to save

    void ISaveableRegister(); // Register to SaveLoadManager

    void ISaveableDeregister(); // Degister to SaveLoadManager

    GameObjectSave ISaveableSave(); //save/load functionality, returns GameObjectSave

    void ISaveableLoad(GameSave gameSave); //save/load functionality

    void ISaveableStoreScene(string sceneName); //method game objet will need to implement to store their scene data

    void ISaveableRestoreScene(string sceneName); //method game object will need to implement to restore their scene data

}
