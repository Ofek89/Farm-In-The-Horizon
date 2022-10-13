using System.Collections.Generic;

//this stores all the game objects we wish to save for each scene
[System.Serializable]
public class SceneSave
{
    //* STORES ALL ITEMS IN A SCENE *
    public List<SceneItem> listSceneItem;

    //* STORES ALL GRID PROPERTIES IN A SCENE *
    //GridPropertyDetails - holds details for every square on our scene tilemap, the string(key) will be the square coordinates
    //we will have 1 dictionary per scene
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary; 
    public Dictionary<string, bool> boolDictionary;    // string key is an identifier name we choose for this list

    //* STORES GAME TIME *
    public Dictionary<string, int> intDictionary;


    // * STORES PLAYERS POSITION AND DIRECTION IN A SCENE *
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, Vector3Serializable> vector3Dictionary; //string key will represent players direction, and value Vector3 wil be players position

    // public List<SceneItem> listSceneItem;

    //* STORES INVENTORY *
    public Dictionary<string, int[]> intArrayDictionary; //stores inventory capacity
    public List<InventoryItem>[] listInvItemArray; //stores inventory items
}
