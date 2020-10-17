using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isFull;
    public int cellIndex;


    public void AddPiece(Transform followerTarget, bool isNew)
    {
        isFull = true;
        followerTarget.SetParent(transform);
        followerTarget.position = new Vector3(followerTarget.parent.position.x, followerTarget.parent.position.y, followerTarget.parent.position.z + 0.1f);
        followerTarget.rotation = followerTarget.parent.rotation;

        if (isNew)
        {
            GameManager.Instance.currentFilledCellCount++;
        }
        GameManager.Instance.cellManager.NeighborTest(cellIndex);
    }
}
