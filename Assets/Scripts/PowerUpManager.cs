using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUp
{
    None,
    Joker,
    Switch,
    PieceBomb,
    SliceBomb,
    ExtraDeal,
    Deal,
    FourColorTransform,
    FourShapeTransform
}
public class PowerUpManager : MonoBehaviour
{
    public void ActivatePowerUp(PowerUp ThePower)
    {
        switch (ThePower)
        {
            case PowerUp.Joker:
                break;
            case PowerUp.Switch:
                break;
            case PowerUp.PieceBomb:
                break;
            case PowerUp.SliceBomb:
                break;
            case PowerUp.ExtraDeal:
                break;
            case PowerUp.Deal:
                break;
            case PowerUp.FourColorTransform:
                break;
            case PowerUp.FourShapeTransform:
                break;
            default:
                break;
        }
    }
}
