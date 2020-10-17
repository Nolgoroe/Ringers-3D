using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    public List<Cell> cells;

    public List<Piece> pieceSides;

    private void Awake()
    {
        GameManager.Instance.cellManager = this;
    }


    public void NeighborTest(int cellIndex)
    {

    }
}
