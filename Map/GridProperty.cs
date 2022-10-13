[System.Serializable] //makes it saveable to a file
//this class will be used to create a SO asset value to store all boolean values in a list for a tilemap
public class GridProperty
{
    public GridCoordinate gridCoordinate; //the type we created which contains x,y grid coordinate
    public GridBoolProperty gridBoolProperty; //an enum, contains different bool types (diggable, can DropItem, etc)
    public bool gridBoolValue = false; //when we paint a tilemap, this will be true, or false if we didnt paint that grid

    //constructor that sets the parameter values equals to the fields variables
    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty, bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;
    }
}
