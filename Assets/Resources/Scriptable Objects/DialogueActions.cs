using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Create Dialogue Action")]
public class DialogueActions : ScriptableObject
{
    public void GiveSwitchPotion()
    {
        MaterialsAndForgeManager.Instance.UnlockPotion(PowerUp.Switch);
    }

    public void GiveJokerPotion()
    {
        MaterialsAndForgeManager.Instance.UnlockPotion(PowerUp.Joker);
    }
    public void GiveSliceBombPotion()
    {
        MaterialsAndForgeManager.Instance.UnlockPotion(PowerUp.SliceBomb);
    }
    public void GiveTileBombPotion()
    {
        MaterialsAndForgeManager.Instance.UnlockPotion(PowerUp.PieceBomb);
    }

    public void DeactivateAllGameplayUI()
    {
        UIManager.Instance.DeactivateOnDialogueStart();
    }
    public void ActivateAllGameplayUI()
    {
        UIManager.Instance.gameplayCanvas.SetActive(true);
    }
}
