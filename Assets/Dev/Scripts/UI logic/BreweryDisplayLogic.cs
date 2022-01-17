using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

[Serializable]
public class CraftingMatsNeededToRubies
{
    public CraftingMats mat;
    public int amountMissing;

    public CraftingMatsNeededToRubies(CraftingMats INmat, int INamount)
    {
        mat = INmat;
        amountMissing = INamount;
    }
}
public class BreweryDisplayLogic : MonoBehaviour
{
    public RawImage mainIcon;
    public Button brewButton;
    public Transform[] matZones;

    public TMP_Text potionName, potionDescription;

    public EquipmentDisplayer selectedPotion;

    public List<CraftingMatsNeededToRubies> materialsNeedToBuyPotion;

    public int rubiesNeededToBuyPotion;

    public bool canForgePotion;

    private void Awake()
    {
        materialsNeedToBuyPotion = new List<CraftingMatsNeededToRubies>();
    }


    public void BreweryPotionDisplay(EquipmentDisplayer ED)
    {
        materialsNeedToBuyPotion.Clear();
        rubiesNeededToBuyPotion = 0;
        canForgePotion = false;

        ED.SpawnMaterialsNeeded(ED.data.mats, matZones);
        mainIcon.texture = Resources.Load(ED.data.spritePath) as Texture2D;

        potionName.text = ED.data.name;
        potionDescription.text = ED.data.Description;


        canForgePotion = ED.CheckIfCanForgeEquipment(ED.craftingMatsForEquipment);

        if (TutorialSequence.Instacne.duringSequence)
        {
            if (!canForgePotion)
            {
                if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isPotionTabPhase)
                {
                    TutorialSequence.Instacne.AddToPlayerMatsForPotion(materialsNeedToBuyPotion);
                    BreweryPotionDisplay(selectedPotion);
                }
            }
        }
    }

    public void SetSelectedPotion(EquipmentDisplayer ED)
    {
        brewButton.onClick.RemoveAllListeners();


        brewButton.onClick.AddListener(() => ED.ForgeItem(canForgePotion, false));
        brewButton.onClick.AddListener(() => BreweryPotionDisplay(ED));

        selectedPotion = ED;
        BreweryPotionDisplay(ED);

        StartCoroutine(MovepPotions(ED));
    }

    IEnumerator MovepPotions(EquipmentDisplayer ED)
    {
        yield return new WaitForEndOfFrame();

        foreach (EquipmentDisplayer equipment in MaterialsAndForgeManager.Instance.equipmentInForge)
        {
            if (equipment != ED)
            {
                RectTransform rect = equipment.GetComponent<RectTransform>();

                LeanTween.move(rect, equipment.originalPotionPosForSelection, 0.5f).setEase(LeanTweenType.easeInOutQuad); // animate
            }
            else
            {
                RectTransform rect = ED.GetComponent<RectTransform>();

                Debug.Log(rect.anchoredPosition.y + 50);
                Debug.Log(rect.anchoredPosition.x);

                LeanTween.move(rect, new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y + 50, 0) , 0.5f).setEase(LeanTweenType.easeInOutQuad); // animate
            }
        }
    }

    public void GetAllAnchorPositions()
    {
        Debug.Log("amount of calls");
        foreach (EquipmentDisplayer equipment in MaterialsAndForgeManager.Instance.equipmentInForge)
        {
            StartCoroutine(equipment.GetAnchoredPosition());
        }
    }

    public void AddNeededCraftingMatsToRubiesItem(CraftingMats _mat, int _amount)
    {
        CraftingMatsNeededToRubies CMNTR = new CraftingMatsNeededToRubies(_mat, _amount);

        materialsNeedToBuyPotion.Add(CMNTR);
    }

    public void CalculateNeededRubiesToBuyPotion()
    {
        rubiesNeededToBuyPotion = 0;

        foreach (CraftingMatsNeededToRubies CMNTR in materialsNeedToBuyPotion)
        {
            if(CMNTR.mat == CraftingMats.DewDrops)
            {
                for (int i = 0; i < CMNTR.amountMissing; i++)
                {
                    rubiesNeededToBuyPotion += PlayerManager.Instance.priceInGemsDewDrops;
                }
            }
            else
            {
                CraftingMatEntry CME = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == CMNTR.mat).Single();

                for (int i = 0; i < CMNTR.amountMissing; i++)
                {
                    rubiesNeededToBuyPotion += CME.amountPerPurchaseGems;
                }
            }
        }
    }

    public void BuyPotionAction()
    {
        if(PlayerManager.Instance.rubyCount >= rubiesNeededToBuyPotion)
        {
            PlayerManager.Instance.rubyCount -= rubiesNeededToBuyPotion;

            selectedPotion.ForgeItem(true, true);

            UIManager.Instance.updateRubyAndDewDropsCount();


            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
        }
    }
}
