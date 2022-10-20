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
            AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Clear Rive " + GameManager.Instance.currentIndexInCluster);
            Debug.LogError("here 4");

            if (TestLevelsSystemManagerSaveData.instance.CompletedCount + 1 == GameManager.Instance.currentCluster.clusterLevels.Length)
            {
                SoundManager.Instance.PlaySound(Sounds.RiveRelease);
            }
            else
            {
                SoundManager.Instance.PlaySound(Sounds.RiveRootRelease);
            }

            AnimationManager.instance.hasPlayedRelaseSound = true;
        }
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ActivateBackToHudButton()
    {
        UIManager.Instance.backToHubButton.interactable = true;
    }
}
