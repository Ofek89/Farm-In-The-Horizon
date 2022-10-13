using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Transform cropParentTransform; //the parent to which we attach any crops we create in the scene
    [Header("Reference to the crop list SO asset containing all the different crop details")]
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;

    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary; //key is square coordinates, and value is square properties
    
    [Header("Reference to all the scene SO assets containing gridProperties")]
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null; //populate through the inspector
    
    [Tooltip("Drag here all the tiles which will be used to show dugged ground. Total of 16 tiles")]
    [SerializeField] private Tile[] dugGround = null; //stores the dug ground tiles which will be used when we dug the ground

    [Tooltip("Drag here all the tiles which will be used to show watered ground. Total of 16 tiles")]
    [SerializeField] private Tile[] wateredGround = null; //stores the watered ground tiles which will be used when we water the ground

    private bool isFirstTimeSceneLoaded = true;

    private string _iSaveableUniqueID; //ISaveable interface req field
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave; //ISaveable interface req field
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

        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    private void ClearDisplayGroundDecorations()
    {
        // Remove ground decorations, clears all the tiles that are placed on the tilemap
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    //destroy all crops in the scene
    private void ClearDisplayAllPlantedCrops()
    {
        //Remove all crops in scene
        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();
        foreach (Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }

    private void ClearDisplayGridPropertyDetails() //this methods is called when scene is being restored or day is being advanced 
    {
        ClearDisplayGroundDecorations();
        ClearDisplayAllPlantedCrops();
    }

    //Dug Tiles Methods
    #region Dug Tiles
    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Dug
        if (gridPropertyDetails.daysSinceDug > -1) //default value is -1 so if its greater than -1 means it was dugged
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile that we need to place, based on surrounding four tiles and whether if they are dugged or not

        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        // Set 4 tiles if dug surrounding current tile - up, down, left, right now that this central tile has been dug

        GridPropertyDetails adjacentGridPropertyDetails;

        //get tile above dugged tile (top grid square)
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        //check this grid square isnt null and it is dugged
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            //change tile accordingly
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        //get tile below dugged tile (bottom grid square)
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        //get tile on the left of the dugged tile (left grid square)
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        //get tile on the right of the dugged tile (right grid square)
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    private Tile SetDugTile(int xGrid, int yGrid)
    {
        //Get whether surrounding tiles (up,down,left, and right) are dug or not

        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);

        //checks for 16 possible combinations, to adjust the tiles based on four surrounding tiles around the dugged tile
        #region Set appropriate tile based on whether surrounding tiles are dug or not

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }

        return null;

        #endregion Set appropriate tile based on whether surrounding tiles are dug or not
    }

    //get gridPropertyDetails for specfic square, and check if it was dugged or not
    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion Dug Tiles 

    //Water Tiles Methods
    #region Water Tiles
    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // Watered
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile based on surrounding watered tiles

        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);

        // Set 4 tiles if watered surrounding current tile - up, down, left, right now that this central tile has been watered

        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), wateredTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), wateredTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile3 = SetWateredTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), wateredTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile4 = SetWateredTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), wateredTile4);
        }
    }

    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        // Get whether surrounding tiles (up,down,left, and right) are watered or not

        bool upWatered = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downWatered = IsGridSquareWatered(xGrid, yGrid - 1);
        bool leftWatered = IsGridSquareWatered(xGrid - 1, yGrid);
        bool rightWatered = IsGridSquareWatered(xGrid + 1, yGrid);

        #region Set appropriate tile based on whether surrounding tiles are watered or not

        if (!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[0];
        }
        else if (!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[1];
        }
        else if (!upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[2];
        }
        else if (!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[3];
        }
        else if (!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[4];
        }
        else if (upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[5];
        }
        else if (upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[6];
        }
        else if (upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[7];
        }
        else if (upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[8];
        }
        else if (upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[9];
        }
        else if (upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[10];
        }
        else if (upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[11];
        }
        else if (upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[12];
        }
        else if (!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[13];
        }
        else if (!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[14];
        }
        else if (!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[15];
        }

        return null;

        #endregion Set appropriate tile based on whether surrounding tiles are watered or not
    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion Water Tiles 

    //we will call this method on ISaveableRestoreScene method.
    //we call the Display Dug/Watered/PlantedCrop methods here
    private void DisplayGridPropertyDetails()
    {
        // Loop through all grid items
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);

            DisplayWateredGround(gridPropertyDetails);

            DisplayPlantedCrop(gridPropertyDetails);
        }
    }

    // Display planted crop for gridpropertyDetails
    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemCode > -1) // -1 is unplanted default
        {
            // get crop details from the SO cropList
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            if (cropDetails != null) //check if we have details in the SO_cropDetails list for that seed to prevent errors
            {
                // prefab to use
                GameObject cropPrefab;
                // instantiate crop prefab at grid location
                int growthStages = cropDetails.growthDays.Length;

                int currentGrowthStage = 0;

                for (int i = growthStages - 1; i >= 0; i--)
                {
                    //check for each growth stage if we growth days req passed or not
                    if (gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                    {
                        currentGrowthStage = i;
                        break;
                    }
                }

                cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

                Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

                Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);

                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
                cropInstance.transform.SetParent(cropParentTransform);
                cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            } else
            {
                Debug.Log("Seed doesnt exist on the SO_cropDetails");
            }
        }
    }

    private void AfterSceneLoaded()
    {
        if(GameObject.FindGameObjectWithTag(Tags.CropsParentTransform) != null)
        {
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            cropParentTransform = null;
        }

        // Get Grid
        grid = GameObject.FindObjectOfType<Grid>();

        // Get tilemaps
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    // This initialises the grid property dictionary with the values from the SO_GridProperties assets and stores the values for each scene in
    // GameObjectSave sceneData
    private void InitialiseGridProperties()
    {
        // Loop through all gridproperties assets in the array
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            // Create dictionary of grid property details
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            // Populate grid property dictionary - Iterate through all the grid properties in the so gridproperties list 
            //which exists on the SO asset, and updated each time we edit tilemap on the editor, via TilemapGridProperties script
            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;

                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);

                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                //gridBoolProperty is of type enum we declared for bool types
                switch (gridProperty.gridBoolProperty) 
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canFish:
                        gridPropertyDetails.canFish = gridProperty.gridBoolValue;
                        break;
                    
                    default:
                        break;
                }

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            // Create scene save for this gameobject
            SceneSave sceneSave = new SceneSave();

            // Add grid property dictionary to scene save data
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            // If starting scene set the gridPropertyDictionary member variable to the current iteration
            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            // Add bool dictionary and set first time scene loaded to true
            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", true);

            // Add scene save to game object scene data
            GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }

    // Get the grid property details for the tile at (gridX,gridY).  If no grid property details exist null is returned and can assume that all grid property details values are null or false
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    // Returns the gridPropertyDetails at the gridlocation for the supplied dictionary, or null if no properties exist at that location.
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        // Check if grid property details exist for coordinate and retrieve
        if (!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            // if not found
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    // Set the grid property details to gridPropertyDetails for the tile at (gridX,gridY) for current scene
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    // Set the grid property details to gridPropertyDetails for the tile at (gridX,gridY) for the gridpropertyDictionary.
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        // Set value
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    //  Returns the Crop object component at the gridX, gridY position or null if no crop was found
    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        //gets the grid world position
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        //returns an array of any colliders that overlap that worldPosition
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

        // Loop through colliders to get crop game object 
        Crop crop = null;

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
        }

        return crop;
    }

    // Returns Crop Details for the provided seedItemCode
    public CropDetails GetCropDetails(int seedItemCode)
    {
        return so_CropDetailsList.GetCropDetails(seedItemCode);
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Remove sceneSave for scene
        GameObjectSave.sceneData.Remove(sceneName);

        // Create sceneSave for scene
        SceneSave sceneSave = new SceneSave();

        // create & add dict grid property details dictionary
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        // create & add bool dictionary for first time scene loaded
        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

        // Add scene save to game object scene data
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    //store the current scene data
    public GameObjectSave ISaveableSave()
    {
        // Store current scene data
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    //when we have the gameSave object passed as a parameter, we try to allocate the GameObjectSave variable for this game object
    //by its unique ID, so we look into the game object data dictionary 
    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave; //if found game object with that unique ID, save it into GameObjectSave

            // Restore data for current scene
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Get sceneSave for scene - it exists since we created it in initialise
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            // get grid property details dictionary - it exists since we created it in initialise
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

            // Check if scene has been loaded first time or not, to know if should instantiate crop prefabs or not
            // get dictionary of bools - it exists since we created it in initialise
            if (sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoaded", out bool storedIsFirstTimeSceneLoaded))
            {
                isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
            }
            // Instantiate any crop prefabs initially present in the scene
            if (isFirstTimeSceneLoaded)
                EventHandler.CallInstantiateCropPrefabsEvent();


            // If grid properties exist
            if (gridPropertyDictionary.Count > 0)
            {
                // grid property details found for the current scene destroy existing ground decoration
                ClearDisplayGridPropertyDetails();

                // Instantiate grid property details for current scene
                DisplayGridPropertyDetails();
            }

            // Update first time scene loaded bool, means we already loaded it now and it would be first time its loaded again
            if (isFirstTimeSceneLoaded == true)
            {
                isFirstTimeSceneLoaded = false;
            }
        }
    }

    // in here we change any of the grid properties detail across all of our scenes that needs to response when advance day happens in the game
    //this method is subscribed to the AdvanceGameDayEvent
    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // Clear Display All Grid Property Details
        ClearDisplayGridPropertyDetails();

        // Loop through all scenes - by looping through all gridproperties in the array
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            // Get gridpropertydetails dictionary for scene
            if (GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    //looping backwards because at foreach in this case we would get an error
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        //"ElementAt(i)" usuable by using Linq namespace, which retrieve an element from the dictionary at an element with location i
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        #region Update all grid properties to reflect the advance in the day

                        //If a crop is planted
                        if (gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }

                        // If ground is watered, then clear water
                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        // Set gridpropertydetails with the updated new gridPropertyDetails values
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);

                        #endregion Update all grid properties to reflect the advance in the day
                    }
                }
            }
        }

        // Display grid property details to reflect changed values
        DisplayGridPropertyDetails();
    }

}
