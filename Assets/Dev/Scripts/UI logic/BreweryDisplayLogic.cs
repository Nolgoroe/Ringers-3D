using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreweryDisplayLogic : MonoBehaviour
{
    public RawImage mainIcon;
    public Button brewButton;
    public Transform[] matZones;

    public TMP_Text potionName, potionDescription;

    public EquipmentDisplayer selectedPotion;

    public void BreweryPotionDisplay(EquipmentDisplayer ED)
    {
        ED.SpawnMaterialsNeeded(ED.data.mats, matZones);
        mainIcon.texture = Resources.Load(ED.data.spritePath) as Texture2D;

        potionName.text = ED.data.name;
        potionDescription.text = ED.data.Description;


        ED.CheckIfCanForgeEquipment(ED.craftingMatsForEquipment);
    }

    public void SetSelectedPotion(EquipmentDisplayer ED)
    {
        brewButton.onClick.RemoveAllListeners();

        brewButton.onClick.AddListener(() => ED.ForgeItem());
        brewButton.onClick.AddListener(() => BreweryPotionDisplay(ED));

        selectedPotion = ED;
        BreweryPotionDisplay(ED);
    }
}
