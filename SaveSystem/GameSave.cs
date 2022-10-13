using System.Collections.Generic;

[System.Serializable]
public class GameSave
{
    // string key = GUID gameobject ID, GameObjectSave value is the data type that our scene save is saved into
    public Dictionary<string, GameObjectSave> gameObjectData;

    public GameSave()
    {
        gameObjectData = new Dictionary<string, GameObjectSave>();
    }
}
