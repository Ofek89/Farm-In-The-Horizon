using UnityEditor;
using UnityEngine; 
using UnityEngine.Tilemaps;

//will run both in editor and on play mode, combined with the if check (*) we make it run only during the editor
//we attach this script to each tilemap boolean tilemap

//this updates the gridPropertyList exists on the SO asset, with the current painted tiles for each bool tilemap type on each scene
[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR //this makes sure this code will only get compile inside the unity editor (to prevent errors when create a build of the game)
    private Tilemap tilemap; //will contain the tilemap that this script is attached to
    
    //we will drag the SO_GridProperties asset to this property from the inspector
    [Header("Reference to the SO_GridProperties asset for that scene")]
    [SerializeField] private SO_GridProperties gridProperties = null; 
    
    // we use this to indicate what is the property we are capturing with this component on this particular tilemap (out of the enum)
    [Header("Reference to the currect bool type of the relevant tilemap")]
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;

    //OnEnable we clear the SO list (because it means we plan to start editing the tilemap)
    private void OnEnable()
    {
        // (*) Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>(); //gets tileMap component

            if (gridProperties != null) //makes sure we have a reference to the SO 
            {
                gridProperties.gridPropertyList.Clear(); //clear the SO list
            }
        }
    }

    //OnDisable we populate SO list (because it means we finished editing the tilemap)
    //we paint tilemaps, and when we done painting, we disable the tilemapProperties parent gameobject, then this method executes
    //looks through the tilemaps we have pianted, can capture everything in there and save it to the SO
    private void OnDisable()
    {      // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridProperties != null)
            {
                // This is required to ensure that the updated gridproperties gameobject gets saved when the game is saved - otherwise they are not saved.
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    //
    private void UpdateGridProperties()
    {
        // Compress timemap bounds - saved unity method that goes through tilemap and compresses the bounds to just the tiles that are painted on
        //(it prevent the extension of tilemap bounds that can happen if we paint outside of tilemaps bounds)
        tilemap.CompressBounds();

        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            if (gridProperties != null) //check to see if we have SO passed in for the grid properties
            {
                //stores the bounds of the all property tilemap
                Vector3Int startCell = tilemap.cellBounds.min; //bottom left of the tile map
                Vector3Int endCell = tilemap.cellBounds.max; //top right of the tile map

                // loops through every square inside the bounds, 
                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0)); //gets a tile at the tilemap at the above position

                        //if the tile isnt null it means there is a tile painted, so we want to add a value to our property list on our SO 
                        if (tile != null)
                        {
                            //adds that tile to the gridpropertylist, with three parameters; tile coordinates, booltype form the enum, true to indicates its painted tile
                            gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {      // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("DISABLE PROPERTY TILEMAPS"); //just remind us to disable it when we done painting
        }
    }
#endif
}