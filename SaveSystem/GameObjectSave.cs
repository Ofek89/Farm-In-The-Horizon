using System.Collections.Generic;

[System.Serializable]
public class GameObjectSave
{
    // string key = scene name. the amount of entries in this dictionary is equal to the amount of scenes we have in the game
    public Dictionary<string, SceneSave> sceneData;

    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData = sceneData;
    }
}
