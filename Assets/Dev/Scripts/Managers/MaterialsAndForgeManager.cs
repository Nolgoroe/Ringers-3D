using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MaterialsAndForgeManager : MonoBehaviour
{
    public static MaterialsAndForgeManager Instance;

    //public GameObject equipmentForgeDisplayerPrefab;
    public GameObject brewScreeneDisplayerPrefab;
    public GameObject corruptedDeviceDisplayerPrefab;
    public GameObject crafingMatPrefab;
    public Transform brewScreenPotionContentZone; 
    public Transform matInventoryContent;
    public Transform ForgeContent;

    //public Dictionary<CraftingMats, string> materialSpriteByName;
    //public Dictionary<CraftingMatType, Transform> matTypeToParent;

    public List<EquipmentDisplayer> equipmentInBrewScreen; /// Equipment that the player does not have / has not created yet
    public List<CorruptedDevicesDisplayer> corruptedDevicesInForge; /// Equipment that the player does not have / has not created yet

    //public string[] materialPaths;

    //public Transform[] matTypeCatagorieParents;


    public PowerUp currentTest;

    private void Start()
    {
        Instance = this;
        //materialSpriteByName = new Dictionary<CraftingMats, string>();
        ////matTypeToParent = new Dictionary<CraftingMatType, Transform>();

        //for (int i = 1; i <= materialPaths.Length; i++)
        //{
        //    materialSpriteByName.Add((CraftingMats)i, materialPaths[i -1]);//// i - 1 becuase there is no "NONE" to paths, see the below comment
        //}

        //for (int i = 1; i < Enum.GetValues(typeof(CraftingMatType)).Length; i++) /// i = 1 because we are skipping "None" ( 0 == None )
        //{
        //    matTypeToParent.Add((CraftingMatType)i, matTypeCatagorieParents[i - 1]); //// i - 1 becuase there is no "NONE" to transform, see the above comment
        //}
    }

    public void Init()
    {
        StartCoroutine(FillBrewScreen(PlayerManager.Instance.unlockedPowerups));
    }

    [ContextMenu("UnlockPotion")]
    public void callUnlockPotion()
    {
        UnlockPotion(currentTest);
    }

    public void UnlockPotion(PowerUp potionType)
    {
        if(!PlayerManager.Instance.unlockedPowerups.Contains(potionType))
        {
            PlayerManager.Instance.unlockedPowerups.Add(potionType);
        }


        EquipmentData EQ = GameManager.Instance.csvParser.allEquipmentInGame.Where(p => p.power == potionType).SingleOrDefault();
        if (EQ == null)
        {
            Debug.LogError("Error potions here");
            return;
        }

        StartCoroutine(FillBrewScreen(PlayerManager.Instance.unlockedPowerups));

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
    }

    public IEnumerator FillBrewScreen(List<PowerUp> powerTypes)
    {
        equipmentInBrewScreen.Clear();

        foreach (Transform EQ in brewScreenPotionContentZone)
        {
            Destroy(EQ.gameObject);
        }

        yield return new WaitForEndOfFrame();

        foreach (PowerUp power in powerTypes)
        {
            EquipmentData EQ = GameManager.Instance.csvParser.allEquipmentInGame.Where(p => p.power == power).SingleOrDefault();
            
            if(EQ == null)
            {
                Debug.LogError("Error potions here");
                yield break;
            }
            
            GameObject go = Instantiate(brewScreeneDisplayerPrefab, brewScreenPotionContentZone);
            EquipmentDisplayer ED = go.GetComponent<EquipmentDisplayer>();

            ED.itemName.text = EQ.name;
            ED.data = EQ;

            switch (EQ.scopeOfUses)
            {
                case 0:
                    ED.usageCount.text = EQ.numOfUses.ToString() + " Per Day";
                    break;
                case 1:
                    ED.usageCount.text = EQ.numOfUses.ToString() + " Per Match";
                    break;
                case -1:
                    ED.usageCount.text = EQ.numOfUses.ToString() + " FUCK YOU";
                    Debug.LogError("WTFFFFFFFFFFFFFFFFFFF");
                    break;
                default:
                    Debug.LogError("dOes NOt ExiSt BiaTch");
                    break;
            }
            string tmp = GameManager.Instance.powerupManager.spriteByType[EQ.power];
            //ED.powerUp.texture = Resources.Load(tmp) as Texture2D;
            ED.itemImage.texture = Resources.Load(EQ.spritePath)as Texture2D;

            ED.name = ED.itemName.text;


            equipmentInBrewScreen.Add(ED);
            //ED.SpawnMaterialsNeeded(EQ.mats);
        }

        //foreach (EquipmentData EQ in equipment)
        //{
        //    GameObject go = Instantiate(brewScreeneDisplayerPrefab, brewScreenPotionContentZone);
        //    EquipmentDisplayer ED = go.GetComponent<EquipmentDisplayer>();

        //    ED.itemName.text = EQ.name;
        //    ED.data = EQ;

        //    switch (EQ.scopeOfUses)
        //    {
        //        case 0:
        //            ED.usageCount.text = EQ.numOfUses.ToString() + " Per Day";
        //            break;
        //        case 1:
        //            ED.usageCount.text = EQ.numOfUses.ToString() + " Per Match";
        //            break;
        //        case -1:
        //            ED.usageCount.text = EQ.numOfUses.ToString() + " FUCK YOU";
        //            Debug.LogError("WTFFFFFFFFFFFFFFFFFFF");
        //            break;
        //        default:
        //            Debug.LogError("dOes NOt ExiSt BiaTch");
        //            break;
        //    }
        //    string tmp = GameManager.Instance.powerupManager.spriteByType[EQ.power];
        //    //ED.powerUp.texture = Resources.Load(tmp) as Texture2D;
        //    ED.itemImage.texture = Resources.Load(EQ.spritePath)as Texture2D;

        //    ED.name = ED.itemName.text;


        //    equipmentInBrewScreen.Add(ED);
        //    //ED.SpawnMaterialsNeeded(EQ.mats);
        //}
    }

    public void PopulateMaterialBagAll()
    {
        foreach (CraftingMatEntry CM in PlayerManager.Instance.craftingMatsInInventory)
        {
            if(CM.craftingMatType != CraftingMatType.None && CM.amount > 0)
            {
                GameObject go = Instantiate(crafingMatPrefab, matInventoryContent);

                CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

                //CMD.materialImage.texture = Resources.Load(materialSpriteByName[CM.mat]) as Texture2D;
                CMD.materialImage.sprite = LootManager.Instance.allMaterialSprites[(int)CM.mat];
                CMD.materialCount.text = CM.amount.ToString();
            }
        }
    }
    public void RefreshMaterialBagSort(CraftingMatType type)
    {
        foreach (Transform Mat in matInventoryContent)
        {
            //for (int i = 0; i < Mat.childCount; i++)
            //{
            Destroy(Mat/*.GetChild(i)*/.gameObject);
            //}
        }

        foreach (CraftingMatEntry CM in PlayerManager.Instance.craftingMatsInInventory)
        {
            if(CM.craftingMatType == type && CM.amount > 0)
            {
                GameObject go = Instantiate(crafingMatPrefab, matInventoryContent);

                CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

                CMD.materialImage.sprite = LootManager.Instance.allMaterialSprites[(int)CM.mat];
                CMD.materialCount.text = CM.amount.ToString();
                CMD.craftingMatEnum = CM.mat;

                CMD.CheckShowPointInterestList(CM.mat);

                RemoveBulkFromPointsOfInterestInventory(CMD, type);
            }
        }
    }

    public void RemoveBulkFromPointsOfInterestInventory(CraftingMatDisplayer CMD, CraftingMatType type)
    {
        CMD.GetComponent<PointOfInterest>().HideInterestPointImage();

        if (pointsOfInterestSaveData.instance.inventoryPointOfInterest.Contains(CMD.craftingMatEnum))
        {
            pointsOfInterestSaveData.instance.inventoryPointOfInterest.Remove(CMD.craftingMatEnum);
        }

        foreach (InventorySortButtonData sortButton in UIManager.Instance.inventorySortButtons)
        {
            if (sortButton.id == (int)type)
            {
                sortButton.GetComponent<PointOfInterest>().HideInterestPointImage();
            }
        }

        if (pointsOfInterestSaveData.instance.inventoryPointOfInterest.Count <= 0)
        {
            InterestPointsManager.instance.TurnOffPointsOfInterestDisplay(TypesPointOfInterest.inventory);
        }
    }

    [ContextMenu("Refresh Material Bag")]
    public void RefreshMaterialBag()
    {
        foreach (Transform Mat in matInventoryContent)
        {
            //for (int i = 0; i < Mat.childCount; i++)
            //{
                Destroy(Mat/*.GetChild(i)*/.gameObject);
            //}
        }

        PopulateMaterialBagAll();
    }

    [ContextMenu("Refresh Forge")]
    public void RefreshForge()
    {
        StartCoroutine(FillBrewScreen(PlayerManager.Instance.unlockedPowerups));
    }



    public void FillCorruptionDevices(List<CorruptedDevicesData> corruptedDevices)
    {
        corruptedDevicesInForge.Clear();

        foreach (CorruptedDevicesData CDD in corruptedDevices)
        {
            GameObject go = Instantiate(corruptedDeviceDisplayerPrefab, ForgeContent);
            CorruptedDevicesDisplayer CDDisplay = go.GetComponent<CorruptedDevicesDisplayer>();

            //CDDisplay.itemName.text = CDD.deviceName;
            CDDisplay.data = CDD;

            CDDisplay.itemImage.texture = Resources.Load(CDD.spritePath) as Texture2D;

            //CDDisplay.name = CDDisplay.itemName.text;
            corruptedDevicesInForge.Add(CDDisplay);
            CDDisplay.SpawnMaterialsNeeded(CDD.mats);
        }
    }

    public void RefreshCorruptionDevices()
    {
        corruptedDevicesInForge.Clear();

        //foreach (Transform EQ in equipmentContent)
        //{
        //    Destroy(EQ.gameObject);
        //}

        //FillCorruptionDevices(GameManager.Instance.csvParser.allCorruptedDevicesInGame);
    }

    public void ResetPotionDataAfterTutorial()
    {
        foreach (EquipmentDisplayer equipment in equipmentInBrewScreen)
        {
            equipment.selectPotionButton.onClick.AddListener(() => equipment.BDL.SetSelectedPotion(equipment));
            equipment.selectPotionButton.onClick.AddListener(() => SoundManager.Instance.PlaySound(Sounds.ButtonPressUI));
        }

    }
}
