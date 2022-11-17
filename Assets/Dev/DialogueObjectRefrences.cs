using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueObjectRefrences : MonoBehaviour
{
    [Header("Dialogue Entry")]

    public SpriteRenderer portraitRenderer;
    public SpriteRenderer textBGRender;
    public SpriteRenderer nameBGRenderer;

    public TMP_Text textObject; 
    public TMP_Text nameText;

    public Transform textBoxParent;

    [Header("Image Entry")]
    public SpriteRenderer spriteImageRenderer;


    [Header("General")]
    public GameObject arrowObject;
}
