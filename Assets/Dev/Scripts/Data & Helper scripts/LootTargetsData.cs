using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTargetsData : MonoBehaviour
{
    public Transform /*goldTargetLoot,*/ rubyTargetLoot, materialsTargetLoot;

    public static LootTargetsData instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
}
