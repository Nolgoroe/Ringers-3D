using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumImageAnimalData : MonoBehaviour
{
    public AnimalsInGame imageAnimalEnum;
    public bool isUnlocked;

    public Image animalImage;

    private void Start()
    {
        animalImage = GetComponent<Image>();
    }
}
