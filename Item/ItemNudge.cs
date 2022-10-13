using System.Collections;
using UnityEngine;

//adds nudge effect to items, this triggers from inside the Item script, which there we declare what kind of items types (from the item enum)
//will be the item types that the nudge effect will trigger on
public class ItemNudge : MonoBehaviour
{
    private WaitForSeconds pause; 
    private bool isAnimating = false;

    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //reponding only to the collider of the child gameobject under the Player, which has that tag
        if (collision.gameObject.tag == Tags.ItemNudgeCollider)
        {

            if (isAnimating == false)
            {
                if (gameObject.transform.position.x < collision.gameObject.transform.position.x)
                {
                    StartCoroutine(RotateAntiClock());
                }
                else
                {
                    StartCoroutine(RotateClock());
                }

                    AudioManager.Instance.PlaySound(SoundName.effectRustle);
            }


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //reponding only to the collider of the child gameobject under the Player, which has that tag
        if (collision.gameObject.tag == Tags.ItemNudgeCollider)
        {
            if (isAnimating == false)
            {
                if (gameObject.transform.position.x > collision.gameObject.transform.position.x)
                {
                    StartCoroutine(RotateAntiClock());
                }
                else
                {
                    StartCoroutine(RotateClock());
                }

                AudioManager.Instance.PlaySound(SoundName.effectRustle);

            }
        }
    }

    private IEnumerator RotateAntiClock()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

        yield return pause;

        isAnimating = false;
    }

    private IEnumerator RotateClock()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

        yield return pause;

        isAnimating = false;
    }
}
