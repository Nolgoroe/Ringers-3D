using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class pieceDataStruct
{
    public PieceColor colorOfPieceRight;
    public PieceSymbol symbolOfPieceRight;
    public PieceColor colorOfPieceLeft;
    public PieceSymbol symbolOfPieceLeft;
}

public class TutorialSequence : MonoBehaviour
{
    public static TutorialSequence Instacne;

    private void Start()
    {
        Instacne = this;
    }
}
