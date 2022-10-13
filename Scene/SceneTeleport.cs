using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoto = SceneName.Scene1_Farm;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();
    private GameObject normaPlayerCollider; //this will contain the child game object of the player which has the normal collider attached to it

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == Tags.NormalPlayerCollider) //if we collide with the child game object of the Player with the normal collider attached (which has "PlayerNormalCollider" tag)
        {
            normaPlayerCollider = collision.gameObject; //store the child gameobject with the normal collider

            Player player = collision.GetComponentInParent<Player>(); //grab the player script, from the parent gameobject 

            if (player != null) //if we found the player script, move the player to the new scene
            {
                //  Calculate players new position
                float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;

                float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

                float zPosition = 0f;

                // Teleport to new scene
                SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));
            }
        }
    }
}
