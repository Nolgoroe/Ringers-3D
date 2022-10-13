using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsAnimals : MonoBehaviour
{
    public void CallAfterAnimalAnimation()
    {
        if (!AnimationManager.instance.hasSkippedToAfterAnimalAnim)
        {
            //StartCoroutine(AnimationManager.instance.AfterAnimalAnimation());
            AnimationManager.instance.CallAfterAnimalAnimation();
        }
    }

    public void ShakeAnimalStatue()
    {
        if (GameManager.Instance.currentLevel.isAnimalLevel)
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
        else
        {
            AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Clear Rive " + TestLevelsSystemManagerSaveData.instance.CompletedCount);
            SoundManager.Instance.PlaySound(Sounds.RiveRelease);
        }
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
