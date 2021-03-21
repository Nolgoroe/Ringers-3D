using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public enum DocumentType
{
    Equipment,
    HollowObject
}

[System.Serializable]
public class csvFileInfo
{
    public TextAsset csvFiles;
    public DocumentType typeOfCsvDoc;
}
public class CSVParser : MonoBehaviour
{
    public List<EquipmentData> allEquipmentInGame;
    public List<HollowCraftObjectData> allHollowCraftObjectsInGame;

    public csvFileInfo[] csvFiles;

    StreamReader inputStream;

    string targetPath;

    void Start()
    {
        Debug.Log("Reading CSV");

        if (Application.platform == RuntimePlatform.Android)
        {
            targetPath = Application.persistentDataPath;

            foreach (csvFileInfo FI in csvFiles)
            {
                SaveToPersistentDataPath(FI);
            }
        }
        else
        {
            foreach (csvFileInfo FI in csvFiles)
            {
                readTextFile(FI);
            }
        }

    }

    public void SaveToPersistentDataPath(csvFileInfo FI)
    {
        File.WriteAllText(Application.persistentDataPath +"/" + FI.csvFiles.name + ".csv", FI.csvFiles.text);

        readTextFile(FI);
    }

    void readTextFile(csvFileInfo FI)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            switch (FI.typeOfCsvDoc)
            {
                case DocumentType.Equipment:
                    inputStream = new StreamReader(Application.persistentDataPath + "/Equipment.csv");
                    break;
                case DocumentType.HollowObject:
                    inputStream = new StreamReader(Application.persistentDataPath + "/Hollow Crafts.csv");
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (FI.typeOfCsvDoc)
            {
                case DocumentType.Equipment:
                    inputStream = new StreamReader("Assets/Resources/Equipment/Equipment.csv");
                    break;
                case DocumentType.HollowObject:
                    inputStream = new StreamReader("Assets/Resources/HollowObjects/Hollow Crafts.csv");
                    break;
                default:
                    break;
            }
        }

        List<string> lineList = new List<string>();

        while (!inputStream.EndOfStream)
        {
            string inputLine = inputStream.ReadLine();

            lineList.Add(inputLine);
        }

        inputStream.Close();

        parseList(lineList, FI.typeOfCsvDoc);
    }

    void parseList(List<string> stringList, DocumentType DT)
    {
        List<string[]> parsedList = new List<string[]>();
        for (int i = 0; i < stringList.Count; i++)
        {
            string[] temp = stringList[i].Split(',');
            for (int j = 0; j < temp.Length; j++)
            {
                temp[j] = temp[j].Trim();  //removed the blank spaces
            }
            parsedList.Add(temp);
        }

        switch (DT)
        {
            case DocumentType.Equipment:
                TranslateToEquipment(parsedList);
                break;
            case DocumentType.HollowObject:
                TranslateToHollowObjects(parsedList);
                break;
            default:
                break;
        }
    }

    public void TranslateToEquipment(List<string[]> parsedList)
    {

        for (int i = 1; i < parsedList.Count; i++) //// i = 1 becuase we are skipping the first row in the CSV file (they are the titles)
        {
            EquipmentData EQ = new EquipmentData();

            EQ.name = parsedList[i][0].ToString();
            EQ.slot = (slotType)Convert.ToInt16(parsedList[i][1]);

            if (parsedList[i][2].ToString().Contains("-"))
            {
                string[] temp = parsedList[i][2].ToString().Split('-');

                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = temp[j].Trim();
                }

                EQ.power = (PowerUp)Convert.ToInt16(temp[0]);

                if(EQ.power == PowerUp.FourColorTransform)
                {
                    EQ.specificColor = (PieceColor)Convert.ToInt16(temp[1]);
                    EQ.specificSymbol = PieceSymbol.None;
                }
                else
                {
                    EQ.specificSymbol = (PieceSymbol)Convert.ToInt16(temp[1]);
                    EQ.specificColor = PieceColor.None;
                }
            }
            else
            {
                EQ.power = (PowerUp)Convert.ToInt16(parsedList[i][2]);
                EQ.specificColor = PieceColor.None;
                EQ.specificSymbol = PieceSymbol.None;
            }

            if (parsedList[i][3].ToString().Contains("-"))
            {
                string[] temp = parsedList[i][3].ToString().Split('-');

                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = temp[j].Trim();
                }
                EQ.scopeOfUses = Convert.ToInt16(temp[0]);
                EQ.numOfUses = Convert.ToInt16(temp[1]);

                if (temp.Length == 3)
                {
                    EQ.timeForCooldown = Convert.ToInt16(temp[2]);
                }
            }
            else ///// Probably never going to enter here because every item has cooldown/charges (will contain '-')
            {
                EQ.numOfUses = Convert.ToInt16(parsedList[i][3]);
                EQ.scopeOfUses = -1;
            }
            EQ.mats = parsedList[i][4].ToString();

            EQ.spritePath = parsedList[i][5].ToString();

            allEquipmentInGame.Add(EQ);
        }


        MaterialsAndForgeManager.Instance.FillForge(allEquipmentInGame);
    }
    public void TranslateToHollowObjects(List<string[]> parsedList)
    {

        for (int i = 1; i < parsedList.Count; i++) //// i = 1 becuase we are skipping the first row in the CSV file (they are the titles)
        {
            HollowCraftObjectData HCOD = new HollowCraftObjectData();

            HCOD.objectHollowType = new List<ObjectHollowType>();

            HCOD.objectname = parsedList[i][0].ToString();

            if (parsedList[i][1].ToString().Contains("-"))
            {
                string[] temp = parsedList[i][1].ToString().Split('-');

                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = temp[j].Trim();
                }

                foreach (string hollowType in temp)
                {
                    HCOD.objectHollowType.Add((ObjectHollowType)Convert.ToInt16(hollowType));
                }
            }
            

            HCOD.mats = parsedList[i][2].ToString();

            HCOD.spritePath = parsedList[i][3].ToString();

            allHollowCraftObjectsInGame.Add(HCOD);
        }


        HollowCraftAndOwnedManager.Instance.FillCraftScreen(allHollowCraftObjectsInGame); /// Fill The Craft screen
    }
}
