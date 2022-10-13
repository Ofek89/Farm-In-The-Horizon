using UnityEngine;

[System.Serializable] //makes it saveable to a file
public class GridCoordinate
{
    public int x;
    public int y;

    public GridCoordinate(int p1, int p2)
    {
        x = p1;
        y = p2;
    }
    
    // "public static explicit operator" makes explicit type conversion - gets a type parameter and returns that parameter as a type we define
    // enables us to cast a GridCoordinate type variable to a Vector2/Vector2Int/Vector3/Vector3Int types variables 
   
    public static explicit operator Vector2(GridCoordinate gridCoordinate)
    {
        return new Vector2((float)gridCoordinate.x, (float)gridCoordinate.y);
    }

    public static explicit operator Vector2Int(GridCoordinate gridCoordinate)
    {
        return new Vector2Int(gridCoordinate.x, gridCoordinate.y);
    }

    public static explicit operator Vector3(GridCoordinate gridCoordinate)
    {
        return new Vector3((float)gridCoordinate.x, (float)gridCoordinate.y, 0f);
    }

    public static explicit operator Vector3Int(GridCoordinate gridCoordinate)
    {
        return new Vector3Int(gridCoordinate.x, gridCoordinate.y, 0);
    }
}
