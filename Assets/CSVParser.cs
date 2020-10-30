using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSVParser : MonoBehaviour
{
    public List<EquipmentData> allEquipmentInGame;
    void Start()
    {
        readTextFile();
    }

    void readTextFile()
    {
        StreamReader inputStream = new StreamReader("Assets/Resources/Equipment/Equipment.csv");
        List<string> lineList = new List<string>();

        while (!inputStream.EndOfStream)
        {
            string inputLine = inputStream.ReadLine();

            lineList.Add(inputLine);
        }

        inputStream.Close();

        parseList(lineList);
    }

    void parseList(List<string> stringList)
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

        TranslateToEquipment(parsedList);
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
}
