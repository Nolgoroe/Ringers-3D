using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WardrobeManager : MonoBehaviour
{
    public Transform equipmentZone;

    public GameObject wardrobeEquipmentPrefab;
    public GameObject equippedPrefab;

    public List<WardrobeEquipmentDisplayer> equipmentInWardrobe;

    public EquipmentSlot[] equipmentSlots;

    //public Dictionary<slotType, EquipmentSlot> slotToSpot; ///// get slot for equipment enum and return the parent transform

    public static WardrobeManager Instance;

    private void Start()
    {
        Instance = this;

        //slotToSpot = new Dictionary<slotType, EquipmentSlot>();
        //for (int i = 1; i < System.Enum.GetValues(typeof(slotType)).Length; i++)
        //{
        //    slotToSpot.Add((slotType)i, equipmentSlots[i - 1]);
        //}
    }

    public void SpawnWardrobeEquipment(EquipmentData eData)
    {
        GameObject go = Instantiate(wardrobeEquipmentPrefab, equipmentZone);

        WardrobeEquipmentDisplayer WED = go.GetComponent<WardrobeEquipmentDisplayer>();

        WED.theEquipmentData = eData;

        go.name = eData.name;

        WED.equipmentImage.texture = Resources.Load(eData.spritePath) as Texture2D;

        string tmp = GameManager.Instance.powerupManager.spriteByType[eData.power];
        WED.powerupImage.texture = Resources.Load(tmp) as Texture2D;

        WED.equipementName.text = eData.name;

        switch (eData.scopeOfUses)
        {
            case 0:
                WED.equipmentUsage.text = eData.numOfUses.ToString() + " Per Day";
                break;
            case 1:
                WED.equipmentUsage.text = eData.numOfUses.ToString() + " Per Match";
                break;
            case -1:
                WED.equipmentUsage.text = eData.numOfUses.ToString() + " FUCK YOU";
                Debug.LogError("WTFFFFFFFFFFFFFFFFFFF");
                break;
            default:
                Debug.LogError("dOes NOt ExiSt BiaTch");
                break;
        }

        equipmentInWardrobe.Add(WED);
    }


    //public void EquipMe(EquipmentData data) //// Called only when loading the game
    //{
    //slotToSpot[data.slot].equipmentInSlot = data;
    //slotToSpot[data.slot].isFull = true;

    //GameObject go = Instantiate(equippedPrefab, slotToSpot[data.slot].transform);

    //go.GetComponentInChildren<RawImage>().texture = Resources.Load(data.spritePath) as Texture2D;
    //}

    public void UnEquipMe(EquipmentData data)
    {
        //if (slotToSpot[data.slot].isFull)
        //{
        //    slotToSpot[data.slot].isFull = false;

        //    PlayerManager.Instance.equippedItems.Remove(data);
        //    PlayerManager.Instance.wardrobeEquipment.Add(data);

        //    SpawnWardrobeEquipment(data);

        //    Destroy(slotToSpot[data.slot].transform.GetChild(0).gameObject);

        //    SortMaster.Instance.FilterWardrobe();

        //    PlayerManager.Instance.SavePlayerData();
        //}
    }
}
