using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterSkin : MonoBehaviour
{

    // Configures
    public Color SkinColor;
    public SpriteRenderer Head, Face, Body;
    public SpriteRenderer TopLeftLeg, BottomLeftLeg, TopRightLeg, BottomRightLeg;
    public SpriteRenderer Foot, Foot2;
    public SpriteRenderer Hat;
    public SpriteRenderer Cap;
    public SpriteRenderer Shirt;
    public SpriteRenderer RightTopSlavePart, RightBottomSlavePart, LeftTopSlavePart, LeftBottomSlavePart;
    public SpriteRenderer GunHolster, GunHolsterTop, Gun;
    public SpriteRenderer RightPantsBottomPart, RightPantsTopPart, LeftPantsBottomPart, LeftPantsTopPart;
    public SpriteRenderer LeftShoe, RightShoe;


    //--------------PRIVATES-------------    
    private Object[] SpriteSheet;
    private Sprite Sprite2;


    void Start()
    {
        // Change Sprite (Multiple) Code ------ //
        //SpriteSheet = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/CharacterMaleModel.png");
        //foreach (Object spt in SpriteSheet)
        //{
        //    if (spt.name == "Face")
        //    {
        //        Face.sprite = (Sprite)spt;
        //    }
        //}

        // Change Single Sprite
        //Sprite2 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/axeLogo.png");
        //Face.sprite = Sprite2;

        // Skin Color
        Face.color = SkinColor;
        Foot.color = SkinColor;
        Body.color = SkinColor;
        //====F****n Hands======//
        transform.Find("Body/RightArm/RightForearm/RightHand").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger1_1").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger1_1/RightHandFinger1_2").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger1_1/RightHandFinger1_2/RightHandFinger1_3").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger2_1").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger2_1/RightHandFinger2_2").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger2_1/RightHandFinger2_2/RightHandFinger2_3").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger3_1").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger3_1/RightHandFinger3_2").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger3_1/RightHandFinger3_2/RightHandFinger3_3").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger4_1").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger4_1/RightHandFinger4_2").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandFinger4_1/RightHandFinger4_2/RightHandFinger4_3").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/RightArm/RightForearm/RightHand/RightHandThumb").GetComponent<SpriteRenderer>().color = SkinColor;

        transform.Find("Body/LeftArm/LeftForearm/LeftPalm").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger1_1").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger1_1/LeftPalmFinger1_2").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger1_1/LeftPalmFinger1_2/LeftPalmFinger1_3").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger2_1").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger2_1/LeftPalmFinger2_2").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger2_1/LeftPalmFinger2_2/LeftPalmFinger2_3").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger3_1").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger3_1/LeftPalmFinger3_2").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger3_1/LeftPalmFinger3_2/LeftPalmFinger3_3").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger4_1").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger4_1/LeftPalmFinger4_2").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFinger4_1/LeftPalmFinger4_2/LeftPalmFinger4_3").GetComponent<SpriteRenderer>().color = SkinColor;
        transform.Find("Body/LeftArm/LeftForearm/LeftPalm/LeftPalmFingerThumb").GetComponent<SpriteRenderer>().color = SkinColor;
    }

    void Update()
    {

    }
}
