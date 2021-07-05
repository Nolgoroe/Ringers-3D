using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialsAndForgeManager : MonoBehaviour
{
    public static MaterialsAndForgeManager Instance;

    public GameObject equipmentForgeDisplayerPrefab;
    public GameObject crafingMatPrefab;
    public Transform equipmentContent; /// Parent
    public Dictionary<CraftingMats, string> materialSpriteByName;
    //public Dictionary<CraftingMatType, Transform> matTypeToParent;
    public Transform matInventoryParent;

    public List<EquipmentDisplayer> equipmentInForge; /// Equipment that the player does not have / has not created yet

    public string[] materialPaths;

    //public Transform[] matTypeCatagorieParents;
    private void Start()
    {
        Instance = this;
        materialSpriteByName = new Dictionary<CraftingMats, string>();
        //matTypeToParent = new Dictionary<CraftingMatType, Transform>();

        for (int i = 1; i <= materialPaths.Length; i++)
        {
            materialSpriteByName.Add((CraftingMats)i, materialPaths[i -1]);//// i - 1 becuase there is no "NONE" to paths, see the below comment
        }

        //for (int i = 1; i < Enum.GetValues(typeof(CraftingMatType)).Length; i++) /// i = 1 because we are skipping "None" ( 0 == None )
        //{
        //    matTypeToParent.Add((CraftingMatType)i, matTypeCatagorieParents[i - 1]); //// i - 1 becuase there is no "NONE" to transform, see the above comment
        //}
    }

    public void FillForge(List<EquipmentData> equipment)
    {

        foreach (EquipmentData EQ in equipment)
        {
            GameObject go = Instantiate(equipmentForgeDisplayerPrefab, equipmentContent);
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
            equipmentInForge.Add(ED);
            ED.SpawnMaterialsNeeded(EQ.mats);
        }
    }

    public void PopulateMaterialBagAll()
    {
        foreach (CraftingMatEntry CM in PlayerManager.Instance.craftingMatsInInventory)
        {
            if(CM.craftingMatType != CraftingMatType.None && CM.amount > 0)
            {
                GameObject go = Instantiate(crafingMatPrefab, matInventoryParent);

                CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

                CMD.materialImage.texture = Resources.Load(materialSpriteByName[CM.mat]) as Texture2D;
                CMD.materialCount.text = CM.amount.ToString();
            }
        }
    }
    public void RefreshMaterialBagSort(CraftingMatType type)
    {
        foreach (Transform Mat in matInventoryParent)
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
                GameObject go = Instantiate(crafingMatPrefab, matInventoryParent);

                CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

                CMD.materialImage.texture = Resources.Load(materialSpriteByName[CM.mat]) as Texture2D;
                CMD.materialCount.text = CM.amount.ToString();
            }
        }
    }

    [ContextMenu("Refresh Material Bag")]
    public void RefreshMaterialBag()
    {
        foreach (Transform Mat in matInventoryParent)
        {
            //for (int i = 0; i < Mat.childCount; i++)
            //{
                Destroy(Mat/*.GetChild(i)*/.gameObject);
            //}
        }

        PopulateMaterialBagAll();///// Ask Alon to explain Logic here
    }

    [ContextMenu("Refresh Forge")]
    public void RefreshForge()
    {
        equipmentInForge.Clear();

        foreach (Transform EQ in equipmentContent)
        {
            Destroy(EQ.gameObject);
        }

        FillForge(GameManager.Instance.csvParser.allEquipmentInGame);
    }
}
