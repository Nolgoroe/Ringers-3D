using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using UnityEngine.Playables;

public class DialogueObjectRefrences : MonoBehaviour
{
    [Header("Dialogue Entry")]

    //public SpriteRenderer portraitRenderer;
    public SpriteRenderer textBGRender;
    public SpriteRenderer nameBGRenderer;

    public TMP_Text textObject; 
    public TMP_Text nameText;

    public Transform textBoxParent;

    public GameObject spineAsset;

    [Header("Spine Zone")]
    public SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState spineAnimationState;
    public SkeletonRenderer skeletonRenderer;
    public PlayableDirector director;
    public SpineBlinkPlayer blinkAnimationRef;

    [Header("Image Entry")]
    public SpriteRenderer spriteImageRenderer;


    [Header("General")]
    public GameObject arrowObject;

    private void Awake()
    {
        if(spineAsset == null)
        {
            Debug.LogError("No spine asset");
            return;
        }

        spineAsset.TryGetComponent<SkeletonAnimation>(out skeletonAnimation);
        if (skeletonAnimation == null)
        {
            Debug.LogError("Problem here 1!"); 
            return;
        }

        spineAnimationState = skeletonAnimation.AnimationState;
        if (spineAnimationState == null)
        {
            Debug.LogError("Problem here 2!");
            return;
        }

        spineAsset.TryGetComponent<SkeletonRenderer>(out skeletonRenderer);
        if (skeletonRenderer == null)
        {
            Debug.LogError("Problem here 3!"); 
            return;
        }

        spineAsset.TryGetComponent<PlayableDirector>(out director);
        if (director == null)
        {
            Debug.LogError("Problem here 4!");
            return;
        }

        spineAsset.TryGetComponent<SpineBlinkPlayer>(out blinkAnimationRef);
        if (blinkAnimationRef == null)
        {
            Debug.LogError("Problem here 5!");
            return;
        }

    }

    public void CallStartBlinkingSpine()
    {
        StartCoroutine(blinkAnimationRef.StartBlinking());
    }

    public void RefreshAnimationData()
    {
        // every time we change the data set of the spine we must reset the refs of the 
        // skeletonAnimation and the spineAnimationState... it's just "how it works"

        spineAsset.TryGetComponent<SkeletonAnimation>(out skeletonAnimation);
        if (skeletonAnimation == null)
        {
            Debug.LogError("Problem here 1!");
            return;
        }

        spineAnimationState = skeletonAnimation.AnimationState;
        if (spineAnimationState == null)
        {
            Debug.LogError("Problem here 2!");
            return;
        }
    }
}
