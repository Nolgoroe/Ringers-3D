using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsAnimals : MonoBehaviour
{
    public void CallAfterAnimalAnimation()
    {
       AnimationManager.instance.endAnimToWinScreen = StartCoroutine(AnimationManager.instance.AfterAnimalAnimation());
    }

    public void ShakeAnimalStatue()
    {
        if (AnimalsManager.Instance.statueToSwap.GetComponent<CameraShake>())
        {
            AnimalsManager.Instance.statueToSwap.GetComponent<CameraShake>().ShakeOnce();
        }

        if (AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>())
        {
            AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>().renderer.materials[0].EnableKeyword("_EMISSION");
        }
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
