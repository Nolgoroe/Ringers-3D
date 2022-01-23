using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HideTiles : MonoBehaviour
{
    [SerializeField]
    private string tileTag;
    [SerializeField]
    private Vector3 tileSize;
    [SerializeField]
    private float maxDistance;
    private GameObject[] tiles;
    // Use this for initialization
    void Start()
    {
        tiles = GameObject.FindGameObjectsWithTag(tileTag);
        DeactivateDistantTiles();
    }
    void DeactivateDistantTiles()
    {
        Vector3 playerPosition = this.gameObject.transform.position;

        foreach (GameObject tile in tiles)
        {
            Vector3 tilePosition = tile.gameObject.transform.position;

            float xDistance = Mathf.Abs(tilePosition.x - playerPosition.x);
            float zDistance = Mathf.Abs(tilePosition.z - playerPosition.z);
            float calcDistance = xDistance + zDistance;

            if (calcDistance > maxDistance)
            {
                tile.SetActive(false);
            }
            else
            {
                tile.SetActive(true);
            }
        }
    }
    void Update()
    {
        DeactivateDistantTiles();
    }

    public void updateMaxDistance(float multiplier)
    {
        maxDistance += multiplier;

        if (maxDistance < 75)
        {
            maxDistance = 75;
        }
    }
}