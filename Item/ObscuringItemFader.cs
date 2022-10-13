using System.Collections;
using UnityEngine;

//this script needs to be attached to each of gameobject we want to fade while interacting with the player
//there is another script called TriggerObscuringItemFader attached to the player, that will trigger this script on the relevent 
//gameobject that we want to fade
// * the game object this script is attached, must have collider2d with IsTrigger option attached, so we can trigger it from the 
//TriggerObscuringItemFader script attached to the player
[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float currentAlpha = spriteRenderer.color.a; //capture current alpha of the sr
        float distance = currentAlpha - Settings.targetAlpha; //calculate the difference between current alpha and the targetAlpha we declared in the settings file

        //while loop that changed the alpha untill it reached our targetAlpha value from the settings file
        while(currentAlpha - Settings.targetAlpha > 0.01f)
        {
            currentAlpha = currentAlpha - distance / Settings.fadeOutSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha); //sets the current alpha to the new calcualted alpha
            yield return null; //cause the game to return here without a delay untill we reache the targetAlpha
        }

        //we end the loop at we reached the targetAlpha, and we set the sr alpha to the targetAlpha
        spriteRenderer.color = new Color(1f, 1f, 1f, Settings.targetAlpha);
    }

    //same as FadeOutRoutine, just with reverse values
    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = 1f - currentAlpha;

        while (1f - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + distance / Settings.fadeInSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

}
