using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumImageAnimalData : MonoBehaviour
{
    public AnimalsInGame imageAnimalEnum;
    public bool isUnlocked;

    public GameObject hiddenAnimal, RevealedAnimal;

    public Image[] imagesToReveal;

    public void TransferToRevealed()
    {
        //we want both of them to always be displayed together when animation is started
        hiddenAnimal.SetActive(true);
        RevealedAnimal.SetActive(true);

        foreach (Image image in imagesToReveal)
        {
            LeanTween.value(image.gameObject, 0.75f, 1.5f, AnimalAlbumManager.Instance.revealAnimalSpeed).setOnComplete(AfterReveal).setOnUpdate((float val) =>
            {
                Material mat = image.material;

                mat.SetFloat("_DissolveSprite", val);
            });
        }
    }

    public void AfterReveal()
    {
        hiddenAnimal.SetActive(false);
    }

    public void TransferToRevealedImmediate()
    {
        //we want both of them to always be displayed together when animation is started
        hiddenAnimal.SetActive(true);
        RevealedAnimal.SetActive(true);


        foreach (Image image in imagesToReveal)
        {
            Material mat = image.material;

            mat.SetFloat("_DissolveSprite", 1.5f);
        }
    }

    public void ResetTransferDataImmediate()
    {
        hiddenAnimal.SetActive(true);
        RevealedAnimal.SetActive(false);


        foreach (Image image in imagesToReveal)
        {
            Material mat = image.material;

            mat.SetFloat("_DissolveSprite", 0.75f);
        }
    }
}
