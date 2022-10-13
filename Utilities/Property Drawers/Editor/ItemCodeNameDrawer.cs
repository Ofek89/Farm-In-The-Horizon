using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//this script draws th itemName in the inspector, when we click on an item gameobject, it uses ItemCodeNameAttribute which is another script with an empty class
// * for it to work, we need to hold a reference in the Item script as follows: "[GetItemName]" so the Item class will derive the use of that property
[CustomPropertyDrawer(typeof(ItemCodeNameAttribute))] //specify the type of the custom property
public class ItemCodeNameDrawer : PropertyDrawer //inherit from PropertyDrawer which is a saved unity script
{
    //saved unity method to declare the property height that will be drawn in the inspector
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Change the returned property height to be double to cater for the additional item code name that we will draw, in addition to the item code 
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    //saved unity method, responsible for drawing the actuall property on the inspector
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that prefab override logic works on the entire property.

        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer) //test if the property type is of type integer
        {

            EditorGUI.BeginChangeCheck(); // Start of check for changed values

            // Draw item code
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);

            // Draw item name
            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2), "Item Name", GetItemName(property.intValue));

            // If item code value has changed, then set value to new value
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }


        }

        EditorGUI.EndProperty();
    }

    private string GetItemName(int itemCode)
    {
        SO_ItemList so_itemList; //reference to our SO_itemList scriptable object item that holds all the items information

        //the path for our SO_itemList scriptable object item in our project 
        so_itemList = AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Items/so_ItemList.asset",typeof(SO_ItemList)) as SO_ItemList;

        List<ItemDetails> itemDetailsList = so_itemList.itemDetails; //list of itemDetailsList for each item in the inventory

        ItemDetails itemDetail = itemDetailsList.Find(x => x.itemCode == itemCode); //checks if we have an item on our item list, that has the itemCode which we recieved as a pramater

        if (itemDetail != null)
        {
            return itemDetail.itemName;
        }
        else
        {
            return "itemCode doesn't exist";
        }
    }

}