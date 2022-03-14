using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsAnimals : MonoBehaviour
{
    public void CallAfterAnimalAnimation()
    {
       AnimationManager.instance.endAnimToWinScreen = StartCoroutine(AnimationManager.instance.AfterAnimalAnimation());
    }
}
