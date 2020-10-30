using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CraftingMats
{
    None,
    Wood,
    Stone,
    PurpleFlower,
    FireShard,
    Feather
}
public enum CraftingMatType
{
    None,
    Basic,
    Magical,
    Textile,
}
[Serializable]
public class CraftingMatEntry
{
    public CraftingMats mat;
    public CraftingMatType craftingMatType;
    public int amount;
}
public class PlayerManager : MonoBehaviour
{
    public List<PowerUp> activePowerups;

    public List<CraftingMatEntry> craftingMatsInInventory;

    public static PlayerManager Instance;
    private void Start()
    {
        Instance = this;

        MaterialsAndForgeManager.Instance.PopulateMaterialBag();
    }
}
