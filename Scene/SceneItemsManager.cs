using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//this script will exist in the persistent scene, its job is to control the store and restore, save and load of items in the scene
//we call the methods from this scene from the SceneControlleManager
[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>, ISaveable
{
    private Transform parentItem; //the parent object that stores all the items as his childs in the scene
    [SerializeField] private GameObject itemPrefab = null; //reference to the general itemPrafab prefab

    //two properties required by the ISaveable Interface which we inherit from (we also declare methods below req by that Interface)
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    //finds the parentItem of all items in the scene after the scene has been loaded (so we know it exists in the scene)
    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    // Destroy items currently in the scene
    private void DestroySceneItems()
    {
        // Get all items in the scene
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        // Loop through all scene items and destroy them
        for (int i = itemsInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;

        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    //adds item to the list
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }
    
    //removes item from the list
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Restore data for current scene
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }


    //used when we move into a new scene
    public void ISaveableRestoreScene(string sceneName)
    {
        //looks for the sceneName list of items from the GameObjectSave dictionary which contains all itemsData list for each scene
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.listSceneItem != null)
            {
                // scene list items found - destroy existing items in scene
                DestroySceneItems();

                // now instantiate the list of scene items
                InstantiateSceneItems(sceneSave.listSceneItem);
            }
        }
    }


    public GameObjectSave ISaveableSave()
    {
        // Store current scene data
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }



    public void ISaveableStoreScene(string sceneName)
    {
        // Remove old scene save for gameObject if exists
        GameObjectSave.sceneData.Remove(sceneName);

        // Get all items in the scene
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();

        // Loop through all scene items
        foreach (Item item in itemsInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            // Add scene item to list (creates a "clone" list of all items in the scene from the itemInScene array)
            sceneItemList.Add(sceneItem);
        }

        // Create list scene items in scene save and set to scene items list 
        SceneSave sceneSave = new SceneSave(); 
        sceneSave.listSceneItem = sceneItemList;

        // Add scene save to gameobjectSave, which holds a dictionary of sceneName and list of sceneItem
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}
