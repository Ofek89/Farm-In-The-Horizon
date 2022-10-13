// using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "so_AnimationType", menuName = "Scriptable Objects/Animation/Animation Type")]
//describes each animation that can be swapped, with all the information we need
public class SO_AnimationType : ScriptableObject
{
    public AnimationClip animationClip;         //the animationClip that will be used as an Override animation instead of the original animatio clip
    public AnimationName animationName;         //selected from an Enum
    public CharacterPartAnimator characterPart; //selected from an Enum
    public PartVariantColour partVariantColour; //selected from an Enum
    public PartVariantType partVariantType;     //selected from an Enum
}
