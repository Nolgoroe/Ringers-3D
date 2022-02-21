using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionAnimLogic : MonoBehaviour
{
    void DestroyMe()
    {
        Destroy(transform.parent.gameObject);
    }
}
