using UnityEngine;

//this scipt is attached to the player, and will activate the fade out effect near objects that have the script: "ObscuringItemFader" attached to them
public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the gameobject we have collided with, and then get all the Obscuring Item Fader components on it and its children - and then trigger the fade out

        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringItemFader.Length > 0)
        {
            for(int i=0; i<obscuringItemFader.Length; i++)
            {
                obscuringItemFader[i].FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Get the gameobject we have collided with, and then get all the Obscuring Item Fader components on it and its children - and then trigger the fade in

        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringItemFader.Length > 0)
        {
            for (int i = 0; i < obscuringItemFader.Length; i++)
            {
                obscuringItemFader[i].FadeIn();
            }
        }

    }
}
