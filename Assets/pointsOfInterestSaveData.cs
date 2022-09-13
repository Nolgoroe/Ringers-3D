using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointsOfInterestSaveData : MonoBehaviour
{
    public static pointsOfInterestSaveData instance;

    public List<CraftingMats> inventoryPointOfInterest;
    //public List<AnimalsInGame> AnimalAlbumPointOfInterest;

    private void Start()
    {
        instance = this;
    }

    public void AddToPointsOfInterest(CraftingMats craftingMatEnum)
    {
        if (!inventoryPointOfInterest.Contains(craftingMatEnum))
        {
            inventoryPointOfInterest.Add(craftingMatEnum);
        }
    }
}
