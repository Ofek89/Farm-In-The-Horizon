using System.Collections.Generic;
using UnityEngine;

//we create SO asset for each scene we have in the game
[CreateAssetMenu(fileName = "so_GridProperties", menuName = "Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    public SceneName sceneName;

    //maximum width and height of the tilemap
    [Header("Dragged from the bottom left square to the top right square, the total size of the grid map")]
    public int gridWidth;
    public int gridHeight;

    //origin of the tile map - bottom left square of the tile map
    [Header("X and Y values of the bottom left square in the TileMap Grid of the scene")]
    public int originX;
    public int originY;

    //for every scene, this asset wil contain all the boolean properties in a list that represents the values that are being painted on the tilemaps
    [Header("Keep this list collapsed to avoid editor slow down")]
    [SerializeField]
    public List<GridProperty> gridPropertyList; //main list which contains all the painted squares on the scene tilemap
}
