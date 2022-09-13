using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum TypesPointOfInterest
{
    inventory,
    AnimalAlbum
}

[System.Serializable]
public class TypeToObjectsOfInterestClass
{
    public TypesPointOfInterest typeInterest;
    public PointOfInterest[] pointsOfInterestForType;
}
public class InterestPointsManager : MonoBehaviour
{
    public static InterestPointsManager instance;

    public TypeToObjectsOfInterestClass[] generalPointsOfInterest;
    public Dictionary<TypesPointOfInterest, TypeToObjectsOfInterestClass> typeInterestToPointInterest;

    void Start()
    {
        instance = this;


        typeInterestToPointInterest = new Dictionary<TypesPointOfInterest, TypeToObjectsOfInterestClass>();

        for (int i = 0; i < System.Enum.GetValues(typeof(TypesPointOfInterest)).Length; i++)
        {
            typeInterestToPointInterest.Add((TypesPointOfInterest)i, generalPointsOfInterest[i]);
        }

        foreach (TypeToObjectsOfInterestClass mainList in generalPointsOfInterest)
        {
            foreach (PointOfInterest point in mainList.pointsOfInterestForType)
            {
                point.HideInterestPointImage();
            }
        }
    }

    public void TurnOnPointsOfInterestDisplay(TypesPointOfInterest type)
    {
        foreach (var typeToPoints in typeInterestToPointInterest)
        {
            if (typeToPoints.Key == type)
            {
                foreach (var pointOfInterest in typeToPoints.Value.pointsOfInterestForType)
                {
                    pointOfInterest.ShowInterestPointImage();
                }
            }
        }

        if(type == TypesPointOfInterest.inventory)
        {
            PointsOfInterestInventorySort();
        }

        if(type == TypesPointOfInterest.AnimalAlbum)
        {
            PointsOfInterestAnimalAlbumSort();
        }
    }
    public void TurnOffPointsOfInterestDisplay(TypesPointOfInterest type)
    {
        foreach (var typeToPoints in typeInterestToPointInterest)
        {
            if (typeToPoints.Key == type)
            {
                foreach (var pointOfInterest in typeToPoints.Value.pointsOfInterestForType)
                {
                    pointOfInterest.HideInterestPointImage();
                }
            }
        }
    }

    public void PointsOfInterestInventorySort()
    {
        foreach (InventorySortButtonData sortButton in UIManager.Instance.inventorySortButtons)
        {
            sortButton.GetComponent<PointOfInterest>().HideInterestPointImage();
        }

        foreach (CraftingMats mat in pointsOfInterestSaveData.instance.inventoryPointOfInterest)
        {
            CraftingMatEntry matEntry = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == mat).SingleOrDefault();

            foreach (InventorySortButtonData sortButton in UIManager.Instance.inventorySortButtons)
            {
                if (sortButton.id == (int)matEntry.craftingMatType)
                {
                    sortButton.GetComponent<PointOfInterest>().ShowInterestPointImage();
                }
            }
        }

    }
    public void PointsOfInterestAnimalAlbumSort()
    {
        foreach (InventorySortButtonData sortButton in UIManager.Instance.animalAlbumSortButtons)
        {
            sortButton.GetComponent<PointOfInterest>().HideInterestPointImage();
        }

        foreach (OwnedAnimalDataSet ownedAnimal in AnimalsManager.Instance.unlockedAnimals)
        {
            if(!AnimalsManager.Instance.revealedAnimalsInAlbum.Contains(ownedAnimal.animalEnum))
            {
                foreach (InventorySortButtonData sortButton in UIManager.Instance.animalAlbumSortButtons)
                {
                    if (sortButton.id == (int)ownedAnimal.animalTypeEnum)
                    {
                        sortButton.GetComponent<PointOfInterest>().ShowInterestPointImage();
                    }
                }
            }
        }
    }

    public bool CheckAdditionalPointsOfInterestAnimalAlbum()
    {
        foreach (OwnedAnimalDataSet ownedAnimal in AnimalsManager.Instance.unlockedAnimals)
        {
            if (!AnimalsManager.Instance.revealedAnimalsInAlbum.Contains(ownedAnimal.animalEnum))
            {
                foreach (InventorySortButtonData sortButton in UIManager.Instance.animalAlbumSortButtons)
                {
                    if (sortButton.id == (int)ownedAnimal.animalTypeEnum)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    public bool CheckAdditionalPointsOfInterestInventory()
    {
        if(pointsOfInterestSaveData.instance.inventoryPointOfInterest.Count > 0)
        {
            return true;
        }
        return false;
    }
}
