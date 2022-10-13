using UnityEngine;
using Cinemachine;

//this script is attached to the PlayerFollowVirtualCamera gameobject
public class SwitchConfineBoundingShape : MonoBehaviour
{
    void Start()
    {
        //SwitchBoundingShape();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundingShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundingShape;
    }

    //this is called from afterSceneLoadEvent event, to make sure we call this method after the scene has been loaded
    // Switch the collider that cinemachine uses to define the edges of the screen
    private void SwitchBoundingShape()
    {
        //  Get the polygon collider on the 'boundsconfiner' gameobject which is used by Cinemachine to prevent the camera going beyond the screen edges
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        // since the confiner bounds have changed need to call this to clear the cache;

        cinemachineConfiner.InvalidatePathCache();
    }

}
