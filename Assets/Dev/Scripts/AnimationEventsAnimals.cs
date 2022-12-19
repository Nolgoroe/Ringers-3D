using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void ShakeAnimalStatue() //this is when the particles hit the level "object" - the name shake animal statue is bad
    {
        if (GameManager.Instance.currentLevel.isAnimalLevel)
        {
            if (AnimalsManager.Instance.statueToSwap.GetComponent<CameraShake>())
            {
                AnimalsManager.Instance.statueToSwap.GetComponent<CameraShake>().ShakeOnce();
            }

            if (GameManager.Instance.currentIndexInCluster == GameManager.Instance.currentCluster.clusterLevels.Length - 1)
            {
                if (AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>())
                {
                    AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>().renderer.materials[0].EnableKeyword("_EMISSION");
                }
            }
        }
        else if (GameManager.Instance.currentLevel.isTimerLevel)
        {
            //CallAfterAnimalAnimation();
        }
        else
        {
            AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Clear Rive " + GameManager.Instance.currentIndexInCluster);
            Debug.LogError("here 4");

            if (TestLevelsSystemManagerSaveData.instance.CompletedCount + 1 == GameManager.Instance.currentCluster.clusterLevels.Length)
            {
                //SoundManager.Instance.PlaySound(Sounds.RiveRelease);
            }
            else
            {
                //SoundManager.Instance.PlaySound(Sounds.RiveRootRelease);
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
        //if (TestLevelsSystemManager.instance.isGiveChest(GameManager.Instance.currentIndexInCluster + 1))
        //{
        //    UIManager.Instance.nextLevelFromWinScreen.interactable = false;
        //    UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
        //}
        //else
        //{
        //    UIManager.Instance.nextLevelFromWinScreen.interactable = true;
        //    UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(true);
        //}

        LeanTween.value(UIManager.Instance.backToHubButton.gameObject, 0f, 1, AnimationManager.instance.fadeInTimeButtons).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
        {
            Image image = UIManager.Instance.backToHubButton.GetComponent<Image>();
            Color newColor = image.color;
            newColor.a = val;
            image.color = newColor;
        });

        UIManager.Instance.backToHubButton.gameObject.SetActive(true);
        UIManager.Instance.backToHubButton.interactable = true;
    }

    public void PlaySound(AudioClip clip)
    {
        Debug.LogError("TESTING");
        SoundManager.Instance.PlaySound(clip);
    }

    public void ResetChestDataAndGiveLoot()
    {
        LootManager.Instance.UnpackChestLoot();
    }
}
