using UnityEngine;

//this should be attached to each game object we want its data saved (like items)
//we want this to run in the editor. most MonoBehhaviour only runs when we press play button
//[ExecuteAlways] = will run in edit mode aswell
//creates a unique string key that will identify the game object that we want to save
[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        // Only populate in the editor
        //because this script will always runs even in the editor, we do a test to continue 
        //only if we are on the editor
        if (!Application.IsPlaying(gameObject))
        {
            // Ensure the object has a guaranteed unique id, by checking first it has an empty string
            if (_gUID == "")
            {
                //Assign GUID which is a unique string key
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
