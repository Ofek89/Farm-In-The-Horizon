using System.Collections.Generic;
using System.IO; //enabling reading and writign to files
using System.Runtime.Serialization.Formatters.Binary; //does the serialization in binary format
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{
    public GameSave gameSave;

    public List<ISaveable> iSaveableObjectList; //List of Interface ISaveable

    protected override void Awake()
    {
        base.Awake();

        iSaveableObjectList = new List<ISaveable>();
    }

    //this function is called when the use click the LOAD button on the PauseMenu -> Settings Tab
    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter(); //the object we use for serialize data

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat")) //check if we already have a save game exist
        {
            gameSave = new GameSave(); //create a new instance of type gameSave we declared above, which is a dictionary of gameObjectSave

            //opens the file string for reading (by using system.IO namespace)
            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);

            //Deserialize the binary format of the saved game file, changing it from binary file to a game object of type gameSave
            gameSave = (GameSave)bf.Deserialize(file);

            // loop through all ISaveable objects and apply save data
            for (int i = iSaveableObjectList.Count - 1; i > -1; i--)
            {
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                // else if iSaveableObject unique ID is not in the game object data then destroy object
                else
                {
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }

            file.Close();
        }

        UIManager.Instance.DisablePauseMenu();
    }

    //this function is called when the use click the SAVE button on the PauseMenu -> Settings Tab
    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        // loop through all ISaveable objects and generate save data
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);

        bf.Serialize(file, gameSave);

        file.Close();

        UIManager.Instance.DisablePauseMenu();
    }

    //Store and Restore methods:
    //we know each item we loop through will contain ISaveable/StoreScene/RestoreScene because they implemented in the ISaveable Interface

    public void StoreCurrentSceneData()
    {
        // loop through all ISaveable objects and trigger store scene data for each, at the current active scene
        //Items add themselved during their Awake() to the iSaveableObject list
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        // loop through all ISaveable objects and trigger restore scene data for each
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
