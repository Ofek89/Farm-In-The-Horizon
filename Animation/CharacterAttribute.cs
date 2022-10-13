[System.Serializable]
//describes the animation we want to swap to, with character part, color variant and type
public struct CharacterAttribute
{
    public CharacterPartAnimator characterPart; //arms,legs //selected from an Enum
    public PartVariantColour partVariantColour; //none, red //selected from an Enum
    public PartVariantType partVariantType;     //carry     //selected from an Enum

    public CharacterAttribute(CharacterPartAnimator characterPart, PartVariantColour partVariantColour, PartVariantType partVariantType)
    {
        this.characterPart = characterPart;
        this.partVariantColour = partVariantColour;
        this.partVariantType = partVariantType;
    }
}
