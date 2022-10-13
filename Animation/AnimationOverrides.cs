using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//this script is attached to a child game object of the player, called "character customiser"
public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null; //stores player gameobject
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null; //stores a list of animation types, which is made out of SO 

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    private void Start()
    {
        //INITIALISE TWO DICTIONARIES:

        // (1) Initialise animation type dictionary keyed by animation clip with new Override clips from the soAnimationTypeArray 
        //Key = SO with new animation to Override | Value = the actuall SO asset
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        // (2) Initialise animation type dictionary keyed by string
        //same as above, with key differece, here the key is a strong combined by all the SO information
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            //for each SO in the array, create a string combining all of the SO information and store it as a key for the 2nd dictionary
            string key = item.characterPart.ToString() + item.partVariantColour.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            //adds the string value to the key and the SO asset at the value
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }

    }

    //this method swaps the animation. we call this method from outside of this script to swap animations
    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributesList)
    {
        //Stopwatch s1 = Stopwatch.StartNew();

        // Loop through all character attributes and set the animation override controller for each
        foreach (CharacterAttribute characterAttribute in characterAttributesList)
        {
            Animator currentAnimator = null;

            //create a new empty list of dictionary containing keyValuePair of two animations
            //this will be used below, to store default animation (key), and an override animation clip (value)
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            //stores the characterPart name, from the characterAttributeList we recieved as a parameter
            string animatorSOAssetName = characterAttribute.characterPart.ToString(); //arms/legs etc

            //stores all animators on child gameobjects of the player
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();

            // Find the specific animator out of animators array that matches the scriptable object animator type 
            foreach (Animator animator in animatorsArray)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator; // store the currect animator type for each body part
                    break;
                }
            }

            //After we found the currentAnimator which we want to override above, we handle the override:
            //In short - takes default animation list (animationsList) from the Animator Controller, and create a KeyValueList (animsKeyValuePairList) to store their animations Override (swapAnimationClip)
            //then applies it to the aoc

            //create new AnimatorOverrideController to aoc var
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            
            //sets the default animationClips of the aoc, to animationsList variable
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips); 

            //for each animation clip in animationList, find a new overrideAnimation and store it inside 
            //this is a list of all the basic animations stored inside the AnimatorController, which we pass in to the OverrideController (aoc)
            foreach (AnimationClip animationClip in animationsList)
            {
                SO_AnimationType so_AnimationType;

                // find animation in dictionary we initialised above, by key, and return value (which is a SO_AnimationType)
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation) 
                {
                    //if we have an animation, we get its key which is a long string of the animation info
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString() + characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();

                    SO_AnimationType swapSO_AnimationType; //will be used to store an animation stored inside a SO_AnimationType
                    //check if the hey string with the info exist as a key on the other dictionary, and return the value associated with it, if it does
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;

                        //stores inside a list (which we declared above) animations pair, with default animation and its desired override animation
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }

            // Apply animation updates to animation override controller and then update animator with the new controller
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }

        // s1.Stop();
        // UnityEngine.Debug.Log("Time to apply character customisation : " + s1.Elapsed + "   elapsed seconds");
    }

}
