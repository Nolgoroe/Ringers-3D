using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsDen : MonoBehaviour
{
    public Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ResetPlacedDataObjects()
    {
        anim.SetBool("Release Now", false);
        AnimationManager.instance.isPlacingDenItem = false; // this is here to diable the option of moving out of screen until animation is done!
    }
}
