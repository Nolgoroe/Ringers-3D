using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cluster", menuName = "ScriptableObjects/Create Cluster")]
public class ClusterScriptableObject : ScriptableObject
{
    public LevelScriptableObject[] clusterLevels;

    public List<ListLootChest> chestLootPacksCluster;

    public GameObject specificClsuterStatue;

}
