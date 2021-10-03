using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEasyLifeScript : MonoBehaviour
{
    public GameObject[] turnoff;

    public void turnoffall()
    {
        for (int i = 0; i < turnoff.Length; i++)
        {
            turnoff[i].gameObject.SetActive(false);
        }

        GameManager.Instance.gameBoard.SetActive(false);
        GameManager.Instance.gameClip.SetActive(false);
    }
  }