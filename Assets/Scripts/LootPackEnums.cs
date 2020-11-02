using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LootPacks ////// Names of loot packs containing loot packs
{
    None,
    M1,
    M2,
    M3,
    R1,
    R2,
    I1,
    I2,
    I3,
    I4,
    I5,
    I6,
    I7
}

[System.Serializable]
public enum ItemTables
{
    None,
    L1,
    L2,
    L3,
    L4,
    L5,
    L6,
    L7
}

[System.Serializable]
public class LootPackList
{
    public List<ItemTables> Pack;

    public bool IsMoneyOrRubies;

    public int[] minMaxValues;
}

[System.Serializable]
public class LootPackChancesList
{
    public int[] minMaxValues;
}

[System.Serializable]
public class ItemTablesToCraftingMats
{
    public List<CraftingMats> tableToMat;
}
public class LootPackEnums : MonoBehaviour
{
    public static LootPackEnums Instance;

    [Header("List for Loot by Level")]
    /////// LIST FOR LOOT BY LEVEL
    public List<ItemTablesToCraftingMats> listOfTablesToMats;

    [Header("List for Loot Packs")]
    /////// LIST FOR LOOTPACKS
    public List<LootPackList> listOfListItemTables1;

    [Header("List for Chances to Drop")]
    /////// LIST FOR CHANCES TO DROP
    public List<LootPackChancesList> listOfChances;

    public Dictionary<LootPacks, List<ItemTables>> lootpackEnumToList;
    public Dictionary<LootPacks, int[]> lootPackToValues;

    public Dictionary<List<ItemTables>, int[]> listOfTablesToChances;
    public Dictionary<ItemTables, List<CraftingMats>> itemTableToListOfMats;

    public Dictionary<LootPacks, bool> checkerForItemOrCurrency;

    private void Start()
    {
        Instance = this;



        lootpackEnumToList = new Dictionary<LootPacks, List<ItemTables>>();
        lootPackToValues = new Dictionary<LootPacks, int[]>();
        listOfTablesToChances = new Dictionary<List<ItemTables>, int[]>();
        itemTableToListOfMats = new Dictionary<ItemTables, List<CraftingMats>>();
        checkerForItemOrCurrency = new Dictionary<LootPacks, bool>();


        for (int i = 1; i < System.Enum.GetValues(typeof(LootPacks)).Length; i++)
        {
            if(!listOfListItemTables1[i - 1].IsMoneyOrRubies)
            {
                lootpackEnumToList.Add((LootPacks)i, listOfListItemTables1[i - 1].Pack);
            }
            else
            {
                lootPackToValues.Add((LootPacks)i, listOfListItemTables1[i - 1].minMaxValues);
            }
        }

        int counter = 0;
        for (int i = 0; i < listOfListItemTables1.Count; i++)
        {
            if (!listOfListItemTables1[i].IsMoneyOrRubies)
            {

                listOfTablesToChances.Add(listOfListItemTables1[i].Pack, listOfChances[counter].minMaxValues);
                counter++;
            }
        }

        for (int i = 1; i < System.Enum.GetValues(typeof(ItemTables)).Length; i++)
        {
            itemTableToListOfMats.Add((ItemTables)i, listOfTablesToMats[i - 1].tableToMat);
        }

        for (int i = 1; i <= listOfListItemTables1.Count; i++)
        {
            checkerForItemOrCurrency.Add((LootPacks)i, listOfListItemTables1[i - 1].IsMoneyOrRubies);
        }


        for (int i = 0; i < listOfListItemTables1.Count; i++) //// Debug to check if dict's work
        {
            //if (!listOfListItemTables1[i].IsMoneyOrRubies)
            //{
            //    Debug.Log(listOfTablesToChances[listOfListItemTables1[i].Pack]);

            //    foreach (int num in listOfTablesToChances[listOfListItemTables1[i].Pack])
            //    {
            //        Debug.Log(num);
            //    }
            //}
        } //// Debug to check if dict's work

        for (int i = 0; i < listOfListItemTables1.Count; i++) //// Debug to check if dict's work
        {
            if (!listOfListItemTables1[i].IsMoneyOrRubies)
            {
                //Debug.Log(lootpackEnumToList[(LootPacks)i + 1]);

                //foreach (ItemTables IT in lootpackEnumToList[(LootPacks)i + 1])
                //{
                //    Debug.Log(IT);
                //}

            }
            else
            {
                //Debug.Log(lootPackToValues[(LootPacks)i + 1]);

                //foreach (int num in lootPackToValues[(LootPacks)i + 1])
                //{
                //    Debug.Log(num);
                //}
            }
        } //// Debug to check if dict's work
    }
    public void RollOnTable(LootPacks lootPack)
    {
        if (!checkerForItemOrCurrency[lootPack])
        {
            List<ItemTables> tablesByLootPack = new List<ItemTables>();

            tablesByLootPack = lootpackEnumToList[lootPack];

            List<int> itemTableChanceValues = new List<int>();

            itemTableChanceValues.AddRange(listOfTablesToChances[tablesByLootPack]);

            List<CraftingMats> craftingMatsFromTables = new List<CraftingMats>();



            for (int i = 0; i < tablesByLootPack.Count; i++)
            {
                int chance = Random.Range(1, 101);

                craftingMatsFromTables.AddRange(itemTableToListOfMats[tablesByLootPack[i]]);

                if (chance > itemTableChanceValues[i])
                {
                    Debug.Log("Youa sucka Fuckkkkaeaeaeaeaeae");
                    craftingMatsFromTables.Clear();
                }
                else
                {
                    int randomMat = Random.Range(0, craftingMatsFromTables.Count);

                    Debug.Log(craftingMatsFromTables[randomMat]);
                    PlayerManager.Instance.AddMaterials(craftingMatsFromTables[randomMat], 5); //////// Figure out how to get amount from outside dynamically

                    craftingMatsFromTables.Clear();
                }
            }


        }
        else
        {
            int[] valuesToRecieve;
            valuesToRecieve = lootPackToValues[lootPack];

            if (lootPack.ToString().Contains("M"))
            {
                PlayerManager.Instance.AddGold(Random.Range(valuesToRecieve[0], valuesToRecieve[1] + 1));
            }
            else
            {
                PlayerManager.Instance.AddRubies(Random.Range(valuesToRecieve[0], valuesToRecieve[1] + 1));
            }
        }
    }
}
